using UnityEngine;

namespace Pridefall.Input
{
    /// <summary>
    /// Editor/desktop stand-in for the Omni One treadmill so gameplay can be
    /// iterated without hardware. W/S walk and backpedal, A/D rotate the
    /// body ring, Left Shift runs, Space queues a jump.
    /// </summary>
    public class SimulatedLocomotionProvider : MonoBehaviour, ILocomotionProvider
    {
        [SerializeField] private float _walkSpeed = 1.4f;
        [SerializeField] private float _runSpeed = 3.2f;
        [SerializeField] private float _turnSpeedDegPerSec = 120f;
        [SerializeField] private float _acceleration = 6f;

        private float _bodyYaw;
        private float _speed;
        private bool _backpedal;
        private bool _jumpQueued;

        public bool IsActive => true;
        public bool IsArtificial => true;
        public float BodyYawDegrees => _bodyYaw;
        public float StrideYawDegrees => _backpedal ? _bodyYaw + 180f : _bodyYaw;
        public float GaitSpeed => _speed;

        private void Update()
        {
            float turn = 0f;
            if (UnityEngine.Input.GetKey(KeyCode.A)) turn -= 1f;
            if (UnityEngine.Input.GetKey(KeyCode.D)) turn += 1f;
            _bodyYaw += turn * _turnSpeedDegPerSec * Time.deltaTime;

            float targetSpeed = 0f;
            _backpedal = false;
            if (UnityEngine.Input.GetKey(KeyCode.W))
            {
                targetSpeed = UnityEngine.Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.S))
            {
                targetSpeed = _walkSpeed * 0.6f;
                _backpedal = true;
            }

            _speed = Mathf.MoveTowards(_speed, targetSpeed, _acceleration * Time.deltaTime);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                _jumpQueued = true;
            }
        }

        public bool ConsumeJump()
        {
            if (!_jumpQueued) return false;
            _jumpQueued = false;
            return true;
        }

        public void Calibrate()
        {
            _bodyYaw = 0f;
            _speed = 0f;
        }
    }
}
