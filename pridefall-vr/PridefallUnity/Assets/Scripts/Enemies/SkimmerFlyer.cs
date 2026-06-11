using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Enemies
{
    /// <summary>
    /// Native flying harasser. No NavMeshAgent: it orbits a home point with a
    /// sine bob, swoops at the player's head when it has sight, deals Kinetic
    /// contact damage on the pass, climbs back to altitude, and repeats.
    /// Per the GDD creature table it dies to one or two sidearm shots.
    /// </summary>
    [RequireComponent(typeof(EnemyPerception))]
    public class SkimmerFlyer : EnemyBase
    {
        [Header("Hover")]
        [Tooltip("Ground-level marker the Skimmer hovers above. Falls back to the spawn position.")]
        [SerializeField] private Transform _homePoint;
        [SerializeField] private float _hoverHeight = 5f;
        [SerializeField] private float _orbitRadius = 3.5f;
        [SerializeField] private float _orbitDegreesPerSecond = 45f;
        [SerializeField] private float _bobAmplitude = 0.4f;
        [Tooltip("Bob cycles per second.")]
        [SerializeField] private float _bobFrequency = 0.5f;
        [SerializeField] private float _hoverSpeed = 5f;

        [Header("Dive")]
        [SerializeField] private float _diveSpeed = 14f;
        [SerializeField] private float _contactDamage = 12f;
        [SerializeField] private float _contactRadius = 0.8f;
        [SerializeField] private float _diveCooldown = 2.5f;
        [Tooltip("How sharply velocity bends toward the steering target. Higher = twitchier.")]
        [SerializeField] private float _turnResponsiveness = 3.5f;

        private enum SwoopPhase { Cooldown, Dive, Recover }

        private EnemyPerception _perception;
        private Vector3 _homePosition;
        private Vector3 _velocity;
        private float _orbitAngle;
        private SwoopPhase _phase;
        private float _nextDiveTime;
        private Vector3 _diveTarget;
        private Vector3 _recoverTarget;
        private bool _bitThisDive;

        protected override void Awake()
        {
            base.Awake();
            _perception = GetComponent<EnemyPerception>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _homePosition = _homePoint != null ? _homePoint.position : transform.position;
            _velocity = Vector3.zero;
            _orbitAngle = Random.Range(0f, 360f);
            if (_perception != null) _perception.OnNoiseHeard += OnNoiseHeard;
        }

        private void OnDisable()
        {
            if (_perception != null) _perception.OnNoiseHeard -= OnNoiseHeard;
        }

        // Editor-time defaults per the GDD creature table.
        private void Reset()
        {
            _maxHealth = 20f;
            _scrapDrop = 0; // fauna pay in bio-resin, not scrap
            _staggerThreshold = 15f;
            _despawnDelay = 3f;
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
            if (state == EnemyState.Attack)
            {
                _phase = SwoopPhase.Cooldown;
                _nextDiveTime = Time.time + 0.5f; // a beat to line up before the first swoop
            }
        }

        protected override void TickIdle()
        {
            HoverAround(_homePosition);
            if (_perception != null && _perception.CanSeePlayer && PlayerIsAlive) ChangeState(EnemyState.Attack);
        }

        protected override void TickInvestigate()
        {
            if (_perception == null) { ChangeState(EnemyState.Idle); return; }
            if (_perception.CanSeePlayer && PlayerIsAlive) { ChangeState(EnemyState.Attack); return; }

            Vector3 target = _perception.LastKnownPlayerPosition + Vector3.up * _hoverHeight;
            Vector3 desired = Vector3.ClampMagnitude((target - transform.position) * 1.5f, _hoverSpeed);
            IntegrateSteering(desired, ref _velocity, _turnResponsiveness);

            if ((target - transform.position).sqrMagnitude < 2f) ChangeState(EnemyState.Idle);
        }

        protected override void TickAttack()
        {
            if (!PlayerIsAlive) { ChangeState(EnemyState.Idle); return; }
            Transform head = PlayerHead;
            if (head == null) { ChangeState(EnemyState.Idle); return; }

            switch (_phase)
            {
                case SwoopPhase.Cooldown:
                    if (_perception != null && !_perception.CanSeePlayer) { ChangeState(EnemyState.Investigate); return; }
                    HoverAround(head.position); // circle above the player between passes
                    if (Time.time >= _nextDiveTime) BeginDive(head);
                    break;

                case SwoopPhase.Dive:
                    // Mild homing: strafing dodges the swoop, standing still does not.
                    _diveTarget = Vector3.MoveTowards(_diveTarget, head.position, 2f * Time.deltaTime);
                    Vector3 toTarget = _diveTarget - transform.position;
                    IntegrateSteering(toTarget.normalized * _diveSpeed, ref _velocity, _turnResponsiveness * 2f);

                    TryBite(head);

                    if (toTarget.sqrMagnitude < 1f || Vector3.Dot(_velocity, toTarget) < 0f) BeginRecover();
                    break;

                case SwoopPhase.Recover:
                    Vector3 desired = Vector3.ClampMagnitude((_recoverTarget - transform.position) * 1.5f, _hoverSpeed);
                    IntegrateSteering(desired, ref _velocity, _turnResponsiveness);
                    if ((_recoverTarget - transform.position).sqrMagnitude < 1.5f)
                    {
                        _phase = SwoopPhase.Cooldown;
                        _nextDiveTime = Time.time + _diveCooldown;
                    }
                    break;
            }
        }

        private void BeginDive(Transform head)
        {
            _phase = SwoopPhase.Dive;
            _diveTarget = head.position;
            _bitThisDive = false;
        }

        private void BeginRecover()
        {
            _phase = SwoopPhase.Recover;
            Vector3 flat = _velocity;
            flat.y = 0f;
            flat = flat.sqrMagnitude > 0.01f ? flat.normalized : transform.forward;
            // Carry the swoop momentum forward, then climb back to attack altitude.
            _recoverTarget = transform.position + flat * 4f;
            _recoverTarget.y = _diveTarget.y + _hoverHeight;
        }

        private void TryBite(Transform head)
        {
            if (_bitThisDive) return;
            if ((head.position - transform.position).sqrMagnitude > _contactRadius * _contactRadius) return;

            _bitThisDive = true;
            TryDamagePlayer(_contactDamage, DamageType.Kinetic, head.position, _velocity.normalized, gameObject);
        }

        private void HoverAround(Vector3 groundCenter)
        {
            _orbitAngle += _orbitDegreesPerSecond * Time.deltaTime;
            float rad = _orbitAngle * Mathf.Deg2Rad;

            Vector3 target = groundCenter;
            target.x += Mathf.Cos(rad) * _orbitRadius;
            target.z += Mathf.Sin(rad) * _orbitRadius;
            target.y += _hoverHeight + Mathf.Sin(Time.time * _bobFrequency * 2f * Mathf.PI) * _bobAmplitude;

            Vector3 desired = Vector3.ClampMagnitude((target - transform.position) * 1.5f, _hoverSpeed);
            IntegrateSteering(desired, ref _velocity, _turnResponsiveness);
        }
    }
}
