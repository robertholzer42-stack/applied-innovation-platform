using UnityEngine;

namespace Pridefall.Core
{
    public enum VignetteStrength { Off, Light, Strong }

    [System.Serializable]
    public class ComfortSettings
    {
        public VignetteStrength Vignette = VignetteStrength.Light;
        [Tooltip("Accessibility fallback for off-treadmill play. Off by default on Omni One.")]
        public bool ThumbstickLocomotionFallback = false;
        public bool SnapTurnFallback = false;
        [Range(0.5f, 1.5f)] public float MovementGain = 1.25f;
    }

    /// <summary>
    /// Engagement-level state: checkpoints, respawn, comfort settings,
    /// scrap wallet. Lives across scene loads.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Comfort")]
        public ComfortSettings Comfort = new();

        [Header("Checkpointing")]
        [Tooltip("Treadmill sessions end when legs end. Checkpoint aggressively.")]
        [SerializeField] private float _autoCheckpointInterval = 240f;

        public Vector3 CheckpointPosition { get; private set; }
        public Quaternion CheckpointRotation { get; private set; }
        public string CheckpointId { get; private set; } = "start";
        public int Scrap { get; private set; }
        public int BioResin { get; private set; }

        private float _autoCheckpointTimer;
        private Transform _player;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.PlayerDied += OnPlayerDied;
        }

        private void OnDisable()
        {
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.PlayerDied -= OnPlayerDied;
        }

        public void RegisterPlayer(Transform player)
        {
            _player = player;
            if (CheckpointId == "start")
            {
                CommitCheckpoint(player.position, player.rotation, "start");
            }
        }

        private void Update()
        {
            if (_player == null) return;

            _autoCheckpointTimer += Time.deltaTime;
            if (_autoCheckpointTimer >= _autoCheckpointInterval)
            {
                CommitCheckpoint(_player.position, _player.rotation, "auto");
            }
        }

        public void CommitCheckpoint(Vector3 position, Quaternion rotation, string id)
        {
            CheckpointPosition = position;
            CheckpointRotation = rotation;
            CheckpointId = id;
            _autoCheckpointTimer = 0f;
            GameEvents.RaiseCheckpointReached(position, id);
        }

        public bool SpendScrap(int amount)
        {
            if (Scrap < amount) return false;
            Scrap -= amount;
            return true;
        }

        public bool SpendBioResin(int amount)
        {
            if (BioResin < amount) return false;
            BioResin -= amount;
            return true;
        }

        public void AddBioResin(int amount) => BioResin += Mathf.Max(0, amount);

        private void OnEnemyKilled(GameObject enemy, int scrap) => Scrap += scrap;

        private void OnPlayerDied(Vector3 position)
        {
            Invoke(nameof(Respawn), 2.5f);
        }

        private void Respawn()
        {
            if (_player != null)
            {
                // CharacterController must be disabled to teleport.
                var cc = _player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                _player.SetPositionAndRotation(CheckpointPosition, CheckpointRotation);
                if (cc != null) cc.enabled = true;
            }
            GameEvents.RaisePlayerRespawned();
        }
    }
}
