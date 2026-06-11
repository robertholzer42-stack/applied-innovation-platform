using UnityEngine;
using UnityEngine.UI;
using Pridefall.Core;
using Pridefall.Player;

namespace Pridefall.UI
{
    /// <summary>
    /// Diegetic wrist display on the left forearm, Hubris-style: health
    /// segment pips, an air bar that only exists underwater, objective
    /// toasts, and the scrap/bio-resin wallet (polled at 2 Hz). The whole
    /// canvas fades in only while the wrist is rotated toward the face, so
    /// it never clutters combat.
    /// </summary>
    public class WristHud : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] private PlayerRig _rig;
        [Tooltip("Forearm anchor the canvas sits on. Its up axis must point out of the top of the wrist.")]
        [SerializeField] private Transform _wristAnchor;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Visibility")]
        [Tooltip("Dot of wrist-up against the reversed head-forward. Higher = the wrist must turn further toward the eyes.")]
        [SerializeField, Range(0f, 1f)] private float _faceDotThreshold = 0.55f;
        [SerializeField] private float _fadeSpeed = 8f;

        [Header("Health")]
        [Tooltip("One pip per health segment, left to right. Image type must be Filled.")]
        [SerializeField] private Image[] _healthPips;

        [Header("Air")]
        [Tooltip("Parent of the air bar; active only while the head is underwater.")]
        [SerializeField] private GameObject _airGroup;
        [Tooltip("Image type must be Filled.")]
        [SerializeField] private Image _airFill;

        [Header("Objective Toast")]
        [SerializeField] private Text _toastText;
        [SerializeField] private float _toastHoldSeconds = 5f;
        [SerializeField] private float _toastFadeSeconds = 1.5f;

        [Header("Resources")]
        [SerializeField] private Text _resourceText;

        private const float ResourcePollInterval = 0.5f; // 2 Hz; wallets don't need frame-rate updates

        private float _toastTimer;
        private float _resourcePollTimer;
        private int _lastScrap = -1;
        private int _lastBioResin = -1;

        private void Awake()
        {
            if (_rig == null) _rig = GetComponentInParent<PlayerRig>();
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_airGroup != null) _airGroup.SetActive(false);
            if (_toastText != null) _toastText.text = string.Empty;
        }

        private void OnEnable()
        {
            GameEvents.PlayerHealthChanged += OnHealthChanged;
            GameEvents.PlayerAirChanged += OnAirChanged;
            GameEvents.PlayerSubmergedChanged += OnSubmergedChanged;
            GameEvents.ObjectiveUpdated += OnObjectiveUpdated;
        }

        private void OnDisable()
        {
            GameEvents.PlayerHealthChanged -= OnHealthChanged;
            GameEvents.PlayerAirChanged -= OnAirChanged;
            GameEvents.PlayerSubmergedChanged -= OnSubmergedChanged;
            GameEvents.ObjectiveUpdated -= OnObjectiveUpdated;
        }

        private void Update()
        {
            UpdateVisibility();
            UpdateToast();
            PollResources();
        }

        private void UpdateVisibility()
        {
            if (_canvasGroup == null) return;

            bool facing = false;
            if (_wristAnchor != null && _rig != null && _rig.Head != null)
            {
                // Wrist-up points against the gaze when the player checks their wrist.
                facing = Vector3.Dot(_wristAnchor.up, -_rig.Head.forward) >= _faceDotThreshold;
            }

            float target = facing ? 1f : 0f;
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, _fadeSpeed * Time.deltaTime);
        }

        private void UpdateToast()
        {
            if (_toastText == null || _toastTimer <= 0f) return;

            _toastTimer -= Time.deltaTime;
            float alpha = _toastTimer >= _toastFadeSeconds
                ? 1f
                : Mathf.Clamp01(_toastTimer / _toastFadeSeconds);
            Color color = _toastText.color;
            color.a = alpha;
            _toastText.color = color;
        }

        private void PollResources()
        {
            _resourcePollTimer -= Time.deltaTime;
            if (_resourcePollTimer > 0f) return;
            _resourcePollTimer = ResourcePollInterval;

            if (_resourceText == null || GameManager.Instance == null) return;

            int scrap = GameManager.Instance.Scrap;
            int bioResin = GameManager.Instance.BioResin;
            if (scrap == _lastScrap && bioResin == _lastBioResin) return;

            _lastScrap = scrap;
            _lastBioResin = bioResin;
            // The HUD's only allocation: a string rebuilt when the wallet actually changes.
            _resourceText.text = $"Scrap {scrap}   Resin {bioResin}";
        }

        private void OnHealthChanged(int fullSegments, int maxSegments, float activeFill)
        {
            if (_healthPips == null) return;

            for (int i = 0; i < _healthPips.Length; i++)
            {
                Image pip = _healthPips[i];
                if (pip == null) continue;

                // PlayerHealth reports full segments below the active one, so
                // the pip at index fullSegments shows the regenerating fill.
                if (i < fullSegments) pip.fillAmount = 1f;
                else if (i == fullSegments) pip.fillAmount = activeFill;
                else pip.fillAmount = 0f;
            }
        }

        private void OnAirChanged(float normalized)
        {
            if (_airFill != null) _airFill.fillAmount = normalized;
        }

        private void OnSubmergedChanged(bool submerged)
        {
            if (_airGroup != null) _airGroup.SetActive(submerged);
        }

        private void OnObjectiveUpdated(string text)
        {
            if (_toastText == null) return;

            _toastText.text = text;
            Color color = _toastText.color;
            color.a = 1f;
            _toastText.color = color;
            _toastTimer = _toastHoldSeconds + _toastFadeSeconds;
        }
    }
}
