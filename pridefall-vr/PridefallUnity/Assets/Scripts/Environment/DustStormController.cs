using System;
using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Environment
{
    /// <summary>
    /// Chapter-level silica storm director for the Glasslands edge. Ramps a
    /// 0-1 intensity over a serialized curve and bites through enemy sight
    /// (CurrentSightMultiplier), audio, and the VFX hooks. The capsule is
    /// never moved from here: PlayerLocomotionController is the single writer
    /// to CharacterController.Move, so wind is published as the read-only
    /// CurrentWind hook for the locomotion controller to sample in a future
    /// revision rather than applied as a competing Move.
    /// </summary>
    public class DustStormController : MonoBehaviour
    {
        /// <summary>Multiplier on enemy sight range, 1 in clear air. Enemy perception samples this.</summary>
        public static float CurrentSightMultiplier { get; private set; } = 1f;

        /// <summary>
        /// World-space wind velocity (m/s) at current intensity. Hook only:
        /// nothing may Move the player from this value except the locomotion
        /// controller itself, in a future revision.
        /// </summary>
        public static Vector3 CurrentWind { get; private set; }

        [Header("Timeline")]
        [Tooltip("Intensity 0-1 over normalized storm time. Default ramps up, holds, dies down.")]
        [SerializeField] private AnimationCurve _intensityOverTime = new(
            new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(0.8f, 1f), new Keyframe(1f, 0f));
        [SerializeField] private float _stormDurationSeconds = 180f;
        [SerializeField] private bool _beginOnStart;

        [Header("Effects")]
        [Tooltip("Enemy sight multiplier at full intensity. 0.35 = Wardens see a third as far.")]
        [SerializeField, Range(0.05f, 1f)] private float _sightMultiplierAtPeak = 0.35f;
        [Tooltip("Wind speed (m/s) at full intensity, blowing along this transform's forward.")]
        [SerializeField] private float _peakWindSpeed = 6f;
        [Tooltip("Looping wind bed; volume follows intensity. Optional.")]
        [SerializeField] private AudioSource _windLoop;

        [Header("Warning")]
        [Tooltip("Intensity at which the wrist HUD onset warning fires, once per storm.")]
        [SerializeField, Range(0f, 1f)] private float _warningThreshold = 0.1f;
        [SerializeField] private string _onsetWarning = "Silica storm rolling in. Visibility is dropping. So is theirs.";

        /// <summary>Fires every frame the storm runs with the current 0-1 intensity; drive particle density and fog from here.</summary>
        public event Action<float> IntensityChanged;
        public event Action StormEnded;

        public bool IsActive { get; private set; }
        public float Intensity { get; private set; }

        private float _elapsed;
        private bool _warningRaised;

        private void Start()
        {
            if (_beginOnStart) BeginStorm();
        }

        public void BeginStorm()
        {
            IsActive = true;
            _elapsed = 0f;
            _warningRaised = false;
            if (_windLoop != null)
            {
                _windLoop.volume = 0f;
                if (!_windLoop.isPlaying) _windLoop.Play();
            }
        }

        public void EndStorm()
        {
            if (!IsActive) return;
            IsActive = false;
            Intensity = 0f;
            PublishClearAir();
            if (_windLoop != null) _windLoop.Stop();
            IntensityChanged?.Invoke(0f);
            StormEnded?.Invoke();
        }

        private void Update()
        {
            if (!IsActive) return;

            _elapsed += Time.deltaTime;
            if (_elapsed >= _stormDurationSeconds)
            {
                EndStorm();
                return;
            }

            Intensity = Mathf.Clamp01(_intensityOverTime.Evaluate(_elapsed / _stormDurationSeconds));
            CurrentSightMultiplier = Mathf.Lerp(1f, _sightMultiplierAtPeak, Intensity);
            CurrentWind = transform.forward * (_peakWindSpeed * Intensity);

            if (!_warningRaised && Intensity >= _warningThreshold)
            {
                _warningRaised = true;
                GameEvents.RaiseObjectiveUpdated(_onsetWarning);
            }

            if (_windLoop != null) _windLoop.volume = Intensity;
            IntensityChanged?.Invoke(Intensity);
        }

        // One storm director runs per chapter; a disabled or unloaded director
        // must not leave the world permanently blind.
        private void OnDisable() => PublishClearAir();

        private static void PublishClearAir()
        {
            CurrentSightMultiplier = 1f;
            CurrentWind = Vector3.zero;
        }
    }
}
