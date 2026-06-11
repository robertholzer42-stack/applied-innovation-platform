using UnityEngine;
using UnityEngine.AI;
using Pridefall.Core;

namespace Pridefall.Enemies
{
    /// <summary>
    /// Armored native crawler. Patrols waypoints, telegraphs (rear up) when
    /// it sees the player, then charges on the NavMesh. Connecting deals
    /// Kinetic damage and briefly self-stuns; charging into terrain stuns it
    /// much longer, which is the intended flank window. Armor is data, not
    /// code: child hitbox colliders carry DamageRelay (top plates 0.25x,
    /// belly 3x), so this class only ever sees pre-scaled damage.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyPerception))]
    public class ShardbackCrawler : EnemyBase
    {
        [Header("Patrol")]
        [SerializeField] private Transform[] _waypoints;
        [SerializeField] private float _patrolSpeed = 1.6f;
        [SerializeField] private float _waypointTolerance = 0.6f;
        [SerializeField] private float _investigateSpeed = 2.5f;

        [Header("Charge")]
        [Tooltip("Telegraph time before the charge. The rear-up animation keys off EnterState(Attack).")]
        [SerializeField] private float _windupSeconds = 0.9f;
        [SerializeField] private float _chargeSpeed = 9f;
        [SerializeField] private float _chargeAcceleration = 40f;
        [SerializeField] private float _chargeMaxSeconds = 2.5f;
        [Tooltip("Meters past the player the charge line extends, so near-misses carry through.")]
        [SerializeField] private float _chargeOvershoot = 3f;
        [SerializeField] private float _chargeDamage = 30f;
        [SerializeField] private float _hitRadius = 1.2f;
        [SerializeField] private float _turnDegreesPerSecond = 160f;

        [Header("Stuns")]
        [Tooltip("Self-stun after connecting with the player.")]
        [SerializeField] private float _postHitStun = 0.8f;
        [Tooltip("Stun after slamming into terrain. This is the flank/belly window.")]
        [SerializeField] private float _missStun = 2.5f;

        private enum ChargePhase { Telegraph, Charge }

        private EnemyPerception _perception;
        private ChargePhase _phase;
        private int _waypointIndex;
        private float _phaseEndTime;
        private float _chargeStartTime;
        private bool _dealtChargeDamage;
        private float _baseAcceleration;

        protected override void Awake()
        {
            base.Awake();
            _perception = GetComponent<EnemyPerception>();
            _baseAcceleration = Agent != null ? Agent.acceleration : 8f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_perception != null) _perception.OnNoiseHeard += OnNoiseHeard;
        }

        private void OnDisable()
        {
            if (_perception != null) _perception.OnNoiseHeard -= OnNoiseHeard;
        }

        // Editor-time defaults per the GDD creature table.
        private void Reset()
        {
            _maxHealth = 120f;
            _scrapDrop = 0; // fauna pay in bio-resin, not scrap
            _staggerThreshold = 40f; // only belly hits (3x relay) realistically stagger it
        }

        private void OnNoiseHeard(Vector3 position)
        {
            if (CurrentState == EnemyState.Idle || CurrentState == EnemyState.Patrol)
            {
                ChangeState(EnemyState.Investigate);
            }
        }

        protected override void EnterState(EnemyState state)
        {
            base.EnterState(state);
            if (Agent == null || !Agent.enabled) return;

            switch (state)
            {
                case EnemyState.Idle:
                    SetAgentStopped(true);
                    break;

                case EnemyState.Patrol:
                    Agent.updateRotation = true;
                    Agent.acceleration = _baseAcceleration;
                    Agent.speed = _patrolSpeed;
                    SetAgentStopped(false);
                    SetWaypointDestination();
                    break;

                case EnemyState.Investigate:
                    Agent.updateRotation = true;
                    Agent.acceleration = _baseAcceleration;
                    Agent.speed = _investigateSpeed;
                    SetAgentStopped(false);
                    if (_perception != null) SetDestinationSafe(_perception.LastKnownPlayerPosition);
                    break;

                case EnemyState.Attack:
                    BeginTelegraph();
                    break;
            }
        }

        protected override void TickIdle()
        {
            if (SeesLivePlayer()) { ChangeState(EnemyState.Attack); return; }
            if (_waypoints != null && _waypoints.Length > 0) ChangeState(EnemyState.Patrol);
        }

