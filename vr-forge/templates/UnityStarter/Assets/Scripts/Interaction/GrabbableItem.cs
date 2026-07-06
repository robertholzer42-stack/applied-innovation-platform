using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRForge.Interaction
{
    /// <summary>
    /// Thin wrapper over XRGrabInteractable that adds a holster/socket type
    /// tag plus grab/release C# events. HolsterSocket filters on the tag so
    /// a pistol snaps to a hip holster but not to the backpack slot. Keep
    /// grab tuning (attach transforms, throw velocity) on the interactable
    /// itself; this component only carries identity and events.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabbableItem : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Socket type this item fits, e.g. 'pistol', 'tool', 'generic'. Matched case-insensitively by HolsterSocket.")]
        private string socketTag = "generic";

        /// <summary>Socket type tag this item fits.</summary>
        public string SocketTag => socketTag;

        /// <summary>The wrapped interactable, cached in Awake.</summary>
        public XRGrabInteractable Interactable { get; private set; }

        /// <summary>Raised when any interactor (hand or socket) selects this item.</summary>
        public event Action<GrabbableItem> Grabbed;

        /// <summary>Raised when the selecting interactor releases this item.</summary>
        public event Action<GrabbableItem> Released;

        private void Awake()
        {
            Interactable = GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            if (Interactable == null) return;

            Interactable.selectEntered.AddListener(HandleSelectEntered);
            Interactable.selectExited.AddListener(HandleSelectExited);
        }

        private void OnDisable()
        {
            if (Interactable == null) return;

            Interactable.selectEntered.RemoveListener(HandleSelectEntered);
            Interactable.selectExited.RemoveListener(HandleSelectExited);
        }

        /// <summary>True when this item fits a socket that accepts <paramref name="acceptedTag"/>.</summary>
        public bool MatchesSocketTag(string acceptedTag)
        {
            if (string.IsNullOrEmpty(acceptedTag) || string.IsNullOrEmpty(socketTag)) return false;

            return string.Equals(socketTag, acceptedTag, StringComparison.OrdinalIgnoreCase);
        }

        private void HandleSelectEntered(SelectEnterEventArgs args)
        {
            Grabbed?.Invoke(this);
        }

        private void HandleSelectExited(SelectExitEventArgs args)
        {
            Released?.Invoke(this);
        }
    }
}
