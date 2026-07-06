using UnityEngine;
using UnityEngine.UI;
using Pridefall.Core;
using Pridefall.Player;

namespace Pridefall.UI
{
    /// <summary>
    /// Comfort tunnel on a camera-space canvas: an inverted soft-edge radial
    /// sprite (opaque rim, transparent center) whose alpha and aperture
    /// animate smoothly. Strength follows GameManager.Comfort.Vignette and
    /// is forced to Strong while the body is Airborne or Swimming, the two
    /// states where the camera moves without leg input.
    ///
    /// This UI overlay is the cheap, dependency-free implementation. On
    /// device, replace it with a URP fullscreen pass (Renderer Feature or a
    /// single transparent-queue quad) if the frame budget allows; the alpha
    /// and aperture targets computed here map 1:1 onto a shader's
    /// radius/feather parameters.
    /// </summary>
    public class ComfortVignette : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] private PlayerLocomotionController _locomotion;
        [SerializeField] private PlayerRig _rig;

        [Header("Artificial Locomotion")]
        [Tooltip("Planar speed (m/s) above which artificial ground motion (thumbstick, per the provider's IsArtificial flag) forces the strong tunnel. Treadmill gait never triggers this.")]
        [SerializeField] private float _groundMotionThreshold = 0.5f;

        [Header("Overlay")]
        [Tooltip("Fullscreen Image using an inverted radial sprite: opaque edges, transparent center.")]
        [SerializeField] private Image _overlay;

        [Header("Light")]
        [SerializeField, Range(0f, 1f)] private float _lightAlpha = 0.55f;
        [Tooltip("Overlay scale; larger = wider aperture, gentler tunnel.")]
        [SerializeField] private float _lightScale = 1.6f;

        [Header("Strong")]
        [SerializeField, Range(0f, 1f)] private float _strongAlpha = 0.85f;
        [SerializeField] private float _strongScale = 1.15f;

        [Header("Animation")]
        [Tooltip("Tunnel open/close rate in alpha (and scale) units per second. Keep fast enough that a fall is covered within ~0.25 s.")]
        [SerializeField] private float _animateSpeed = 4f;

        private float _alpha;
        private float _scale;

        private void Awake()
        {
            if (_locomotion == null) _locomotion = GetComponentInParent<PlayerLocomotionController>();
            if (_rig == null) _rig = GetComponentInParent<PlayerRig>();
            _scale = _lightScale;
            ApplyToOverlay();
        }

        // LateUpdate so the locomotion state read here is this frame's, not last frame's.
        private void LateUpdate()
        {
            VignetteStrength strength = GameManager.Instance != null
                ? GameManager.Instance.Comfort.Vignette
                : VignetteStrength.Light;

            // Off is a player choice; never override it (comfort checklist:
            // settings the player made are respected everywhere).
            if (strength != VignetteStrength.Off && _locomotion != null)
            {
                if (_locomotion.State == MovementState.Airborne || _locomotion.State == MovementState.Swimming)
                {
                    strength = VignetteStrength.Strong;
                }
                else if (_locomotion.State == MovementState.Grounded && IsArtificialLocomotion())
                {
                    Vector3 velocity = _locomotion.Velocity;
                    velocity.y = 0f;
                    if (velocity.sqrMagnitude > _groundMotionThreshold * _groundMotionThreshold)
                    {
                        strength = VignetteStrength.Strong;
                    }
                }
            }

            float targetAlpha;
            float targetScale;
            switch (strength)
            {
                case VignetteStrength.Strong:
                    targetAlpha = _strongAlpha;
                    targetScale = _strongScale;
                    break;
                case VignetteStrength.Light:
                    targetAlpha = _lightAlpha;
                    targetScale = _lightScale;
                    break;
                default:
                    targetAlpha = 0f;
                    targetScale = _lightScale;
                    break;
            }

            _alpha = Mathf.MoveTowards(_alpha, targetAlpha, _animateSpeed * Time.deltaTime);
            _scale = Mathf.MoveTowards(_scale, targetScale, _animateSpeed * Time.deltaTime);
            ApplyToOverlay();
        }

        private bool IsArtificialLocomotion()
        {
            return _rig != null &&
                   _rig.ActiveLocomotionProvider != null &&
                   _rig.ActiveLocomotionProvider.IsArtificial;
        }

        private void ApplyToOverlay()
        {
            if (_overlay == null) return;

            Color color = _overlay.color;
            color.a = _alpha;
            _overlay.color = color;
            _overlay.rectTransform.localScale = new Vector3(_scale, _scale, 1f);
        }
    }
}
