using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Pridefall.UI;

namespace Pridefall.Player
{
    /// <summary>
    /// Blink teleport for artificial-locomotion platforms (Quest 3 edition),
    /// offered alongside smooth locomotion per the comfort checklist: push
    /// the right thumbstick forward to aim a ballistic arc from the right
    /// hand, release to blink-teleport to a valid landing. Coexists with
    /// snap turn (which reads the same stick's X axis) by requiring a
    /// mostly-vertical deflection to begin aiming.
    ///
    /// Disabled automatically on treadmill hardware: aiming only arms when
    /// the rig's active provider reports IsArtificial.
    /// </summary>
    public class TeleportController : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] private PlayerRig _rig;
        [SerializeField] private PlayerLocomotionController _locomotion;
        [Tooltip("Arc origin; the right hand.")]
        [SerializeField] private Transform _aimHand;

        [Header("Arc")]
        [SerializeField] private float _launchSpeed = 9f;
        [SerializeField] private float _arcGravity = -9.81f;
        [SerializeField] private int _maxArcSteps = 40;
        [SerializeField] private float _arcStepSeconds = 0.05f;
        [SerializeField] private LayerMask _groundMask = ~0;
        [Tooltip("Steepest landable surface, degrees from horizontal.")]
        [SerializeField] private float _maxSlopeDegrees = 35f;

        [Header("Input")]
        [SerializeField, Range(0.3f, 0.95f)] private float _aimThreshold = 0.7f;
        [Tooltip("Reject aim start when the stick is mostly sideways (that gesture is snap turn).")]
        [SerializeField, Range(0.1f, 0.9f)] private float _maxSidewaysToAim = 0.5f;

        [Header("Visuals")]
        [Tooltip("Optional LineRenderer for the arc; point count is set from the simulation.")]
        [SerializeField] private LineRenderer _arcLine;
        [Tooltip("Optional landing reticle, activated while a valid target exists.")]
        [SerializeField] private Transform _reticle;

        private readonly List<InputDevice> _rightDevices = new();
        private readonly Vector3[] _arcPoints = new Vector3[64];

        private bool _aiming;
        private bool _hasValidTarget;
        private Vector3 _target;

        private void Awake()
        {
            if (_rig == null) _rig = GetComponentInParent<PlayerRig>();
            if (_locomotion == null) _locomotion = GetComponentInParent<PlayerLocomotionController>();
            SetVisuals(false);
        }

        private void Update()
        {
            if (!ProviderIsArtificial())
            {
                if (_aiming) StopAiming();
                return;
            }

            Vector2 stick = ReadRightStick();

            if (!_aiming)
            {
                if (stick.y >= _aimThreshold && Mathf.Abs(stick.x) <= _maxSidewaysToAim)
                {
                    _aiming = true;
                }
                return;
            }

            if (stick.y < _aimThreshold * 0.5f)
            {
                // Released: commit if we have a landing spot.
                if (_hasValidTarget) CommitTeleport();
                StopAiming();
                return;
            }

            SimulateArc();
        }

        private void SimulateArc()
        {
            if (_aimHand == null) return;

            Vector3 position = _aimHand.position;
            Vector3 velocity = _aimHand.forward * _launchSpeed;
            int count = 0;
            _hasValidTarget = false;

            _arcPoints[count++] = position;
            for (int i = 0; i < _maxArcSteps && count < _arcPoints.Length; i++)
            {
                Vector3 next = position + velocity * _arcStepSeconds;
                velocity.y += _arcGravity * _arcStepSeconds;

                if (Physics.Linecast(position, next, out RaycastHit hit, _groundMask, QueryTriggerInteraction.Ignore))
                {
                    _arcPoints[count++] = hit.point;
                    if (Vector3.Angle(hit.normal, Vector3.up) <= _maxSlopeDegrees)
                    {
                        _hasValidTarget = true;
                        _target = hit.point;
                    }
                    break;
                }

                position = next;
                _arcPoints[count++] = position;
            }

            if (_arcLine != null)
            {
                _arcLine.enabled = true;
                _arcLine.positionCount = count;
                for (int i = 0; i < count; i++) _arcLine.SetPosition(i, _arcPoints[i]);
            }
            if (_reticle != null)
            {
                _reticle.gameObject.SetActive(_hasValidTarget);
                if (_hasValidTarget) _reticle.position = _target;
            }
        }

        private void CommitTeleport()
        {
            Vector3 destination = _target;
            System.Action move = () =>
            {
                var cc = _locomotion != null ? _locomotion.Controller : null;
                Transform root = _locomotion != null ? _locomotion.transform : transform;

                if (cc != null) cc.enabled = false;
                // Land with the capsule base on the hit point.
                float halfHeight = cc != null ? cc.height * 0.5f + cc.skinWidth : 0.9f;
                root.position = destination + Vector3.up * halfHeight;
                if (cc != null) cc.enabled = true;

                if (_locomotion != null) _locomotion.ReleaseExternalControl(Vector3.zero);
            };

            if (ScreenFade.Instance != null)
            {
                ScreenFade.Instance.Blink(move);
            }
            else
            {
                move(); // no fade wired: still functional, just less comfortable
            }
        }

        private void StopAiming()
        {
            _aiming = false;
            _hasValidTarget = false;
            SetVisuals(false);
        }

        private void SetVisuals(bool on)
        {
            if (_arcLine != null) _arcLine.enabled = on;
            if (_reticle != null) _reticle.gameObject.SetActive(on);
        }

        private bool ProviderIsArtificial()
        {
            return _rig != null &&
                   _rig.ActiveLocomotionProvider != null &&
                   _rig.ActiveLocomotionProvider.IsArtificial;
        }

        private Vector2 ReadRightStick()
        {
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, _rightDevices);
            foreach (var device in _rightDevices)
            {
                if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 value) && value.sqrMagnitude > 0f)
                {
                    return value;
                }
            }
            return Vector2.zero;
        }
    }
}
