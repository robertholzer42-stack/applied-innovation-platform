using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Interaction
{
    /// <summary>Body slots a holsterable item can occupy.</summary>
    public enum HolsterItemType
    {
        Sidearm,
        Carbine,
        Thrower,
        Consumable
    }

    /// <summary>
    /// Declares which HolsterSlot type an item stows into. Implemented by
    /// WeaponBase and by pocketables (cells, medgel).
    /// </summary>
    public interface IHolsterable
    {
        HolsterItemType HolsterType { get; }
    }

    /// <summary>
    /// Body-anchored stow point, parented under PlayerRig.BodyRoot so it
    /// follows the Omni ring yaw: hip (sidearm), shoulder (carbine), chest
    /// (cells and medgel). A matching Grabbable released inside the trigger
    /// volume snaps in and rides the body; grabbing it pulls it back out.
    /// Requires a trigger collider (e.g. SphereCollider) on this object.
    /// </summary>
    public class HolsterSlot : MonoBehaviour
    {
        [SerializeField] private HolsterItemType _accepts = HolsterItemType.Sidearm;
        [Tooltip("Commit a checkpoint when an item is stowed. Treadmill sessions end when legs end; holstering is a natural pause point.")]
        [SerializeField] private bool _checkpointOnStow = true;

        public HolsterItemType Accepts => _accepts;
        public Grabbable Stowed { get; private set; }
        public bool IsOccupied => Stowed != null;

        public bool CanAccept(Grabbable item)
        {
            return !IsOccupied && item != null && item is IHolsterable holsterable && holsterable.HolsterType == _accepts;
        }

        private void Update()
        {
            // The hand re-parents the item on grab (Grabbable.OnGrabbed); the slot just lets go.
            if (Stowed != null && Stowed.IsHeld)
            {
                Stowed = null;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsOccupied) return;

            var item = other.GetComponentInParent<Grabbable>();
            if (item == null || item.IsHeld || !CanAccept(item)) return;

            Stow(item);
        }

        private void Stow(Grabbable item)
        {
            Stowed = item;

            var body = item.GetComponent<Rigidbody>();
            if (body != null) body.isKinematic = true;

            item.transform.SetParent(transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            if (_checkpointOnStow && GameManager.Instance != null)
            {
                Transform playerRoot = transform.root; // slot lives under BodyRoot under PlayerRig
                GameManager.Instance.CommitCheckpoint(playerRoot.position, playerRoot.rotation, "holster");
            }
        }
    }
}
