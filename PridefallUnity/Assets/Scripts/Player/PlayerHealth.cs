using UnityEngine;
using Pridefall.Core;

namespace Pridefall.Player
{
    /// <summary>
    /// Three-segment health. The active (partial) segment regenerates after
    /// a delay; fully lost segments only come back via medgel applied to the
    /// wrist port. Death hands off to GameManager for checkpoint respawn.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _maxSegments = 3;
        [SerializeField] private float _segmentCapacity = 35f;
        [SerializeField] private float _regenDelay = 4f;
        [SerializeField] private float _regenPerSecond = 10f;

        public bool IsAlive { get; private set; } = true;
        public int FullSegments { get; private set; }
        public float ActiveSegmentFill { get; private set; }

        private float _lastDamageTime;

        private void Awake()
        {
            FullSegments = _maxSegments - 1;
            ActiveSegmentFill = 1f;
            RaiseChanged();
        }

        public void TakeDamage(in DamageInfo info)
        {
            if (!IsAlive) return;

            float remaining = info.Amount / _segmentCapacity;
            _lastDamageTime = Time.time;

            ActiveSegmentFill -= remaining;
            while (ActiveSegmentFill <= 0f)
            {
                if (FullSegments > 0)
                {
                    FullSegments--;
                    ActiveSegmentFill += 1f;
                }
                else
                {
                    ActiveSegmentFill = 0f;
                    Die();
                    break;
                }
            }
            RaiseChanged();
        }

        /// <summary>Medgel restores one full segment. Called by the medgel applicator.</summary>
        public bool ApplyMedgel()
        {
            if (!IsAlive) return false;
            if (FullSegments >= _maxSegments - 1 && ActiveSegmentFill >= 1f) return false;

            if (ActiveSegmentFill < 1f)
            {
                ActiveSegmentFill = 1f;
            }
            else
            {
                FullSegments = Mathf.Min(FullSegments + 1, _maxSegments - 1);
            }
            RaiseChanged();
            return true;
        }

        private void Update()
        {
            if (!IsAlive || ActiveSegmentFill >= 1f) return;
            if (Time.time - _lastDamageTime < _regenDelay) return;

            ActiveSegmentFill = Mathf.Min(1f, ActiveSegmentFill + (_regenPerSecond / _segmentCapacity) * Time.deltaTime);
            RaiseChanged();
        }

        private void Die()
        {
            IsAlive = false;
            GameEvents.RaisePlayerDied(transform.position);
        }

        private void OnEnable() => GameEvents.PlayerRespawned += OnRespawned;
        private void OnDisable() => GameEvents.PlayerRespawned -= OnRespawned;

        private void OnRespawned()
        {
            IsAlive = true;
            FullSegments = _maxSegments - 1;
            ActiveSegmentFill = 1f;
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            GameEvents.RaisePlayerHealthChanged(FullSegments, _maxSegments, ActiveSegmentFill);
        }
    }
}
