using System;
using UnityEngine;
using Pridefall.Player;

namespace Pridefall.Enemies
{
    /// <summary>
    /// Shared enemy senses. Sight is a cone with a Linecast occlusion test
    /// run on a low-rate timer; a pack of enemies raycasting every frame is
    /// not affordable on the XR2 budget. Hearing is a static broadcast
    /// channel: weapons and loud world events call ReportNoise and every
    /// enabled perception inside the radius hears it.
    /// </summary>
    public class EnemyPerception : MonoBehaviour
    {
        [Header("Sight")]
        [Tooltip("Eye anchor; falls back to this transform.")]
        [SerializeField] private Transform _eye;
        [Tooltip("Full cone angle in degrees.")]
        [SerializeField] private float _sightAngle = 110f;
        [SerializeField] private float _sightRange = 25f;
        [Tooltip("Geometry that can block sight. Hits on the player or this enemy are ignored at runtime.")]
        [SerializeField] private LayerMask _obstructionMask = ~0;
        [Tooltip("Sight tests per second, deliberately well below frame rate.")]
        [SerializeField] private float _checksPerSecond = 4f;

        [Header("Hearing")]
        [SerializeField] private bool _deaf;

        public bool CanSeePlayer { get; private set; }
        /// <summary>Player root position at last sighting, or the origin of the last heard noise. A valid navigation target.</summary>
        public Vector3 LastKnownPlayerPosition { get; private set; }

        public event Action OnSpotted;
        public event Action OnLost;
        public event Action<Vector3> OnNoiseHeard;

        private static event Action<Vector3, float> NoiseBroadcast;

        private static PlayerRig _player;

        private float _nextCheckTime;

        /// <summary>Broadcast a noise (gunshot, geyser burst, sprint splash) to every listening enemy.</summary>
        public static void ReportNoise(Vector3 position, float radius)
        {
            NoiseBroadcast?.Invoke(position, radius);
        }

        private void OnEnable()
        {
            NoiseBroadcast += HandleNoise;
            CanSeePlayer = false;
            // Random phase so a pack's sight casts do not all land on the same frame.
            _nextCheckTime = Time.time + UnityEngine.Random.value / Mathf.Max(_checksPerSecond, 0.01f);
        }

        private void OnDisable()
        {
            NoiseBroadcast -= HandleNoise;
        }

        private void Update()
        {
            if (Time.time < _nextCheckTime) return;
            _nextCheckTime = Time.time + 1f / Mathf.Max(_checksPerSecond, 0.01f);
            RunSightCheck();
        }

        private void RunSightCheck()
        {
            PlayerRig player = GetPlayer();
            bool visible = false;

            if (player != null && player.Head != null)
            {
                Transform eye = _eye != null ? _eye : transform;
                Vector3 headPosition = player.Head.position;
                Vector3 toHead = headPosition - eye.position;

                if (toHead.sqrMagnitude <= _sightRange * _sightRange &&
                    Vector3.Angle(eye.forward, toHead) <= _sightAngle * 0.5f)
                {
                    visible = true;
                    if (Physics.Linecast(eye.position, headPosition, out RaycastHit hit, _obstructionMask, QueryTriggerInteraction.Ignore))
                    {
                        // Hitting the player or our own body is not occlusion.
                        visible = hit.transform.IsChildOf(player.transform) || hit.transform.IsChildOf(transform);
                    }
                }

                if (visible) LastKnownPlayerPosition = player.transform.position;
            }

            if (visible != CanSeePlayer)
            {
                CanSeePlayer = visible;
                if (visible) OnSpotted?.Invoke();
                else OnLost?.Invoke();
            }
        }

        private void HandleNoise(Vector3 position, float radius)
        {
            if (_deaf) return;
            if ((position - transform.position).sqrMagnitude > radius * radius) return;

            LastKnownPlayerPosition = position;
            OnNoiseHeard?.Invoke(position);
        }

        private static PlayerRig GetPlayer()
        {
            if (_player == null) _player = FindObjectOfType<PlayerRig>();
            return _player;
        }
    }
}
