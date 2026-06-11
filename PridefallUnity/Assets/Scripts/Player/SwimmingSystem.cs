using UnityEngine;
using Pridefall.Core;
using Pridefall.Environment;

namespace Pridefall.Player
{
    /// <summary>
    /// Hubris-style swimming: grip + sweep the controllers to stroke, with
    /// an Omni One twist, walking on the treadmill while submerged adds a
    /// flutter-kick speed bonus. Buoyancy pulls toward the surface; an air
    /// meter runs while the head is under.
    /// </summary>
    public class SwimmingSystem : MonoBehaviour
    {
        [SerializeField] private PlayerRig _rig;
        [SerializeField] private PlayerLocomotionController _locomotion;

        [Header("Strokes")]
        [Tooltip("Hand sweep speed (m/s) converted into body propulsion.")]
        [SerializeField] private float _strokePower = 0.55f;
        [SerializeField] private float _maxSwimSpeed = 2.6f;
        [SerializeField] private float _drag = 1.6f;

        [Header("Flutter Kick (Omni One)")]
        [Tooltip("Fraction of treadmill gait added as forward kick while submerged.")]
        [SerializeField] private float _kickContribution = 0.3f;

        [Header("Buoyancy & Air")]
        [SerializeField] private float _buoyancy = 1.2f;
        [SerializeField] private float _airSeconds = 45f;
        [SerializeField] private float _drownDamagePerSecond = 8f;

        public bool IsSwimming { get; private set; }
        public bool HeadSubmerged { get; private set; }
        public float AirNormalized => _air / _airSeconds;

        private WaterVolume _water;
        private Vector3 _swimVelocity;
        private float _air;
        private PlayerHealth _health;

        private void Awake()
        {
            if (_rig == null) _rig = GetComponent<PlayerRig>();
            if (_locomotion == null) _locomotion = GetComponent<PlayerLocomotionController>();
            _health = GetComponent<PlayerHealth>();
            _air = _airSeconds;
        }

        public void EnterWater(WaterVolume water) => _water = water;

        public void ExitWater(WaterVolume water)
        {
            if (_water == water) _water = null;
        }

        /// <summary>Bubble vents and surfacing refill air.</summary>
        public void RefillAir() => SetAir(_airSeconds);

        private void SetAir(float value)
        {
            _air = Mathf.Clamp(value, 0f, _airSeconds);
            GameEvents.RaisePlayerAirChanged(AirNormalized);
        }

        private void Update()
        {
            float chestHeight = transform.position.y + _locomotion.Controller.height * 0.6f;
            bool chestUnder = _water != null && chestHeight < _water.SurfaceY;
            bool headUnder = _water != null && _rig.Head.position.y < _water.SurfaceY;

            if (headUnder != HeadSubmerged)
            {
                HeadSubmerged = headUnder;
                GameEvents.RaisePlayerSubmergedChanged(headUnder);
            }

            UpdateAir(headUnder);

            if (!chestUnder)
            {
                if (IsSwimming)
                {
                    IsSwimming = false;
                    _locomotion.ReleaseExternalControl(_swimVelocity);
                    _swimVelocity = Vector3.zero;
                }
                return;
            }

            IsSwimming = true;

            // Strokes: while grip is held and the hand sweeps, push opposite.
            Vector3 propulsion = Vector3.zero;
            propulsion += StrokeFrom(_rig.LeftHand);
            propulsion += StrokeFrom(_rig.RightHand);

            // Flutter kick from the treadmill, along the gaze when submerged.
            var provider = _rig.ActiveLocomotionProvider;
            if (provider != null && provider.GaitSpeed > 0.1f)
            {
                Vector3 kickDirection = headUnder ? _rig.Head.forward : Vector3.ProjectOnPlane(_rig.Head.forward, Vector3.up).normalized;
                propulsion += kickDirection * (provider.GaitSpeed * _kickContribution);
            }

            _swimVelocity += propulsion * Time.deltaTime;

            // Buoyancy only below the surface; clamps so you bob, not launch.
            float depth = _water.SurfaceY - _rig.Head.position.y;
            if (depth > 0.05f)
            {
                _swimVelocity.y += _buoyancy * Mathf.Min(depth, 1f) * Time.deltaTime;
            }

            _swimVelocity = Vector3.ClampMagnitude(_swimVelocity, _maxSwimSpeed);
            _swimVelocity = Vector3.Lerp(_swimVelocity, Vector3.zero, _drag * Time.deltaTime);

            _locomotion.SetExternalVelocity(_swimVelocity, MovementState.Swimming);
        }

        private Vector3 StrokeFrom(HandController hand)
        {
            if (hand == null || !hand.GripHeld || hand.IsClimbing || hand.HeldGrabbable != null)
            {
                return Vector3.zero;
            }
            // Push opposite the hand sweep, scaled by how fast the sweep is.
            return -hand.Velocity * _strokePower;
        }

        private void UpdateAir(bool headUnder)
        {
            if (headUnder)
            {
                SetAir(_air - Time.deltaTime);
                if (_air <= 0f && _health != null)
                {
                    _health.TakeDamage(new DamageInfo(_drownDamagePerSecond * Time.deltaTime, DamageType.Corrosive,
                        _rig.Head.position, Vector3.zero, gameObject));
                }
            }
            else if (_air < _airSeconds)
            {
                SetAir(_airSeconds); // surfacing refills instantly
            }
        }
    }
}
