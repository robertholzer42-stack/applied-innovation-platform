using UnityEngine;
using Pridefall.Core;
using Pridefall.Input;

namespace Pridefall.Player
{
    public enum MovementState
    {
        Grounded,
        Airborne,
        Climbing,
        Swimming
    }

    /// <summary>
    /// Translates ILocomotionProvider (Omni One treadmill, or the editor
    /// simulator) into CharacterController motion. Climbing and swimming
    /// systems take over by setting an external velocity each frame; this
    /// controller stays the single writer to CharacterController.Move so
    /// state transitions never double-move the capsule.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerLocomotionController : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] private PlayerRig _rig;

        [Header("Gait")]
        [Tooltip("Multiplier on treadmill speed. >1 makes a comfortable jog read as an in-game sprint.")]
        [SerializeField] private float _movementGain = 1.25f;
        [SerializeField] private float _maxGroundSpeed = 5f;

        [Header("Jumping")]
        [SerializeField] private float _baseJumpImpulse = 3.6f;
        [Tooltip("Extra jump impulse per m/s of gait speed; sprint-then-jump = long jump.")]
        [SerializeField] private float _jumpGaitBonus = 0.55f;

        [Header("Gravity")]
        [SerializeField] private float _gravity = -8.83f; // Khepri: 0.9 g

        public MovementState State { get; private set; } = MovementState.Grounded;
        public Vector3 Velocity => _velocity;
        public CharacterController Controller { get; private set; }

        /// <summary>Low-g anomaly zones scale gravity and jump (set by LowGravityZone).</summary>
        public float GravityScale { get; set; } = 1f;

        private ILocomotionProvider _provider;
        private Vector3 _velocity;
        private Vector3 _externalVelocity;
        private bool _externalControlThisFrame;

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            if (_rig == null) _rig = GetComponent<PlayerRig>();
        }

        private void Start()
        {
            _provider = _rig.ActiveLocomotionProvider;
            if (GameManager.Instance != null)
            {
                _movementGain = GameManager.Instance.Comfort.MovementGain;
                GameManager.Instance.RegisterPlayer(transform);
            }
        }

        /// <summary>
        /// Climbing/swimming call this every frame they own the body.
        /// Velocity is world-space; gravity is suppressed while external.
        /// </summary>
        public void SetExternalVelocity(Vector3 worldVelocity, MovementState state)
        {
            _externalVelocity = worldVelocity;
            _externalControlThisFrame = true;
            State = state;
        }

        /// <summary>Climbing release hands the body back with inherited momentum.</summary>
        public void ReleaseExternalControl(Vector3 inheritedVelocity)
        {
            _externalControlThisFrame = false;
            _velocity = inheritedVelocity;
            State = Controller.isGrounded ? MovementState.Grounded : MovementState.Airborne;
        }

        private void Update()
        {
            if (_provider == null) return;

            if (_externalControlThisFrame)
            {
                Controller.Move(_externalVelocity * Time.deltaTime);
                _externalControlThisFrame = false;
                return;
            }

            float scaledGravity = _gravity * GravityScale;

            // Treadmill stride in world space, rotated by the play space yaw.
            float worldStrideYaw = _rig.PlaySpace.eulerAngles.y + _provider.StrideYawDegrees;
            Vector3 strideDirection = Quaternion.Euler(0f, worldStrideYaw, 0f) * Vector3.forward;
            float speed = Mathf.Min(_provider.GaitSpeed * _movementGain, _maxGroundSpeed);

            Vector3 planar = strideDirection * speed;

            if (Controller.isGrounded)
            {
                State = MovementState.Grounded;
                _velocity = new Vector3(planar.x, -1f, planar.z); // small stick-to-ground

                if (_provider.ConsumeJump())
                {
                    float impulse = (_baseJumpImpulse + _provider.GaitSpeed * _jumpGaitBonus) / Mathf.Sqrt(Mathf.Max(GravityScale, 0.05f));
                    _velocity.y = impulse;
                    State = MovementState.Airborne;
                }
            }
            else
            {
                State = MovementState.Airborne;
                // Limited air control: the treadmill still steers at 40%.
                _velocity.x = Mathf.Lerp(_velocity.x, planar.x, 0.4f * Time.deltaTime * 5f);
                _velocity.z = Mathf.Lerp(_velocity.z, planar.z, 0.4f * Time.deltaTime * 5f);
                _velocity.y += scaledGravity * Time.deltaTime;
            }

            Controller.Move(_velocity * Time.deltaTime);
        }
    }
}
