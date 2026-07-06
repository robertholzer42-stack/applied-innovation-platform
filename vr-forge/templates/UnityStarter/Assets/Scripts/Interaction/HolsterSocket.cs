using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRForge.Interaction
{
    /// <summary>
    /// XRSocketInteractor that only accepts GrabbableItems whose socket tag
    /// matches this socket's accepted tag (case-insensitive). Use for hip
    /// holsters, back slots, and tool belts; anything without a matching
    /// GrabbableItem is refused before hover, so no false snap previews.
    /// </summary>
    public class HolsterSocket : XRSocketInteractor
    {
        [SerializeField]
        [Tooltip("Only GrabbableItems with this socket tag may hover/attach here.")]
        private string acceptedTag = "generic";

        /// <summary>Tag this socket accepts.</summary>
        public string AcceptedTag => acceptedTag;

        /// <inheritdoc />
        public override bool CanHover(IXRHoverInteractable interactable)
        {
            return base.CanHover(interactable) && Accepts(interactable != null ? interactable.transform : null);
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && Accepts(interactable != null ? interactable.transform : null);
        }

        private bool Accepts(Transform candidate)
        {
            if (candidate == null) return false;

            var item = candidate.GetComponent<GrabbableItem>();
            return item != null && item.MatchesSocketTag(acceptedTag);
        }
    }
}
