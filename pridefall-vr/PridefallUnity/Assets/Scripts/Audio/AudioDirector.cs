using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Audio
{
    /// <summary>
    /// Central audio brain: a pooled bed of one-shot AudioSources
    /// (Instantiate-free impacts, barks, UI blips) plus three-layer music.
    /// Calm/combat crossfade follows GameEvents.CombatIntensityChanged with
    /// a boss layer joining at high intensity, and music ducks briefly when
    /// the player dies so the respawn beat lands quiet.
    /// </summary>
    public class AudioDirector : MonoBehaviour
    {
        public static AudioDirector Instance { get; private set; }

        [Header("One-Shot Pool")]
        [Tooltip("Worst case is a Warden volley landing during a Skimmer dive; 12 covers it.")]
        [SerializeField] private int _oneShotSourceCount = 12;
        [Tooltip("Optional template whose 3D/rolloff settings every pooled source copies. A default spatialized source is built if unset.")]
        [SerializeField] private AudioSource _oneShotTemplate;

        [Header("Music Layers")]
        [SerializeField] private AudioSource _calmLoop;
        [SerializeField] private AudioSource _combatLoop;
        [SerializeField] private AudioSource _bossLoop;
        [Tooltip("Volume units per second; 0.8 means a full crossfade takes ~1.25 s.")]
        [SerializeField] private float _crossfadeSpeed = 0.8f;
        [Tooltip("Combat intensity at or above this brings in the boss layer.")]
        [SerializeField, Range(0f, 1f)] private float _bossThreshold = 0.9f;

        [Header("Death Duck")]
        [SerializeField, Range(0f, 1f)] private float _duckLevel = 0.2f;
        [SerializeField] private float _duckSeconds = 2.5f;
        [SerializeField] private float _duckRecoverSpeed = 0.6f;

        private AudioSource[] _oneShotPool;
        private int _nextOneShot;

        private float _intensity;
        private float _duckTimer;
        private float _duck = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            BuildOneShotPool();
            PrimeLoop(_calmLoop, 1f);
            PrimeLoop(_combatLoop, 0f);
            PrimeLoop(_bossLoop, 0f);
        }

        private void OnEnable()
        {
            GameEvents.CombatIntensityChanged += OnCombatIntensityChanged;
            GameEvents.PlayerDied += OnPlayerDied;
        }

        private void OnDisable()
        {
            GameEvents.CombatIntensityChanged -= OnCombatIntensityChanged;
            GameEvents.PlayerDied -= OnPlayerDied;
        }

        /// <summary>
        /// Plays a clip at a world position from the pooled bed. Round-robin:
        /// if every source is busy the oldest is stolen, which is the right
        /// behavior for impact-style sounds.
        /// </summary>
        public void PlayOneShotAt(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null || _oneShotPool == null) return;

            AudioSource source = _oneShotPool[_nextOneShot];
            _nextOneShot = (_nextOneShot + 1) % _oneShotPool.Length;
            if (source == null) return;

            source.transform.position = position;
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume);
            source.Play();
        }

        private void BuildOneShotPool()
        {
            _oneShotPool = new AudioSource[Mathf.Max(1, _oneShotSourceCount)];
            for (int i = 0; i < _oneShotPool.Length; i++)
            {
                AudioSource source;
                if (_oneShotTemplate != null)
                {
                    source = Instantiate(_oneShotTemplate, transform);
                }
                else
                {
                    var child = new GameObject("OneShot_" + i);
                    child.transform.SetParent(transform, false);
                    source = child.AddComponent<AudioSource>();
                    source.spatialBlend = 1f;
                    source.rolloffMode = AudioRolloffMode.Linear;
                    source.maxDistance = 40f;
                }
                source.playOnAwake = false;
                source.loop = false;
                _oneShotPool[i] = source;
            }
        }

        private static void PrimeLoop(AudioSource source, float volume)
        {
            if (source == null || source.clip == null) return;
            source.loop = true;
            source.volume = volume;
            // All layers always play so they stay time-aligned; mixing is volume-only.
            if (!source.isPlaying) source.Play();
        }

        private void Update()
        {
            if (_duckTimer > 0f)
            {
                _duckTimer -= Time.deltaTime;
                // Duck in faster than the normal crossfade so the death beat reads.
                _duck = Mathf.MoveTowards(_duck, _duckLevel, _crossfadeSpeed * 4f * Time.deltaTime);
            }
            else
            {
                _duck = Mathf.MoveTowards(_duck, 1f, _duckRecoverSpeed * Time.deltaTime);
            }

            Fade(_calmLoop, (1f - _intensity) * _duck);
            Fade(_combatLoop, _intensity * _duck);
            Fade(_bossLoop, (_intensity >= _bossThreshold ? 1f : 0f) * _duck);
        }

        private void Fade(AudioSource source, float target)
        {
            if (source == null) return;
            source.volume = Mathf.MoveTowards(source.volume, target, _crossfadeSpeed * Time.deltaTime);
        }

        private void OnCombatIntensityChanged(float intensity) => _intensity = Mathf.Clamp01(intensity);

        private void OnPlayerDied(Vector3 position) => _duckTimer = _duckSeconds;
    }
}
