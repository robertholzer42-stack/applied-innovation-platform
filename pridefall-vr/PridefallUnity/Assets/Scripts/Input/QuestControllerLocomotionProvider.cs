using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Pridefall.Input
{
    /// <summary>
    /// Quest 3 (and generic OpenXR controller) locomotion: left thumbstick
    /// smooth locomotion steered head-relative, right thumbstick snap turn,
    /// A button assisted jump. This is the primary provider for the Quest 3
    /// edition; on Omni One hardware it never activates because PlayerRig
    /// prefers the treadmill providers.
    ///
    /// Mapping onto the ILocomotionProvider contract (built for a treadmill
    /// ring): with no ring, body yaw is approximated by head yaw, and stride
    /// yaw = head yaw + stick deflection angle. Holsters and the wrist HUD
    /// therefore follow the gaze yaw, which matches Quest player
    /// expectations. Snap turn rotates the play space around the head, so
    /// calibration offsets are unaffected.
    /// </summary>
    public class QuestControllerLocomotionProvider : MonoBehaviour, ILocomotionProvider
    {
        [Header("Rig")]
        [Tooltip("HMD transform (the camera). Steering is head-relative.")]
        [SerializeField] private Transform _head;
        [Tooltip("Play space root; snap turn rotates this around the head.")]
        [SerializeField] private Transform _playSpace;

        [Header("Movement")]
        [Tooltip("Gait speed in m/s at full stick deflection (walk band).")]
        [SerializeField] private float _walkSpeed = 2.2f;
        [Tooltip("Speed while the left stick is clicked (sprint).")]
        [SerializeField] private float _sprintSpeed = 4.0f;
        [SerializeField, Range(0f, 0.5f)] private float _stickDeadZone = 0.15f;
        [Tooltip("Smoothing time applied to speed changes, seconds.")]
        [SerializeField] private float _speedSmoothing = 0.08f;

        [Header("Snap Turn")]
        [SerializeField] private float _snapTurnDegrees = 45f;
        [Tooltip("Right-stick deflection that triggers a snap; must return below half of this to re-arm.")]
        [SerializeField, Range(0.3f, 0.95f)] private float _snapThreshold = 0.7f;

        private readonly List<InputDevice> _leftDevices = new();
        private readonly List<InputDevice> _rightDevices = new();

        private float _smoothedSpeed;
        private float _speedVelocity;
        private float _strideYaw;
        private bool _snapArmed = true;
        private bool _jumpQueued;
        private bool _jumpButtonWasDown;

        public bool IsActive
        {
            get
            {
                // Active only when a tracked left controller with a stick exists,
                // so the editor simulator still wins on desktops without XR.
                RefreshDevices();
                foreach (var device in _leftDevices)
                {
                    if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out _)) return true;
                }
                return false;
            }
        }

        public float BodyYawDegrees
        {
            get
            {
                if (_head == null || _playSpace == null) return 0f;
                return Mathf.DeltaAngle(_playSpace.eulerAngles.y, _head.eulerAngles.y);
            }
        }

        public float StrideYawDegrees => _strideYaw;
        public float GaitSpeed => _smoothedSpeed;

        private void Update()
        {
            RefreshDevices();
            UpdateMovement();
            UpdateSnapTurn();
            UpdateJump();
        }

        private void UpdateMovement()
        {
            Vector2 stick = ReadAxis(_leftDevices);
            bool sprint = ReadButton(_leftDevices, CommonUsages.primary2DAxisClick);

            float magnitude = stick.magnitude;
            float target = 0f;
            if (magnitude > _stickDeadZone)
            {
                // Remap so the dead zone edge is 0 and full deflection is 1.
                float t = Mathf.InverseLerp(_stickDeadZone, 1f, magnitude);
                target = t * (sprint ? _sprintSpeed : _walkSpeed);
                _strideYaw = BodyYawDegrees + Mathf.Atan2(stick.x, stick.y) * Mathf.Rad2Deg;
            }
            else
            {
                _strideYaw = BodyYawDegrees;
            }

            _smoothedSpeed = Mathf.SmoothDamp(_smoothedSpeed, target, ref _speedVelocity, _speedSmoothing);
        }

        private void UpdateSnapTurn()
        {
            if (_playSpace == null || _head == null) return;

            float x = ReadAxis(_rightDevices).x;

            if (_snapArmed && Mathf.Abs(x) >= _snapThreshold)
            {
                _snapArmed = false;
                float degrees = Mathf.Sign(x) * _snapTurnDegrees;
                // Rotate the play space around the head so the player pivots
                // in place instead of orbiting the play space origin.
                _playSpace.RotateAround(_head.position, Vector3.up, degrees);
            }
            else if (!_snapArmed && Mathf.Abs(x) < _snapThreshold * 0.5f)
            {
                _snapArmed = true;
            }
        }

        private void UpdateJump()
        {
            bool down = ReadButton(_rightDevices, CommonUsages.primaryButton);
            if (down && !_jumpButtonWasDown)
            {
                _jumpQueued = true;
            }
            _jumpButtonWasDown = down;
        }

        public bool ConsumeJump()
        {
            if (!_jumpQueued) return false;
            _jumpQueued = false;
            return true;
        }

        public void Calibrate()
        {
            // Head-relative steering needs no yaw zeroing; just settle speed.
            _smoothedSpeed = 0f;
            _speedVelocity = 0f;
        }

        private void RefreshDevices()
        {
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, _leftDevices);
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, _rightDevices);
        }

        private static Vector2 ReadAxis(List<InputDevice> devices)
        {
            foreach (var device in devices)
            {
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 value) && value.sqrMagnitude > 0f)
                {
                    return value;
                }
            }
            return Vector2.zero;
        }

        private static bool ReadButton(List<InputDevice> devices, InputFeatureUsage<bool> usage)
        {
            foreach (var device in devices)
            {
                if (device.TryGetFeatureValue(usage, out bool pressed) && pressed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
