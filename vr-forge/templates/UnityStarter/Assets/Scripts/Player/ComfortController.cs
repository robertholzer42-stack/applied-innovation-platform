using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRForge.Player
{
    /// <summary>Comfort vignette presets. Off disables the vignette entirely.</summary>
    public enum VignetteStrength
    {
        Off = 0,
        Light = 1,
        Strong = 2
    }

    /// <summary>
    /// Drives the comfort vignette. While the assigned smooth locomotion
    /// provider is moving the player, the vignette closes to the configured
    /// strength, escalated to Strong when auto-strong is on. Two render
    /// paths are supported:
    ///
    /// 1. XRIT path (preferred): assign the TunnelingVignetteController that
    ///    ships with the XR Interaction Toolkit Starter Assets sample (child
    ///    of the XR Origin main camera). This component feeds it aperture
    ///    parameters as an ITunnelingVignetteProvider.
    /// 2. Fallback path: assign a CanvasGroup on a camera-space canvas that
    ///    holds a full-view radial vignette sprite; its alpha is faded in
    ///    Update instead. Used only when no controller is assigned.
    /// </summary>
    public class ComfortController : MonoBehaviour, ITunnelingVignetteProvider
    {
        [Header("Settings")]
        [SerializeField]
        private VignetteStrength strength = VignetteStrength.Light;

        [SerializeField]
        [Tooltip("Escalate to Strong while smooth locomotion is active, regardless of the configured strength (unless Off).")]
        private bool autoStrongDuringSmoothLocomotion = true;

        [Header("Locomotion source")]
        [SerializeField]
        [Tooltip("The smooth (continuous) move provider on the XR Origin. Teleport providers should NOT be assigned here.")]
        private ContinuousMoveProviderBase smoothLocomotion;

        [Header("Render paths")]
        [SerializeField]
        [Tooltip("XRIT tunneling vignette on the main camera. Preferred path.")]
        private TunnelingVignetteController vignetteController;

        [SerializeField]
        [Tooltip("Fallback: CanvasGroup holding a radial vignette sprite on a camera-space canvas. Used when no controller is assigned.")]
        private CanvasGroup fallbackOverlay;

        [SerializeField]
        private float fallbackFadeSpeed = 4f;

        private readonly VignetteParameters _parameters = new VignetteParameters();
        private bool _isMoving;
        private float _fallbackTargetAlpha;

        /// <summary>Parameters consumed by the XRIT TunnelingVignetteController.</summary>
        public VignetteParameters vignetteParameters => _parameters;

        /// <summary>Configured strength. Writes at runtime re-apply immediately (settings menu).</summary>
        public VignetteStrength Strength
        {
            get => strength;
            set
            {
                strength = value;
                Apply();
            }
        }

        private void OnEnable()
        {
            if (smoothLocomotion != null)
            {
                smoothLocomotion.beginLocomotion += HandleBeginLocomotion;
                smoothLocomotion.endLocomotion += HandleEndLocomotion;
            }
            else
            {
                Debug.LogWarning("[ComfortController] No smooth locomotion provider assigned; vignette will never engage.", this);
            }
            Apply();
        }

        private void OnDisable()
        {
            if (smoothLocomotion != null)
            {
                smoothLocomotion.beginLocomotion -= HandleBeginLocomotion;
                smoothLocomotion.endLocomotion -= HandleEndLocomotion;
            }
            _isMoving = false;
            if (vignetteController != null)
            {
                vignetteController.EndTunnelingVignette(this);
            }
            if (fallbackOverlay != null)
            {
                fallbackOverlay.alpha = 0f;
            }
            _fallbackTargetAlpha = 0f;
        }

        private void Update()
        {
            if (fallbackOverlay == null) return;

            float alpha = fallbackOverlay.alpha;
            if (!Mathf.Approximately(alpha, _fallbackTargetAlpha))
            {
                fallbackOverlay.alpha = Mathf.MoveTowards(alpha, _fallbackTargetAlpha, fallbackFadeSpeed * Time.deltaTime);
            }
        }

        private void HandleBeginLocomotion(LocomotionSystem system)
        {
            _isMoving = true;
            Apply();
        }

        private void HandleEndLocomotion(LocomotionSystem system)
        {
            _isMoving = false;
            Apply();
        }

        private void Apply()
        {
            VignetteStrength effective = EffectiveStrength();

            _parameters.apertureSize = ApertureFor(effective);
            _parameters.featheringEffect = 0.25f;
            _parameters.easeInTime = 0.3f;
            _parameters.easeOutTime = 0.3f;

            if (vignetteController != null)
            {
                if (effective == VignetteStrength.Off)
                {
                    vignetteController.EndTunnelingVignette(this);
                }
                else
                {
                    vignetteController.BeginTunnelingVignette(this);
                }
                _fallbackTargetAlpha = 0f;
            }
            else
            {
                _fallbackTargetAlpha = AlphaFor(effective);
            }
        }

        private VignetteStrength EffectiveStrength()
        {
            if (strength == VignetteStrength.Off || !_isMoving)
            {
                return VignetteStrength.Off;
            }
            return autoStrongDuringSmoothLocomotion ? VignetteStrength.Strong : strength;
        }

        private static float ApertureFor(VignetteStrength value)
        {
            switch (value)
            {
                case VignetteStrength.Light: return 0.7f;
                case VignetteStrength.Strong: return 0.45f;
                default: return 1f; // fully open, no vignette
            }
        }

        private static float AlphaFor(VignetteStrength value)
        {
            switch (value)
            {
                case VignetteStrength.Light: return 0.45f;
                case VignetteStrength.Strong: return 0.75f;
                default: return 0f;
            }
        }
    }
}
