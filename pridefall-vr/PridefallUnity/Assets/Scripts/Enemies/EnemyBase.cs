using UnityEngine;
using UnityEngine.AI;
using Pridefall.Core;
using Pridefall.Player;

namespace Pridefall.Enemies
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Investigate,
        Attack,
        Stagger,
        Dead
    }

    /// <summary>
    /// Base for every Khepri hostile: health, the shared state machine,
    /// stagger on heavy single hits, and pooled death/despawn. Subclasses
    /// override EnterState and the per-state tick methods; the base owns the
    /// transition bookkeeping. Implements IDamageable on the enemy root, so
    /// locational hitboxes (DamageRelay) forward scaled damage here.
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Vitals")]
        [SerializeField] protected float _maxHealth = 60f;
        [Tooltip("Scrap awarded on kill. Machine enemies pay well; native fauna drop little or none.")]
        [SerializeField] protected int _scrapDrop = 5;
        [Tooltip("A single hit dealing more than this interrupts the enemy into Stagger.")]
        [SerializeField] protected float _staggerThreshold = 25f;
        [SerializeField] protected float _staggerDuration = 1.2f;
        [Tooltip("Seconds the corpse lingers before returning to the pool.")]
        [SerializeField] protected float _despawnDelay = 4f;

        public bool IsAlive => CurrentState != EnemyState.Dead;
        public EnemyState CurrentState { get; private set; } = EnemyState.Idle;
        public float Health { get; private set; }
        public float HealthNormalized => _maxHealth > 0f ? Health / _maxHealth : 0f;

        /// <summary>Null on fliers and swimmers; those integrate movement manually.</summary>
        protected NavMeshAgent Agent { get; private set; }

        private Collider[] _colliders;
        private bool _cached;
        private float _staggerEndTime;
        private EnemyState _stateBeforeStagger = EnemyState.Idle;

        private static PlayerRig _player;
        private static PlayerHealth _playerHealth;

        /// <summary>FindObjectOfType is paid once per scene, then shared by every enemy.</summary>
        protected static PlayerRig Player
        {
            get
            {
                if (_player == null)
                {
                    _player = FindObjectOfType<PlayerRig>();
                    _playerHealth = _player != null ? _player.GetComponent<PlayerHealth>() : null;
                }
                return _player;
            }
        }

        /// <summary>Aim point. Navigation should use PlayerPosition (rig root) instead.</summary>
        protected static Transform PlayerHead => Player != null ? Player.Head : null;
        protected static Vector3 PlayerPosition => Player != null ? Player.transform.position : Vector3.zero;
        protected static bool PlayerIsAlive => Player != null && _playerHealth != null && _playerHealth.IsAlive;

        protected virtual void Awake()
        {
            EnsureCached();
        }

        protected virtual void OnEnable()
        {
            EnsureCached();
            Health = _maxHealth;
            SetCollidersEnabled(true);
            if (Agent != null) Agent.enabled = true;
            ForceState(EnemyState.Idle); // pooled respawn: always restart clean
        }

        private void EnsureCached()
        {
            if (_cached) return;
            _cached = true;
            Agent = GetComponent<NavMeshAgent>();
            _colliders = GetComponentsInChildren<Collider>(true);
        }

        protected virtual void Update()
        {
            switch (CurrentState)
            {
                case EnemyState.Idle: TickIdle(); break;
                case EnemyState.Patrol: TickPatrol(); break;
                case EnemyState.Investigate: TickInvestigate(); break;
                case EnemyState.Attack: TickAttack(); break;
                case EnemyState.Stagger: TickStagger(); break;
            }
        }

        protected void ChangeState(EnemyState next)
        {
            if (CurrentState == next) return;
            ForceState(next);
        }

        private void ForceState(EnemyState next)
        {
            CurrentState = next;
            EnterState(next);
        }

        /// <summary>Called once on every transition, including re-entry after Stagger.</summary>
        protected virtual void EnterState(EnemyState state)
        {
            if (state == EnemyState.Stagger)
            {
                _staggerEndTime = Time.time + _staggerDuration;
                if (Agent != null && Agent.enabled && Agent.isOnNavMesh) Agent.isStopped = true;
            }
        }

        protected virtual void TickIdle() { }
        protected virtual void TickPatrol() { }
        protected virtual void TickInvestigate() { }
        protected virtual void TickAttack() { }

        protected virtual void TickStagger()
        {
            if (Time.time < _staggerEndTime) return;
            if (Agent != null && Agent.enabled && Agent.isOnNavMesh) Agent.isStopped = false;
            ChangeState(_stateBeforeStagger == EnemyState.Dead ? EnemyState.Idle : _stateBeforeStagger);
        }

        /// <summary>Enter Stagger for a custom duration (charge self-stun, wall slam).</summary>
        protected void Stagger(float duration)
        {
            if (!IsAlive) return;
            if (CurrentState != EnemyState.Stagger) _stateBeforeStagger = CurrentState;
            ForceState(EnemyState.Stagger);
            _staggerEndTime = Time.time + duration;
        }

        public virtual void TakeDamage(in DamageInfo info)
        {
            if (!IsAlive) return;

            Health -= info.Amount;
            if (Health <= 0f)
            {
                Die();
                return;
            }

            if (info.Amount > _staggerThreshold) Stagger(_staggerDuration);
        }

        protected virtual void Die()
        {
            Health = 0f;
            ForceState(EnemyState.Dead);
            SetCollidersEnabled(false);
            if (Agent != null) Agent.enabled = false;

            GameEvents.RaiseEnemyKilled(gameObject, _scrapDrop);

            // Pool falls back to Destroy for instances it does not own.
            if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject, _despawnDelay);
            else Destroy(gameObject, _despawnDelay);
        }

        protected void SetCollidersEnabled(bool enabled)
        {
            if (_colliders == null) return;
            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null) _colliders[i].enabled = enabled;
            }
        }

        /// <summary>Apply damage to the player's health, if the player exists and is alive.</summary>
        protected static bool TryDamagePlayer(float amount, DamageType type, Vector3 point, Vector3 direction, GameObject source)
        {
            if (!PlayerIsAlive) return false;
            _playerHealth.TakeDamage(new DamageInfo(amount, type, point, direction, source));
            return true;
        }

        /// <summary>
        /// Manual movement for enemies without a NavMeshAgent: exponentially
        /// blends velocity toward the desired vector (framerate independent)
        /// and faces the direction of travel.
        /// </summary>
        protected void IntegrateSteering(Vector3 desiredVelocity, ref Vector3 velocity, float responsiveness)
        {
            float dt = Time.deltaTime;
            velocity = Vector3.Lerp(velocity, desiredVelocity, 1f - Mathf.Exp(-responsiveness * dt));
            transform.position += velocity * dt;
            if (velocity.sqrMagnitude > 0.04f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity.normalized), 6f * dt);
            }
        }
    }
}
