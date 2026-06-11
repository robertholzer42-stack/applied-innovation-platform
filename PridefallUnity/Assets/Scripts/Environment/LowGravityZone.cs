using UnityEngine;
using Pridefall.Player;

namespace Pridefall.Environment
{
    /// <summary>
    /// Low-g anomaly around a mass-driver scar. Scales the player's gravity
    /// while inside, so a sprint-jump becomes a long jump. Overlapping zones
    /// are ref-counted: gravity restores to 1 only when the last zone is
    /// exited. The scale itself is last-writer-wins (the most recently
    /// entered zone sets it), acceptable because scars cluster at similar
    /// scales; do not nest zones with wildly different values.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class LowGravityZone : MonoBehaviour
    {
        [Tooltip("0.35 reads as dramatic but controllable; jump impulse already compensates by 1/sqrt(scale).")]
        [SerializeField, Range(0.05f, 1f)] private float _gravityScale = 0.35f;

        // Single-player game: one shared count across all zone instances.
        private static int _zonesContainingPlayer;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var locomotion = LocomotionFrom(other);
            if (locomotion == null) return;

            _zonesContainingPlayer++;
            locomotion.GravityScale = _gravityScale;
        }

        private void OnTriggerExit(Collider other)
        {
            var locomotion = LocomotionFrom(other);
            if (locomotion == null) return;

            _zonesContainingPlayer = Mathf.Max(0, _zonesContainingPlayer - 1);
            if (_zonesContainingPlayer == 0)
            {
                locomotion.GravityScale = 1f;
            }
        }

        private static PlayerLocomotionController LocomotionFrom(Collider other)
        {
            // Only the capsule counts; hand colliders waving across the
            // boundary must not corrupt the ref-count.
            if (other is not CharacterController) return null;
            return other.GetComponent<PlayerLocomotionController>();
        }
    }
}
