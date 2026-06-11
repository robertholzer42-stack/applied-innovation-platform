using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Enemies
{
    /// <summary>
    /// Locational damage for enemies: sits on a child hitbox collider and
    /// forwards hits to the owning EnemyBase with a multiplier. A Shardback
    /// belly is a 3x relay, its top armor 0.25x, a Warden shield emitter 2x.
    /// Weapons only ever talk to IDamageable; this is the whole model.
    /// </summary>
    public class DamageRelay : MonoBehaviour, IDamageable
    {
        [Tooltip("Weak points 2-3x, armor plate 0.25x.")]
        [SerializeField] private float _damageMultiplier = 1f;
        [Tooltip("Auto-resolved from parents when left empty.")]
        [SerializeField] private EnemyBase _owner;

        public bool IsAlive => _owner != null && _owner.IsAlive;
        public float DamageMultiplier => _damageMultiplier;

        private void Awake()
        {
            if (_owner == null) _owner = GetComponentInParent<EnemyBase>();
            if (_owner == null) Debug.LogError($"[DamageRelay] No EnemyBase above '{name}'; hits here will be lost.", this);
        }

        public void TakeDamage(in DamageInfo info)
        {
            if (_owner == null) return;
            var scaled = new DamageInfo(info.Amount * _damageMultiplier, info.Type, info.Point, info.Direction, info.Source);
            _owner.TakeDamage(in scaled);
        }
    }
}
