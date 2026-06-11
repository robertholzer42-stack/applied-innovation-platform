using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Enemies
{
    /// <summary>
    /// Pooled enemy bolt. Travels in a straight line, sweeping a spherecast
    /// over each step so high speeds cannot tunnel through thin geometry.
    /// Damage tuning lives here on the prefab; the shooter only supplies
    /// velocity via Launch. The Rigidbody is kept kinematic and exists so
    /// other physics systems can query the bolt's motion.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float _damage = 10f;
        [SerializeField] private DamageType _damageType = DamageType.Energy;
        [SerializeField] private float _radius = 0.08f;
        [Tooltip("Layers the bolt flies through (the firing faction, other enemies). Everything else blocks and takes damage.")]
        [SerializeField] private LayerMask _friendlyMask;
        [Tooltip("Seconds before an airborne bolt despawns itself.")]
        [SerializeField] private float _maxLifetime = 5f;
        [SerializeField] private GameObject _impactEffectPrefab;
        [SerializeField] private float _impactEffectLifetime = 1.5f;

        private Rigidbody _rigidbody;
        private Vector3 _velocity;
        private GameObject _source;
        private float _despawnTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true; // moved manually; the sweep handles contact
            _rigidbody.useGravity = false;
        }

        private void OnEnable()
        {
            // Safety: a pooled bolt that never gets Launch() still expires.
            _despawnTime = Time.time + _maxLifetime;
            _velocity = Vector3.zero;
            _source = null;
        }

        /// <summary>Arm and fire. Velocity is world-space; source is skipped by the hit sweep.</summary>
        public void Launch(Vector3 velocity, GameObject source)
        {
            _velocity = velocity;
            _source = source;
            _despawnTime = Time.time + _maxLifetime;
            if (velocity.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(velocity);
        }

        private void FixedUpdate()
        {
            if (Time.time >= _despawnTime)
            {
                Despawn();
                return;
            }
            if (_velocity.sqrMagnitude < 0.001f) return;

            float step = _velocity.magnitude * Time.fixedDeltaTime;
            Vector3 origin = transform.position;

            if (Physics.SphereCast(origin, _radius, _velocity.normalized, out RaycastHit hit, step, ~_friendlyMask, QueryTriggerInteraction.Ignore)
                && (_source == null || !hit.collider.transform.IsChildOf(_source.transform)))
            {
                OnImpact(in hit);
                return;
            }

            _rigidbody.MovePosition(origin + _velocity * Time.fixedDeltaTime);
        }

        private void OnImpact(in RaycastHit hit)
        {
            // Searches self first, so a DamageRelay hitbox wins over its root.
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(new DamageInfo(_damage, _damageType, hit.point, _velocity.normalized, _source));
            }

            if (_impactEffectPrefab != null && ObjectPool.Instance != null)
            {
                Vector3 facing = hit.normal.sqrMagnitude > 0.001f ? hit.normal : -_velocity.normalized;
                GameObject fx = ObjectPool.Instance.Spawn(_impactEffectPrefab, hit.point, Quaternion.LookRotation(facing));
                ObjectPool.Instance.Despawn(fx, _impactEffectLifetime);
            }

            Despawn();
        }

        private void Despawn()
        {
            _velocity = Vector3.zero;
            if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject);
            else Destroy(gameObject);
        }
    }
}
