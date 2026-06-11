using UnityEngine;
using Pridefall.Core;
using Pridefall.Interaction;

namespace Pridefall.Weapons
{
    /// <summary>
    /// Harvested bio-weapon that lobs corrosive spikes on a gravity arc, the
    /// counter to Warden shields. It has no cell port: the magazine is grown
    /// from bio-resin harvested off Khepri's fauna, spent through
    /// GameManager.SpendBioResin on reload. Dry-firing attempts a reload
    /// automatically so the weapon never feels jammed mid-fight.
    /// </summary>
    public class SpikeThrower : WeaponBase
    {
        [Header("Spikes")]
        [Tooltip("Prefab should have gravity enabled on its Projectile for the lobbed arc.")]
        [SerializeField] private Projectile _spikePrefab;
        [SerializeField] private GameObject _impactPrefab;
        [SerializeField] private float _launchSpeed = 16f;
        [SerializeField] private float _damagePerSpike = 25f;
        [Tooltip("Upward tilt added to the muzzle direction so spikes lob rather than dart.")]
        [SerializeField] private float _lobAngleDegrees = 12f;

        [Header("Bio-Resin Magazine")]
        [SerializeField] private int _magazineSize = 6;
        [SerializeField] private int _resinPerReload = 3;

        public int SpikesLoaded { get; private set; }

        public override HolsterItemType HolsterType => HolsterItemType.Thrower;
        protected override bool IsAutomatic => false;
        protected override bool HasAmmo => SpikesLoaded > 0;

        private float _lastToastTime = -10f;

        protected override void Awake()
        {
            base.Awake();
            SpikesLoaded = _magazineSize; // found grown and ready
        }

        protected override void ConsumeAmmo()
        {
            if (SpikesLoaded > 0) SpikesLoaded--;
        }

        protected override void Fire()
        {
            Vector3 direction = Quaternion.AngleAxis(-_lobAngleDegrees, Muzzle.right) * Muzzle.forward;
            SpawnProjectile(_spikePrefab, direction, _launchSpeed, _damagePerSpike, DamageType.Corrosive, _impactPrefab);
        }

        protected override void OnDryFire()
        {
            TryReload();
        }

        /// <summary>Grows a fresh spike magazine from carried bio-resin.</summary>
        public bool TryReload()
        {
            if (SpikesLoaded >= _magazineSize) return false;

            if (GameManager.Instance == null || !GameManager.Instance.SpendBioResin(_resinPerReload))
            {
                if (Time.time - _lastToastTime > 1.5f) // repeated dry-fire must not spam the HUD
                {
                    _lastToastTime = Time.time;
                    GameEvents.RaiseObjectiveUpdated("Spike Thrower: insufficient bio-resin");
                }
                return false;
            }

            SpikesLoaded = _magazineSize;
            return true;
        }

        public override bool TryInsertCell(EnergyCell cell) => false; // organic weapon, no cell port
    }
}
