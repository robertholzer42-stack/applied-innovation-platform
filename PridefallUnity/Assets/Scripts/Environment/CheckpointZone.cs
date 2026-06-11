using UnityEngine;
using Pridefall.Core;
using Pridefall.Player;

namespace Pridefall.Environment
{
    /// <summary>
    /// One-shot checkpoint trigger. The first time the player walks through,
    /// commits a checkpoint at the respawn anchor, not at the trigger, so
    /// respawns land on safe, flat ground facing the intended direction.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CheckpointZone : MonoBehaviour
    {
        [SerializeField] private string _checkpointId = "checkpoint";
        [Tooltip("Respawn pose. Falls back to this transform if unset. Only yaw is kept; the capsule never spawns tilted.")]
        [SerializeField] private Transform _respawnAnchor;

        private bool _committed;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_committed) return;
            if (other.GetComponentInParent<PlayerLocomotionController>() == null) return;

            if (GameManager.Instance == null)
            {
                Debug.LogWarning($"[CheckpointZone] No GameManager in scene; checkpoint '{_checkpointId}' not committed.", this);
                return;
            }

            Transform anchor = _respawnAnchor != null ? _respawnAnchor : transform;
            Quaternion yawOnly = Quaternion.Euler(0f, anchor.eulerAngles.y, 0f);
            GameManager.Instance.CommitCheckpoint(anchor.position, yawOnly, _checkpointId);
            _committed = true;
        }
    }
}
