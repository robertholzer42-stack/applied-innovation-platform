using UnityEngine;
using Pridefall.Core;
using Pridefall.Interaction;
using Pridefall.Player;

namespace Pridefall.Weapons
{
    /// <summary>
    /// Single-use medgel canister. Health segments lost in combat only return
    /// when the player physically presses a held medgel against the off-hand
    /// wrist port while squeezing the trigger; on success it restores one
    /// segment via PlayerHealth.ApplyMedgel and the canister is consumed.
    /// Stows in the chest holster alongside spare cells.
    /// </summary>
    public class MedgelApplicator : Grabbable, IHolsterable
    {
        [Tooltip("Explicit wrist port anchor. If unset, the hand opposite the holder is used at runtime.")]
        [SerializeField] private Transform _wristPort;
        [SerializeField] private float _applyRadius = 0.1f;

        public HolsterItemType HolsterType => HolsterItemType.Consumable;

        private PlayerRig _rig;
        private PlayerHealth _health;
        private bool _consumed;

        private void Update()
        {
            if (_consumed || !IsHeld || Holder == null || !Holder.TriggerHeld) return;

            Transform wrist = ResolveWristPort();
            if (wrist == null) return;
            if ((transform.position - wrist.position).sqrMagnitude > _applyRadius * _applyRadius) return;

            var health = ResolveHealth(wrist);
            if (health == null || !health.ApplyMedgel()) return; // full health: keep the canister

            _consumed = true;
            Holder.ForceRelease();
            if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject); // falls back to Destroy for unpooled instances
            else Destroy(gameObject);
        }

        private Transform ResolveWristPort()
        {
            if (_wristPort != null) return _wristPort;

            if (_rig == null) _rig = FindObjectOfType<PlayerRig>();
            if (_rig == null) return null;

            var offHand = Holder.Hand == Handedness.Left ? _rig.RightHand : _rig.LeftHand;
            return offHand != null ? offHand.transform : null;
        }

        private PlayerHealth ResolveHealth(Transform wrist)
        {
            if (_health == null) _health = wrist.GetComponentInParent<PlayerHealth>();
            if (_health == null) _health = FindObjectOfType<PlayerHealth>();
            return _health;
        }
    }
}
