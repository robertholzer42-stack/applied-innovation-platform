using UnityEngine;
using UnityEngine.AI;
using Pridefall.Core;

namespace Pridefall.Enemies
{
    /// <summary>
    /// The Custodian's shielded ranged enforcer. Strafes a preferred range
    /// band on the NavMesh while facing the player (legs and gun decoupled,
    /// like the player's own rig) and fires pooled energy bolts. The shield
    /// absorbs every hit until broken; Corrosive (Spike Thrower) deals double
    /// to it, which is the intended counter. Projectiles are launched through
    /// a local helper so Enemies has no dependency on the Weapons assembly.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyPerception))]
    public class WardenDrone : EnemyBase
    {
        [Header("Shield")]
        [SerializeField] private float _shieldMax = 60f;
        [Tooltip("Seconds without taking any damage before the shield starts regenerating.")]
        [SerializeField] private float _shieldRegenDelay = 5f;
        [SerializeField] private float _shieldRegenPerSecond = 20f;

        [Header("Ranged Attack")]
        [Tooltip("Prefab carrying a Rigidbody + EnemyProjectile. Damage lives on the projectile; speed is set at launch.")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _projectileSpeed = 18f;
        [SerializeField] private float _fireInterval = 1.4f;
        [Tooltip("Degrees of random scatter so the player can dodge by strafing.")]
        [SerializeField] private float _aimErrorDegrees = 2f;

        [Header("Movement")]
        [SerializeField] private float _preferredRangeMin = 8f;
        [SerializeField] private float _preferredRangeMax = 14f;
        [SerializeField] private float _strafeSpeed = 3.5f;
        [Tooltip("Lateral offset of each strafe destination.")]
        [SerializeField] private float _strafeStep = 3f;
        [Tooltip("Min/max seconds between strafe direction flips.")]
        [SerializeField] private Vector2 _strafeFlipInterval = new Vector2(1.5f, 3.5f);
        [Tooltip("How often the strafe destination is recomputed. Deliberately not every frame.")]
        [SerializeField] private float _repathInterval = 0.4f;
        [SerializeField] private float _turnDegreesPerSecond = 360f;
        [Tooltip("Seconds of lost sight tolerated before falling back to Investigate.")]
        [SerializeField] private float _loseTargetGrace = 2f;

        /// <summary>Remaining shield, 0-1, for the reticle/scan UI.</summary>
        public float ShieldNormalized => _shieldMax > 0f ? _shield / _shieldMax : 0f;
        public bool ShieldUp => _shield > 0f;

        private EnemyPerception _perception;
        private float _shield;
        private float _lastDamageTakenTime;
        private float _nextFireTime;
        private float _nextRepathTime;
        private float _nextStrafeFlipTime;
        private float _lastSeenTime;
        private float _strafeSign = 1f;

        protected override void Awake()
        {
            base.Awake();
            _perception = GetComponent<EnemyPerception>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _shield = _shieldMax;
            _lastDamageTakenTime = float.NegativeInfinity;
            if (_perception != null) _perception.OnNoiseHeard += OnNoiseHeard;
        }

        private void OnDisable()
        {
            if (_perception != null) _perception.OnNoiseHeard -= OnNoiseHeard;
        }

        // Editor-time defaults per the GDD creature table.
        private void Reset()
        {
            _maxHealth = 80f;
            _scrapDrop = 12;
            _staggerThreshold = 35f;
        }

        public override void TakeDamage(in DamageInfo info)
        {
            if (!IsAlive) return;
            _lastDamageTakenTime = Time.time;

            if (_shield > 0f)
            {
                float toShield = info.Type == DamageType.Corrosive ? info.Amount * 2f : info.Amount;
                _shield = Mathf.Max(0f, _shield - toShield);
                return; // the shield soaks the entire hit, including the one that breaks it
            }

            base.TakeDamage(in info);
        }

        protected override void Update()
        {
            base.Update();
            if (!IsAlive || _shield >= _shieldMax) return;
            if (Time.time - _lastDamageTakenTime < _shieldRegenDelay) return;
            _shield = Mathf.Min(_shieldMax, _shield + _shieldRegenPerSecond * Time.deltaTime);
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
                    Agent.updateRotation = true;
                    SetAgentStopped(true);
                    break;

                case EnemyState.Investigate:
                    Agent.updateRotation = true;
                    Agent.speed = _strafeSpeed;
                    SetAgentStopped(false);
                    if (_perception != null) SetDestinationSafe(_perception.LastKnownPlayerPosition);
                    break;

                case EnemyState.Attack:
                    Agent.updateRotation = false; // body faces the player; legs strafe independently
                    Agent.speed = _strafeSpeed;
                    SetAgentStopped(false);
                    _strafeSign = Random.value < 0.5f ? -1f : 1f;
                    _nextRepathTime = 0f;
                    _nextStrafeFlipTime = Time.time + Random.Range(_strafeFlipInterval.x, _strafeFlipInterval.y);
                    _nextFireTime = Time.time + 0.6f; // brief acquire delay before the first bolt
                    _lastSeenTime = Time.time;
                    break;
            }
        }

