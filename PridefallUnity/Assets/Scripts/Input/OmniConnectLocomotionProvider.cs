using UnityEngine;

namespace Pridefall.Input
{
    /// <summary>
    /// PCVR adapter for the Omni One Core (treadmill-only SKU) via Virtuix's
    /// Omni Connect SDK (UPM package com.virtuix.omniconnectsdk). The Omni
    /// Connect desktop app writes treadmill data to shared memory
    /// ("OmniOneSharedMemory"); the SDK's OmniConnectManager exposes it as
    /// a 2D movement vector plus an arm/body yaw. Windows only.
    ///
    /// Enable with the OMNI_CONNECT_SDK scripting define on the Windows
    /// build target after adding the package. Confirmed API:
    ///   Virtuix.OmniConnectSdk.OmniConnectManager.GetMovementVector()
    ///   Virtuix.OmniConnectSdk.OmniConnectManager.GetArmYaw()
    /// </summary>
    public class OmniConnectLocomotionProvider : MonoBehaviour, ILocomotionProvider
    {
        [Header("Gait Tuning")]
        [Tooltip("Gait speed in m/s at full-magnitude movement vector.")]
        [SerializeField] private float _maxGaitSpeed = 4.2f;
        [SerializeField] private float _speedSmoothing = 0.08f;
        [SerializeField] private float _deadZone = 0.05f;

        private float _smoothedSpeed;
        private float _speedVelocity;
        private float _calibrationYawOffset;
        private bool _jumpQueued;

        public bool IsActive
        {
            get
            {
#if OMNI_CONNECT_SDK && (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
                return Virtuix.OmniConnectSdk.OmniConnectManager.Instance != null;
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

#if OMNI_CONNECT_SDK && (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
            Vector2 gait = Virtuix.OmniConnectSdk.OmniConnectManager.GetMovementVector();
            float armYaw = Virtuix.OmniConnectSdk.OmniConnectManager.GetArmYaw();

            BodyYawDegrees = Mathf.DeltaAngle(0f, armYaw - _calibrationYawOffset);

            float magnitude = gait.magnitude;
            if (magnitude < _deadZone)
            {
                magnitude = 0f;
                StrideYawDegrees = BodyYawDegrees;
            }
            else
            {
                StrideYawDegrees = BodyYawDegrees + Mathf.Atan2(gait.x, gait.y) * Mathf.Rad2Deg;
            }

            float target = Mathf.Clamp01(magnitude) * _maxGaitSpeed;
            _smoothedSpeed = Mathf.SmoothDamp(_smoothedSpeed, target, ref _speedVelocity, _speedSmoothing);

            if (PollPrimaryButtonDown())
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
#if OMNI_CONNECT_SDK && (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
            _calibrationYawOffset = Virtuix.OmniConnectSdk.OmniConnectManager.GetArmYaw();
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
