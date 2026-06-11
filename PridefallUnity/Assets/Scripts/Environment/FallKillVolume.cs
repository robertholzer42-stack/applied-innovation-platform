using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Environment
{
    /// <summary>
    /// Instant-death trigger lining chasm bottoms and the Hush's cryo pits.
    /// Deals far more than any health pool in one hit, so anything damageable
    /// that falls in dies: the player respawns through the normal GameManager
    /// checkpoint loop, enemies just die.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FallKillVolume : MonoBehaviour
    {
        [Tooltip("Must exceed any conceivable health pool.")]
        [SerializeField] private float _damage = 10000f;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null || !damageable.IsAlive) return;

            damageable.TakeDamage(new DamageInfo(_damage, DamageType.Kinetic,
                other.transform.position, Vector3.down, gameObject));
        }
    }
}
