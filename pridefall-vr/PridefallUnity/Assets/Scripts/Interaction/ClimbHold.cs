using UnityEngine;

namespace Pridefall.Interaction
{
    /// <summary>
    /// A grabbable climbing surface point: luminous root nodes, pitons,
    /// ledges. Static geometry; the ClimbingSystem moves the player body
    /// opposite to hand motion while one or both hands grip these.
    /// </summary>
    public class ClimbHold : MonoBehaviour
    {
        [Tooltip("Overhang holds drain stamina while gripped.")]
        public bool IsOverhang;

        [Tooltip("Crumbling holds release after this many seconds of grip. 0 = solid.")]
        public float CrumbleSeconds;

        [Tooltip("Checkpoint id committed when grabbed, empty = none. Set on 'rest ledge' holds.")]
        public string CheckpointId = string.Empty;
    }
}