        protected override void TickIdle()
        {
            if (_perception != null && _perception.CanSeePlayer && PlayerIsAlive) ChangeState(EnemyState.Attack);
        }

        protected override void TickInvestigate()
        {
            if (_perception != null && _perception.CanSeePlayer && PlayerIsAlive) { ChangeState(EnemyState.Attack); return; }
            if (Agent == null || !Agent.enabled || !Agent.isOnNavMesh) { ChangeState(EnemyState.Idle); return; }

            if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.2f)
            {
                ChangeState(EnemyState.Idle);
            }
        }

        protected override void TickAttack()
        {
            if (!PlayerIsAlive) { ChangeState(EnemyState.Idle); return; }
            Transform head = PlayerHead;
            if (head == null) { ChangeState(EnemyState.Idle); return; }

            if (_perception != null && _perception.CanSeePlayer) _lastSeenTime = Time.time;
            else if (Time.time - _lastSeenTime > _loseTargetGrace) { ChangeState(EnemyState.Investigate); return; }

            FacePlayer(head.position);

            if (Time.time >= _nextStrafeFlipTime)
            {
                _strafeSign = -_strafeSign;
                _nextStrafeFlipTime = Time.time + Random.Range(_strafeFlipInterval.x, _strafeFlipInterval.y);
            }

            if (Agent != null && Agent.enabled && Agent.isOnNavMesh && Time.time >= _nextRepathTime)
            {
                _nextRepathTime = Time.time + _repathInterval;

                Vector3 away = transform.position - PlayerPosition;
                away.y = 0f;
                float dist = away.magnitude;
                Vector3 radial = dist > 0.1f ? away / dist : -transform.forward;
                float bandDist = Mathf.Clamp(dist, _preferredRangeMin, _preferredRangeMax);
                Vector3 tangent = Vector3.Cross(Vector3.up, radial) * _strafeSign;
                Agent.SetDestination(PlayerPosition + radial * bandDist + tangent * _strafeStep);
            }

            if (_perception != null && _perception.CanSeePlayer && Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + _fireInterval;
                LaunchProjectile(head);
            }
        }

        /// <summary>Local projectile launcher; keeps Enemies decoupled from the Weapons assembly.</summary>
        private void LaunchProjectile(Transform head)
        {
            if (_projectilePrefab == null || head == null) return;

            Transform muzzle = _muzzle != null ? _muzzle : transform;
            Vector3 dir = head.position - muzzle.position;
            dir = dir.sqrMagnitude > 0.001f ? dir.normalized : muzzle.forward;

            if (_aimErrorDegrees > 0f)
            {
                dir = Quaternion.Euler(
                    Random.Range(-_aimErrorDegrees, _aimErrorDegrees),
                    Random.Range(-_aimErrorDegrees, _aimErrorDegrees),
                    0f) * dir;
            }

            Quaternion rotation = Quaternion.LookRotation(dir);
            GameObject bolt = ObjectPool.Instance != null
                ? ObjectPool.Instance.Spawn(_projectilePrefab, muzzle.position, rotation)
                : Instantiate(_projectilePrefab, muzzle.position, rotation);
            if (bolt == null) return;

            if (bolt.TryGetComponent(out EnemyProjectile projectile))
            {
                projectile.Launch(dir * _projectileSpeed, gameObject);
            }
            else if (bolt.TryGetComponent(out Rigidbody body))
            {
                body.velocity = dir * _projectileSpeed; // bare physics prefab fallback
            }
        }

        private void FacePlayer(Vector3 worldPoint)
        {
            Vector3 to = worldPoint - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude < 0.01f) return;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(to), _turnDegreesPerSecond * Time.deltaTime);
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
