using UnityEngine;
using Pridefall.Core;
using Pridefall.Interaction;

namespace Pridefall.Weapons
{
    /// <summary>
    /// The Compact-issue energy pistol, Aris's chapter 2 starter weapon.
    /// Semi-auto, one-handed, cell-fed. Cells are lore-infinite: the suit
    /// fabricates a fresh one at the hip holster whenever a spent cell is
    /// ejected, but the player still performs the physical eject / grab /
    /// insert reload loop.
    /// </summary>
    public class CompactSidearm : WeaponBase
    {
        [Header("Sidearm")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private GameObject _impactPrefab;
        [SerializeField] private float _projectileSpeed = 40f;
        [SerializeField] private float _damagePerShot = 12f;
        [SerializeField] private float _spreadDegrees = 0.5f;

        [Header("Hip Cell Supply")]
        [Tooltip("Fresh cells materialize here on eject. Assign the hip HolsterSlot under BodyRoot.")]
        [SerializeField] private HolsterSlot _hipSlot;
        [SerializeField] private EnergyCell _cellPrefab;

        public override HolsterItemType HolsterType => HolsterItemType.Sidearm;
        protected override bool IsAutomatic => false;

        private EnergyCell _hipCell;

        protected override void Start()
        {
            base.Start();
            RegenerateCellAtHip(); // a reload is always waiting from the first draw
        }

        protected override void Fire()
        {
            Vector3 direction = ApplySpread(Muzzle.forward, _spreadDegrees);
            SpawnProjectile(_projectilePrefab, direction, _projectileSpeed, _damagePerShot, DamageType.Energy, _impactPrefab);
        }

        public override void Eject()
        {
            base.Eject();
            RegenerateCellAtHip();
        }

        /// <summary>Fabricates a free sidearm cell parked at the hip slot. Safe to call repeatedly.</summary>
        public void RegenerateCellAtHip()
        {
            if (_cellPrefab == null || _hipSlot == null) return;
            if (_hipCell != null && _hipCell.transform.parent == _hipSlot.transform) return; // last freebie untouched

            _hipCell = Instantiate(_cellPrefab, _hipSlot.transform.position, _hipSlot.transform.rotation);
            _hipCell.HoldAtAnchor(_hipSlot.transform);
        }
    }
}