        protected override void TickPatrol()
        {
            if (SeesLivePlayer()) { ChangeState(EnemyState.Attack); return; }
            if (Agent == null || !Agent.enabled || !Agent.isOnNavMesh) return;
            if (_waypoints == null || _waypoints.Length == 0) { ChangeState(EnemyState.Idle); return; }

            if (!Agent.pathPending && Agent.remainingDistance <= _waypointTolerance)
            {
                _waypointIndex = (_waypointIndex + 1) % _waypoints.Length;
                SetWaypointDestination();
            }
        }

        protected override void TickInvestigate()
        {
            if (SeesLivePlayer()) { ChangeState(EnemyState.Attack); return; }
            if (Agent == null || !Agent.enabled || !Agent.isOnNavMesh) { ChangeState(EnemyState.Idle); return; }

            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.2f)
            {
                ChangeState(_waypoints != null && _waypoints.Length > 0 ? EnemyState.Patrol : EnemyState.Idle);
            }
        }

        protected override void TickAttack()
        {
            if (!PlayerIsAlive) { ChangeState(EnemyState.Idle); return; }
            if (Agent == null || !Agent.enabled || !Agent.isOnNavMesh) { ChangeState(EnemyState.Idle); return; }

            switch (_phase)
            {
                case ChargePhase.Telegraph:
                    FacePlayer();
                    if (Time.time < _phaseEndTime) return;

                    if (_perception != null && !_perception.CanSeePlayer) ChangeState(EnemyState.Investigate);
                    else BeginCharge();
                    break;

                case ChargePhase.Charge:
                    if (!_dealtChargeDamage)
                    {
                        Vector3 toPlayer = PlayerPosition - transform.position;
                        if (toPlayer.sqrMagnitude <= _hitRadius * _hitRadius)
                        {
                            _dealtChargeDamage = true;
                            TryDamagePlayer(_chargeDamage, DamageType.Kinetic, transform.position + toPlayer * 0.5f, toPlayer.normalized, gameObject);
                            HaltCharge();
                            Stagger(_postHitStun);
                            return;
                        }
                    }

                    // Stalled against geometry or ran the line out without connecting: long stun, belly exposed.
                    bool stalled = Time.time - _chargeStartTime > 0.3f && Agent.velocity.sqrMagnitude < 0.5f;
                    bool ranOut = !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.1f;
                    if (stalled || ranOut)
                    {
                        HaltCharge();
                        Stagger(_missStun);
                        return;
                    }

                    if (Time.time - _chargeStartTime > _chargeMaxSeconds)
                    {
                        HaltCharge();
                        BeginTelegraph();
                    }
                    break;
            }
        }

        private void BeginTelegraph()
        {
            _phase = ChargePhase.Telegraph;
            _phaseEndTime = Time.time + _windupSeconds;
            if (Agent != null && Agent.enabled)
            {
                Agent.updateRotation = false; // we rotate manually to face the player
                SetAgentStopped(true);
            }
        }

        private void BeginCharge()
        {
            Vector3 dir = PlayerPosition - transform.position;
            dir.y = 0f;
            dir = dir.sqrMagnitude > 0.01f ? dir.normalized : transform.forward;

            _phase = ChargePhase.Charge;
            _chargeStartTime = Time.time;
            _dealtChargeDamage = false;

            Agent.updateRotation = true;
            Agent.acceleration = _chargeAcceleration;
            Agent.speed = _chargeSpeed;
            SetAgentStopped(false);
            SetDestinationSafe(PlayerPosition + dir * _chargeOvershoot);
        }

        private void HaltCharge()
        {
            if (Agent == null || !Agent.enabled || !Agent.isOnNavMesh) return;
            Agent.velocity = Vector3.zero;
            Agent.ResetPath();
            Agent.acceleration = _baseAcceleration;
        }

        private void FacePlayer()
        {
            Vector3 to = PlayerPosition - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude < 0.01f) return;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(to), _turnDegreesPerSecond * Time.deltaTime);
        }

        private bool SeesLivePlayer()
        {
            return _perception != null && _perception.CanSeePlayer && PlayerIsAlive;
        }

        private void SetWaypointDestination()
        {
            if (_waypoints == null || _waypoints.Length == 0) return;
            Transform waypoint = _waypoints[_waypointIndex % _waypoints.Length];
            if (waypoint != null) SetDestinationSafe(waypoint.position);
        }

        private void SetDestinationSafe(Vector3 destination)
        {
            if (Agent != null && Agent.enabled && Agent.isOnNavMesh) Agent.SetDestination(destination);
        }

        private void SetAgentStopped(bool stopped)
        {
            if (Agent != null && Agent.enabled && Agent.isOnNavMesh) Agent.isStopped = stopped;
        }
    }
}
