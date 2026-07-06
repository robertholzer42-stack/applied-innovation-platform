using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pridefall.UI
{
    /// <summary>
    /// Fullscreen fade on a camera-space canvas, used by teleport blinks and
    /// death/respawn transitions. Kept separate from ComfortVignette because
    /// a blink must reach full black regardless of the player's vignette
    /// setting (it hides a discontinuity rather than damping vection).
    /// </summary>
    public class ScreenFade : MonoBehaviour
    {
        public static ScreenFade Instance { get; private set; }

        [Tooltip("Fullscreen black Image, alpha 0 at rest.")]
        [SerializeField] private Image _overlay;
        [Tooltip("Seconds for each half of a blink (out, then in).")]
        [SerializeField] private float _halfDuration = 0.12f;

        public bool IsFading { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Fade to black, invoke the action at full black, fade back in.
        /// The action is where the caller moves the camera/rig.
        /// </summary>
        public void Blink(Action atBlack)
        {
            if (IsFading)
            {
                atBlack?.Invoke(); // never drop a rig move because a fade was busy
                return;
            }
            StartCoroutine(BlinkRoutine(atBlack));
        }

        private System.Collections.IEnumerator BlinkRoutine(Action atBlack)
        {
            IsFading = true;
            yield return FadeTo(1f);
            atBlack?.Invoke();
            yield return FadeTo(0f);
            IsFading = false;
        }

        private System.Collections.IEnumerator FadeTo(float target)
        {
            if (_overlay == null) yield break;

            float start = _overlay.color.a;
            float elapsed = 0f;
            while (elapsed < _halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetAlpha(Mathf.Lerp(start, target, elapsed / _halfDuration));
                yield return null;
            }
            SetAlpha(target);
        }

        private void SetAlpha(float alpha)
        {
            Color color = _overlay.color;
            color.a = alpha;
            _overlay.color = color;
        }
    }
}
