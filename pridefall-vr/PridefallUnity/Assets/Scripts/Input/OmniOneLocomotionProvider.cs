using UnityEngine;

namespace Pridefall.Input
{
    /// <summary>
    /// Virtuix Omni One on-device adapter (standalone headset build).
    ///
    /// The Omni One Unity SDK (developers.virtuix.com, imported as the
    /// OmniSDK .unitypackage) exposes a Movement module that "retrieves
    /// movement data from the Omni One" and needs no platform
    /// initialization. Its data model, confirmed across all Virtuix SDK
    /// generations, is a 2D gait vector plus a body/ring yaw angle that is
    /// decoupled from the HMD. This adapter converts that into
    /// ILocomotionProvider.
    ///
    /// The Movement API's exact member names are gated behind the Virtuix
    /// developer portal (the doxygen is not public), so the two SDK
    /// touchpoints below are isolated in ReadGaitVector/ReadBodyYaw and
    /// written against the data model rather than guessed names. After
    /// importing the SDK: add the OMNI_ONE_SDK scripting define for the
    /// Android build target and fill the two methods from the Movement
    /// group of the SDK's doxygen, a two-line change. The legacy SDK
    /// equivalents (OmniMovementComponent.currentOmniYaw, OmniMotionData
    /// GamePad_X/Y) and the PC-side OmniConnectManager.GetMovementVector /
    /// GetArmYaw show the expected shape. See docs/omni-one-integration.md.
    /// </summary>
    public class OmniOneLocomotionProvider : MonoBehaviour, ILocomotionProvider
    {
        [Header("Gait Tuning")]
        [Tooltip("Gait speed in m/s when the treadmill reports a full-magnitude movement vector.")]
        [SerializeField] private float _maxGaitSpeed = 4.2f;
        [Tooltip("Smoothing time for raw treadmill speed, seconds.")]
        [SerializeField] private float _speedSmoothing = 0.08f;
        [Tooltip("Vector magnitudes below this are standing still (sensor noise floor).")]
        [SerializeField] private float _deadZone = 0.05f;

        [Header("Assisted Jump")]
        [Tooltip("Assisted jump on the right primary button; impulse scales with gait speed in PlayerLocomotionController.")]
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
                return true; // Movement module needs no init; device presence is implied on-device.
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

            Vector2 gait = ReadGaitVector();   // x = strafe, y = forward, in body frame
            float ringYaw = ReadBodyYaw();     // degrees, decoupled from HMD

            BodyYawDegrees = Mathf.DeltaAngle(0f, ringYaw - _calibrationYawOffset);

            float magnitude = gait.magnitude;
            if (magnitude < _deadZone)
            {
                magnitude = 0f;
                StrideYawDegrees = BodyYawDegrees;
            }
            else
            {
                // Stride direction = body yaw plus the gait vector's own angle,
                // so strafing and backpedaling on the disc steer correctly.
                StrideYawDegrees = BodyYawDegrees + Mathf.Atan2(gait.x, gait.y) * Mathf.Rad2Deg;
            }

            float target = Mathf.Clamp01(magnitude) * _maxGaitSpeed;
            _smoothedSpeed = Mathf.SmoothDamp(_smoothedSpeed, target, ref _speedVelocity, _speedSmoothing);

            if (_assistedJumpEnabled && PollPrimaryButtonDown())
            {
                _jumpQueued = true;
            }
        }

        // ----- SDK touchpoints: fill from the Omni One SDK Movement module -----

        private Vector2 ReadGaitVector()
        {
#if OMNI_ONE_SDK
            // e.g. return OmniMovement.GetMovementVector();  // per portal doxygen
            return Vector2.zero;
#else
            return Vector2.zero;
#endif
        }

        private float ReadBodyYaw()
        {
#if OMNI_ONE_SDK
            // e.g. return OmniMovement.GetBodyYaw();  // ring angle, degrees
            return 0f;
#else
            return 0f;
#endif
        }

        // -----------------------------------------------------------------------

        public bool ConsumeJump()
        {
            if (!_jumpQueued) return false;
            _jumpQueued = false;
            return true;
        }

        public void Calibrate()
        {
            _calibrationYawOffset = ReadBodyYaw();
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
