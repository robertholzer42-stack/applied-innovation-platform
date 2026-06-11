using System.Collections.Generic;
using UnityEngine;
using Pridefall.Core;
using Pridefall.Interaction;

namespace Pridefall.Player
{
    /// <summary>
    /// Hubris-style hand-over-hand climbing. While one or both hands grip a
    /// ClimbHold, the body moves opposite to hand motion (you pull the world
    /// past you). Treadmill input is ignored during two-handed climbs; with
    /// one hand committed, gait adds a small swing. Overhangs drain stamina;
    /// at zero stamina the hands force-release.
    /// </summary>
    public class ClimbingSystem : MonoBehaviour
    {
        [SerializeField] private PlayerRig _rig;
        [SerializeField] private PlayerLocomotionController _locomotion;

        [Header("Stamina")]
        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _overhangDrainPerSecond = 12f;
        [SerializeField] private float _regenPerSecond = 20f;

        [Header("Feel")]
        [Tooltip("Velocity inherited when releasing mid-pull, for dyno moves.")]
        [SerializeField] private float _releaseMomentumScale = 0.9f;

        public float StaminaNormalized => _stamina / _maxStamina;
        public bool IsClimbing => _grippedHands.Count > 0;

        private readonly List<HandController> _grippedHands = new(2);
        private readonly Dictionary<HandController, Vector3> _gripAnchors = new(2);
        private readonly Dictionary<HandController, float> _gripTimes = new(2);
        private float _stamina;
        private Vector3 _lastFrameBodyDelta;

        private void Awake()
        {
            _stamina = _maxStamina;
            if (_rig == null) _rig = GetComponent<PlayerRig>();
            if (_locomotion == null) _locomotion = GetComponent<PlayerLocomotionController>();
        }

        private void OnEnable()
        {
            Subscribe(_rig != null ? _rig.LeftHand : null);
            Subscribe(_rig != null ? _rig.RightHand : null);
        }

        private void OnDisable()
        {
            Unsubscribe(_rig != null ? _rig.LeftHand : null);
            Unsubscribe(_rig != null ? _rig.RightHand : null);
        }

        private void Subscribe(HandController hand)
        {
            if (hand == null) return;
            hand.ClimbGripStarted += OnGripStarted;
            hand.ClimbGripEnded += OnGripEnded;
        }

        private void Unsubscribe(HandController hand)
        {
            if (hand == null) return;
            hand.ClimbGripStarted -= OnGripStarted;
            hand.ClimbGripEnded -= OnGripEnded;
        }

        private void OnGripStarted(HandController hand, ClimbHold hold)
        {
            if (!_grippedHands.Contains(hand)) _grippedHands.Add(hand);
            _gripAnchors[hand] = hand.transform.position;
            _gripTimes[hand] = Time.time;

            if (!string.IsNullOrEmpty(hold.CheckpointId) && GameManager.Instance != null)
            {
                GameManager.Instance.CommitCheckpoint(transform.position, transform.rotation, hold.CheckpointId);
            }
        }

        private void OnGripEnded(HandController hand, ClimbHold hold)
        {
            _grippedHands.Remove(hand);
            _gripAnchors.Remove(hand);
            _gripTimes.Remove(hand);

            if (_grippedHands.Count == 0)
            {
                // Hand the body back with momentum so dynos feel right.
                _locomotion.ReleaseExternalControl(_lastFrameBodyDelta * _releaseMomentumScale / Mathf.Max(Time.deltaTime, 0.001f));
            }
        }

        private void Update()
        {
            if (!IsClimbing)
            {
                _stamina = Mathf.Min(_maxStamina, _stamina + _regenPerSecond * Time.deltaTime);
                return;
            }

            // Crumbling holds and stamina.
            bool onOverhang = false;
            for (int i = _grippedHands.Count - 1; i >= 0; i--)
            {
                var hand = _grippedHands[i];
                var hold = hand.HeldClimbHold;
                if (hold == null) continue;

                onOverhang |= hold.IsOverhang;

                if (hold.CrumbleSeconds > 0f && Time.time - _gripTimes[hand] > hold.CrumbleSeconds)
                {
                    hand.ForceRelease();
                }
            }

            if (onOverhang)
            {
                _stamina -= _overhangDrainPerSecond * Time.deltaTime;
                if (_stamina <= 0f)
                {
                    _stamina = 0f;
                    ForceReleaseAll();
                    return;
                }
            }

            // Body motion = average of (anchor - current hand position):
            // pulling your hand down moves the body up.
            Vector3 pull = Vector3.zero;
            foreach (var hand in _grippedHands)
            {
                pull += _gripAnchors[hand] - hand.transform.position;
                _gripAnchors[hand] = hand.transform.position + (_gripAnchors[hand] - hand.transform.position); // anchors stay world-fixed
            }
            pull /= _grippedHands.Count;

            _lastFrameBodyDelta = pull;
            _locomotion.SetExternalVelocity(pull / Mathf.Max(Time.deltaTime, 0.001f), MovementState.Climbing);

            // Re-anchor relative to the body move so grip points stay glued to the wall.
            var hands = new List<HandController>(_gripAnchors.Keys);
            foreach (var hand in hands)
            {
                _gripAnchors[hand] -= pull;
            }
        }

        private void ForceReleaseAll()
        {
            for (int i = _grippedHands.Count - 1; i >= 0; i--)
            {
                _grippedHands[i].ForceRelease();
            }
        }
    }
}
