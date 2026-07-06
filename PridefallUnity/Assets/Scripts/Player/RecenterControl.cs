using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Pridefall.Player
{
    /// <summary>
    /// In-game recenter: hold the left controller's menu button for one
    /// second to re-align the play space forward with the current head yaw
    /// and re-run provider calibration (the diegetic "suit fitting" refresh).
    /// Addresses the comfort-audit finding that nothing called Calibrate()
    /// after boot; also the seated-play recovery path.
    /// </summary>
    public class RecenterControl : MonoBehaviour
    {
        [SerializeField] private PlayerRig _rig;
        [Tooltip("Seconds the menu button must be held before recentering.")]
        [SerializeField] private float _holdSeconds = 1f;

        private readonly List<InputDevice> _leftDevices = new();
        private float _heldFor;
        private bool _firedThisHold;

        public event System.Action Recentered;

        private void Awake()
        {
            if (_rig == null) _rig = GetComponentInParent<PlayerRig>();
        }

        private void Update()
        {
            if (ReadMenuButton())
            {
                _heldFor += Time.unscaledDeltaTime;
                if (!_firedThisHold && _heldFor >= _holdSeconds)
                {
                    _firedThisHold = true;
                    Recenter();
                }
            }
            else
            {
                _heldFor = 0f;
                _firedThisHold = false;
            }
        }

        /// <summary>Also callable from a pause/settings menu.</summary>
        public void Recenter()
        {
            if (_rig == null || _rig.PlaySpace == null || _rig.Head == null) return;

            // Rotate the play space around the head so head yaw becomes the
            // play space forward, then let the provider re-zero against it.
            float delta = Mathf.DeltaAngle(_rig.PlaySpace.eulerAngles.y, _rig.Head.eulerAngles.y);
            _rig.PlaySpace.RotateAround(_rig.Head.position, Vector3.up, -delta);

            _rig.ActiveLocomotionProvider?.Calibrate();
            Recentered?.Invoke();
        }

        private bool ReadMenuButton()
        {
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, _leftDevices);
            foreach (var device in _leftDevices)
            {
                if (device.TryGetFeatureValue(CommonUsages.menuButton, out bool pressed) && pressed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
