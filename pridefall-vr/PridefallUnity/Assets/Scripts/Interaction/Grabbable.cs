using UnityEngine;

namespace Pridefall.Interaction
{
    /// <summary>
    /// Base class for anything a hand can pick up: weapons, cells, medgel,
    /// salvage. Kinematic while held; rethrown with hand velocity on release.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Grabbable : MonoBehaviour
    {
        [Tooltip("Optional alignment point; the hand snaps here (e.g. a pistol grip).")]
        [SerializeField] private Transform _gripAnchor;
        [SerializeField] private float _throwVelocityScale = 1.1f;

        public bool IsHeld => Holder != null;
        public virtual bool CanBeGrabbed => !IsHeld;
        public Player.HandController Holder { get; private set; }

        protected Rigidbody Body { get; private set; }

        protected virtual void Awake()
        {
            Body = GetComponent<Rigidbody>();
        }

        public virtual void OnGrabbed(Player.HandController hand)
        {
            Holder = hand;
            Body.isKinematic = true;

            transform.SetParent(hand.transform);
            if (_gripAnchor != null)
            {
                // Align so the grip anchor sits in the palm.
                transform.localRotation = Quaternion.Inverse(_gripAnchor.localRotation);
                transform.localPosition = -(transform.localRotation * _gripAnchor.localPosition);
            }
            else
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }

        public virtual void OnReleased(Player.HandController hand, Vector3 handVelocity)
        {
            Holder = null;
            transform.SetParent(null);
            Body.isKinematic = false;
            Body.velocity = handVelocity * _throwVelocityScale;
        }
    }
}
