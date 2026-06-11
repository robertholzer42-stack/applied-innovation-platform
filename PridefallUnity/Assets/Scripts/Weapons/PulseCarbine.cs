using UnityEngine;
using Pridefall.Core;
using Pridefall.Interaction;
using Pridefall.Player;

namespace Pridefall.Weapons
{
    /// <summary>
    /// Full-auto energy carbine, the mid-game workhorse against Wardens. Two
    /// handed by design: when the off hand grips near the foregrip the cone
    /// of fire tightens and recoil halves, rewarding a true two-handed stance
    /// without requiring it. Eats fabricated cells; no free hip supply.
    /// </summary>
    public class PulseCarbine : WeaponBase
    {
        [Header("Carbine")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private GameObject _impactPrefab;
        [SerializeField] private float _projectileSpeed = 55f;
        [SerializeField] private float _damagePerShot = 8f;

        [Header("Spread")]
        [SerializeField] private float _hipSpreadDegrees = 3.5f;
        [SerializeField] private float _stabilizedSpreadDegrees = 1f;

        [Header("Foregrip")]
        [Tooltip("Off-hand stabilization point (child transform under the barrel). A second gripping, empty hand within radius stabilizes the weapon.")]
        [SerializeField] private Transform _foregrip;
        [SerializeField] private float _foregripRadius = 0.15f;

        public override HolsterItemType HolsterType => HolsterItemType.Carbine;
        protected override bool IsAutomatic => true;
        protected override float RecoilMultiplier => _stabilizedThisShot ? 0.5f : 1f;

        private HandController[] _hands;
        private bool _stabilizedThisShot;

        public override void OnGrabbed(HandController hand)
        {
            base.OnGrabbed(hand);
            if (_hands == null || _hands.Length == 0)
            {
                _hands = FindObjectsOfType<HandController>(); // one-time lookup; the rig's hands never change at runtime
            }
        }

        protected override void Fire()
        {
            _stabilizedThisShot = IsForegripHeld(); // cached so RecoilMultiplier sees the same answer this shot
            float spread = _stabilizedThisShot ? _stabilizedSpreadDegrees : _hipSpreadDegrees;
            Vector3 direction = ApplySpread(Muzzle.forward, spread);
            SpawnProjectile(_projectilePrefab, direction, _projectileSpeed, _damagePerShot, DamageType.Energy, _impactPrefab);
        }

        private bool IsForegripHeld()
        {
            if (_foregrip == null || _hands == null) return false;

            float radiusSqr = _foregripRadius * _foregripRadius;
            for (int i = 0; i < _hands.Length; i++)
            {
                var hand = _hands[i];
                if (hand == null || hand == Holder) continue;
                if (!hand.GripHeld || hand.IsClimbing || hand.HeldGrabbable != null) continue;
                if ((hand.transform.position - _foregrip.position).sqrMagnitude <= radiusSqr) return true;
            }
            return false;
        }
    }
}
