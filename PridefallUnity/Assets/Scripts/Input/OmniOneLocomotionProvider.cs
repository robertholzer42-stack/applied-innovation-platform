using UnityEngine;

namespace Pridefall.Input
{
    /// <summary>
    /// Virtuix Omni One adapter. The Omni One SDK ships an OmniManager-style
    /// component that exposes ring (body) orientation and gait speed from the
    /// treadmill, decoupled from the HMD. This class is the single place SDK
    /// types are touched; everything else in the game consumes
    /// ILocomotionProvider.
    ///
    /// Integration: import the Omni One SDK package from the Virtuix
    /// developer portal (developer.virtuix.com), add the scripting define
    /// OMNI_ONE_SDK to the Android player settings, and fill in the three
    /// marked calls below with the SDK's accessor names. The wrapper pattern
    /// means an SDK update touches one file. See
    /// docs/omni-one-integration.md for the full walkthrough.
    /// </summary>
    public class OmniOneLocomotionProvider : MonoBehaviour, ILocomotionProvider
    {
        [Header("Gait Tuning")]
        [Tooltip("Smoothing time for raw treadmill speed, seconds.")]
        [SerializeField] private float _speedSmoothing = 0.08f;
        [Tooltip("Speeds below this are treated as standing still (sensor noise floor).")]
        [SerializeField] private float _deadZone = 0.08f;

        [Header("Assisted Jump")]
        [Tooltip("Assisted jump consumes the primary button; impulse scales with gait speed in PlayerLocomotionController.")]
        [SerializeField] private bool _assistedJumpEnabled = true;

        private float _smoothedSpeed;
        private float _speedVelocity;
        private float _calibrationYawOffset;
        private bool _jumpQueued;

        public bool IsActive
        {
            get
            {
#if OMNI_ONE_SDK
                return OmniOne.OmniManager.Instance != null && OmniOne.OmniManager.Instance.IsConnected;
#else
                return false;
#endif
            }
        }

        public float BodyYawDegrees { get; private set; }
        public float StrideYawDegrees { get; private set; }
        public float GaitSpeed => _smoothedSpeed;

        private void Update()
        {
            if (!IsActive) return;

#if OMNI_ONE_SDK
            // --- SDK touchpoints (verify names against your SDK version) ---
            var omni = OmniOne.OmniManager.Instance;
            float rawRingYaw = omni.RingAngle;          // degrees, body/ring orientation
            float rawSpeed   = omni.GaitSpeed;          // m/s from treadmill
            bool backpedal   = omni.IsBackpedaling;
            // ---------------------------------------------------------------

            BodyYawDegrees = Mathf.DeltaAngle(0f, rawRingYaw - _calibrationYawOffset);
            StrideYawDegrees = backpedal ? BodyYawDegrees + 180f : BodyYawDegrees;

            float target = rawSpeed < _deadZone ? 0f : rawSpeed;
            _smoothedSpeed = Mathf.SmoothDamp(_smoothedSpeed, target, ref _speedVelocity, _speedSmoothing);

            if (_assistedJumpEnabled && PollPrimaryButtonDown())
            {
                _jumpQueued = true;
            }
#endif
        }

        public bool ConsumeJump()
        {
            if (!_jumpQueued) return false;
            _jumpQueued = false;
            return true;
        }

        public void Calibrate()
        {
#if OMNI_ONE_SDK
            // Zero the ring against the play space's forward axis.
            _calibrationYawOffset = OmniOne.OmniManager.Instance.RingAngle;
#endif
            _smoothedSpeed = 0f;
            _speedVelocity = 0f;
        }

        private bool PollPrimaryButtonDown()
        {
            var devices = new System.Collections.Generic.List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
                UnityEngine.XR.InputDeviceCharacteristics.Controller |
                UnityEngine.XR.InputDeviceCharacteristics.Right, devices);
            foreach (var device in devices)
            {
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool pressed) && pressed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
