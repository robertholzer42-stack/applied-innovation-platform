using UnityEngine;
using Pridefall.Interaction;

namespace Pridefall.Weapons
{
    /// <summary>
    /// Fabricated energy magazine for the sidearm and carbine. A physical
    /// Grabbable: the player pulls it from the chest holster, hip, or a
    /// fabricator tray and pushes it into a weapon's cell port to reload.
    /// Tracks remaining charge; weapons drain it one shot at a time.
    /// </summary>
    public class EnergyCell : Grabbable, IHolsterable
    {
        [SerializeField] private int _capacity = 12;

        public int Capacity => _capacity;
        public int ShotsRemaining { get; private set; }
        public bool IsEmpty => ShotsRemaining <= 0;

        /// <summary>True while seated in a weapon's cell port.</summary>
        public bool IsInserted { get; private set; }

        public HolsterItemType HolsterType => HolsterItemType.Consumable;
        public override bool CanBeGrabbed => base.CanBeGrabbed && !IsInserted;

        private Collider[] _colliders;

        protected override void Awake()
        {
            base.Awake();
            ShotsRemaining = _capacity;
            _colliders = GetComponentsInChildren<Collider>(true);
        }

        public void Consume()
        {
            if (ShotsRemaining > 0) ShotsRemaining--;
        }

        public void Refill() => ShotsRemaining = _capacity;

        /// <summary>Called by WeaponBase when this cell is seated in a port.</summary>
        public void OnInserted(Transform port)
        {
            IsInserted = true;
            Body.isKinematic = true;
            transform.SetParent(port);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            SetCollidersEnabled(false); // seated cells must not block the wielder's hands or re-trip the port
        }

        /// <summary>Called by WeaponBase.Eject(); the cell becomes a free physics object again.</summary>
        public void OnEjected(Vector3 velocity)
        {
            IsInserted = false;
            transform.SetParent(null);
            SetCollidersEnabled(true);
            Body.isKinematic = false;
            Body.velocity = velocity;
        }

        /// <summary>Parks the cell kinematically at an anchor (hip fabrication) until a hand takes it.</summary>
        public void HoldAtAnchor(Transform anchor)
        {
            if (anchor == null) return;

            Body.isKinematic = true;
            transform.SetParent(anchor);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void SetCollidersEnabled(bool value)
        {
            if (_colliders == null) return;
            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null) _colliders[i].enabled = value;
            }
        }
    }
}
