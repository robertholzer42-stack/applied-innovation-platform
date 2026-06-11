using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Weapons
{
    /// <summary>
    /// Pooled physical projectile; PRIDEFALL has no hitscan, every shot has
    /// travel time. Moves by transform with a spherecast swept over each
    /// FixedUpdate step so fast bolts cannot tunnel through a Skimmer or a
    /// canyon wall at 72 Hz. Optional gravity supports the Spike Thrower's
    /// lobbed arcs. On hit it applies DamageInfo to the nearest IDamageable
    /// parent, spawns a pooled impact effect, and returns itself to the pool.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _radius = 0.03f;
        [SerializeField] private float _maxLifetime = 5f;
        [Tooltip("Enable for lobbed projectiles (Spike Thrower). Energy bolts fly straight.")]
        [SerializeField] private bool _useGravity = false;
        [Tooltip("Scales Physics.gravity when gravity is enabled; below 1 gives floatier arcs.")]
        [SerializeField] private float _gravityScale = 1f;
        [SerializeField] private LayerMask _hitMask = ~0;
        [SerializeField] private float _impactEffectLifetime = 2f;

        private Vector3 _velocity;
        private float _damage;
        private DamageType _damageType;
        private GameObject _source;
        private GameObject _impactPrefab;
        private float _age;

        /// <summary>Arms a pool-spawned instance. Must be called immediately after ObjectPool.Spawn.</summary>
        public void Initialize(Vector3 velocity, float damage, DamageType type, GameObject source, GameObject impactPrefab)
        {
            _velocity = velocity;
            _damage = damage;
            _damageType = type;
            _source = source;
            _impactPrefab = impactPrefab;
            _age = 0f;

            if (velocity.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(velocity);
            }
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            _age += dt;
            if (_age >= _maxLifetime)
            {
                Despawn();
                return;
            }

            if (_useGravity)
            {
                _velocity += Physics.gravity * (_gravityScale * dt);
            }

            Vector3 step = _velocity * dt;
            float distance = step.magnitude;
            if (distance > Mathf.Epsilon &&
                Physics.SphereCast(transform.position, _radius, step / distance, out RaycastHit hit, distance, _hitMask, QueryTriggerInteraction.Ignore) &&
                !IsSource(hit.collider))
            {
                Impact(hit);
                return;
            }

            transform.position += step;
            if (_useGravity && _velocity.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(_velocity); // keep spikes nose-first along the arc
            }
        }

        private bool IsSource(Collider hitCollider)
        {
            // Shots never hit the rig that fired them (muzzle sits inside the wielder's colliders).
            return _source != null && hitCollider.transform.IsChildOf(_source.transform);
        }

        private void Impact(RaycastHit hit)
        {
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                Vector3 direction = _velocity.sqrMagnitude > 0.0001f ? _velocity.normalized : transform.forward;
                damageable.TakeDamage(new DamageInfo(_damage, _damageType, hit.point, direction, _source));
            }

            if (_impactPrefab != null && ObjectPool.Instance != null)
            {
                Vector3 normal = hit.normal.sqrMagnitude > 0.0001f ? hit.normal : -transform.forward;
                var effect = ObjectPool.Instance.Spawn(_impactPrefab, hit.point, Quaternion.LookRotation(normal));
                ObjectPool.Instance.Despawn(effect, _impactEffectLifetime);
            }

            Despawn();
        }

        private void Despawn()
        {
            if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject);
            else gameObject.SetActive(false); // pool missing (scene teardown); just go dormant
        }
    }
}
