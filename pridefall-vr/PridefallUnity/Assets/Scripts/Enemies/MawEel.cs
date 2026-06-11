using UnityEngine;
using Pridefall.Core;
using Pridefall.Environment;

namespace Pridefall.Enemies
{
    /// <summary>
    /// Aquatic ambusher for the Mirror Sea kelp beds. Lurks at an anchor
    /// inside a WaterVolume, lunges at swimmers in a fast straight strike,
    /// bites, then circles away on a cooldown before striking again. It never
    /// breaks the surface: position is clamped below SurfaceY minus a margin.
    ///
    /// Activation: listens to GameEvents.PlayerSubmergedChanged rather than
    /// polling "head below SurfaceY" every frame. SwimmingSystem already owns
    /// the submerged definition and the event is edge-triggered, so the eel
    /// does zero work while the player is dry. The event is global across
    /// volumes, so a proximity gate against the anchor keeps an eel from
    /// reacting to a swim in some other pool; a direct SurfaceY test backs it
    /// up for the pooled-spawn-mid-swim case where the edge was missed.
    /// </summary>
    public class MawEel : EnemyBase
    {
        [Header("Territory")]
        [Tooltip("The water body this eel lives in. Auto-resolved from parents when left empty.")]
        [SerializeField] private WaterVolume _water;
        [Tooltip("Lurk position. Falls back to the spawn position.")]
        [SerializeField] private Transform _anchorPoint;
        [SerializeField] private float _engageRadius = 16f;
        [Tooltip("The eel never rises closer to the surface than this.")]
        [SerializeField] private float _surfaceMargin = 0.75f;

        [Header("Movement")]
        [SerializeField] private float _lurkSpeed = 1.5f;
        [SerializeField] private float _strikeSpeed = 11f;
        [SerializeField] private float _circleSpeed = 4.5f;
        [SerializeField] private float _swayRadius = 1.5f;
        [SerializeField] private float _turnResponsiveness = 3f;

        [Header("Bite")]
        [SerializeField] private float _biteDamage = 25f;
        [SerializeField] private float _biteRadius = 1f;
        [Tooltip("Cooldown spent circling between strikes.")]
        [SerializeField] private float _circleSeconds = 3f;
        [SerializeField] private float _circleRadius = 5f;

        private enum StrikePhase { Lunge, Circle }

        private Vector3 _anchorPosition;
        private Vector3 _velocity;
        private StrikePhase _phase;
        private Vector3 _strikeTarget;
        private bool _bit;
        private float _nextStrikeTime;
        private float _circleAngle;
        private bool _playerSwimming;

        protected override void Awake()
        {
            base.Awake();
            if (_water == null) _water = GetComponentInParent<WaterVolume>();
            if (_water == null) Debug.LogWarning($"[MawEel] '{name}' has no WaterVolume; surface clamping is disabled.", this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _anchorPosition = _anchorPoint != null ? _anchorPoint.position : transform.position;
            _velocity = Vector3.zero;
            _playerSwimming = false;
            GameEvents.PlayerSubmergedChanged += OnPlayerSubmergedChanged;
        }

        private void OnDisable()
        {
            GameEvents.PlayerSubmergedChanged -= OnPlayerSubmergedChanged;
        }

        // Editor-time defaults per the GDD creature table.
        private void Reset()
        {
            _maxHealth = 70f;
            _scrapDrop = 0; // fauna pay in bio-resin, not scrap
            _staggerThreshold = 25f;
        }

        private void OnPlayerSubmergedChanged(bool submerged)
        {
            _playerSwimming = submerged;
        }

        protected override void EnterState(EnemyState state)
        {
            base.EnterState(state);
            if (state == EnemyState.Attack) BeginLunge();
        }

        protected override void TickIdle()
        {
            // Lazy figure-eight drift around the anchor.
            Vector3 sway = new Vector3(
                Mathf.Sin(Time.time * 0.6f),
                Mathf.Sin(Time.time * 0.35f) * 0.5f,
                Mathf.Cos(Time.time * 0.5f)) * _swayRadius;
            Vector3 target = ClampBelowSurface(_anchorPosition + sway);
            Vector3 desired = Vector3.ClampMagnitude((target - transform.position) * 1.2f, _lurkSpeed);
            IntegrateSteering(desired, ref _velocity, _turnResponsiveness);
            ClampSelf();

            if (PlayerInTerritory()) ChangeState(EnemyState.Attack);
        }

        protected override void TickAttack()
        {
            if (!PlayerInTerritory()) { ChangeState(EnemyState.Idle); return; }
            Transform head = PlayerHead; // PlayerInTerritory guarantees non-null

            switch (_phase)
            {
                case StrikePhase.Lunge:
                    // Mild homing so a hard kick away still dodges the strike.
                    _strikeTarget = ClampBelowSurface(Vector3.MoveTowards(_strikeTarget, head.position, 3f * Time.deltaTime));
                    Vector3 toTarget = _strikeTarget - transform.position;
                    IntegrateSteering(toTarget.normalized * _strikeSpeed, ref _velocity, _turnResponsiveness * 2f);
                    ClampSelf();

                    if (!_bit && (head.position - transform.position).sqrMagnitude <= _biteRadius * _biteRadius)
                    {
                        _bit = true;
                        TryDamagePlayer(_biteDamage, DamageType.Kinetic, head.position, _velocity.normalized, gameObject);
                        BeginCircle(head.position);
                        return;
                    }

                    if (toTarget.sqrMagnitude < 0.6f || Vector3.Dot(_velocity, toTarget) < 0f) BeginCircle(head.position);
                    break;

                case StrikePhase.Circle:
                    _circleAngle += (_circleSpeed / Mathf.Max(_circleRadius, 0.5f)) * Mathf.Rad2Deg * Time.deltaTime;
                    float rad = _circleAngle * Mathf.Deg2Rad;
                    Vector3 orbit = head.position + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * _circleRadius;
                    orbit = ClampBelowSurface(orbit);
                    orbit.y -= 1f; // circle a little deeper than the swimmer; sets up the rising strike

                    Vector3 desired = Vector3.ClampMagnitude((orbit - transform.position) * 2f, _circleSpeed);
                    IntegrateSteering(desired, ref _velocity, _turnResponsiveness);
                    ClampSelf();

                    if (Time.time >= _nextStrikeTime) BeginLunge();
                    break;
            }
        }

        private void BeginLunge()
        {
            Transform head = PlayerHead;
            if (head == null) { ChangeState(EnemyState.Idle); return; }
            _phase = StrikePhase.Lunge;
            _strikeTarget = ClampBelowSurface(head.position);
            _bit = false;
        }

        private void BeginCircle(Vector3 aroundPoint)
        {
            _phase = StrikePhase.Circle;
            _nextStrikeTime = Time.time + _circleSeconds;
            // Start the orbit from the eel's current bearing so there is no snap.
            Vector3 from = transform.position - aroundPoint;
            _circleAngle = Mathf.Atan2(from.z, from.x) * Mathf.Rad2Deg;
        }

        private bool PlayerInTerritory()
        {
            Transform head = PlayerHead;
            if (head == null || !PlayerIsAlive) return false;
            if ((head.position - _anchorPosition).sqrMagnitude > _engageRadius * _engageRadius) return false;
            // Event is the cheap gate; the SurfaceY test covers an eel enabled mid-swim.
            return _playerSwimming || head.position.y < SurfaceY();
        }

        private float SurfaceY()
        {
            return _water != null ? _water.SurfaceY : float.PositiveInfinity;
        }

        private Vector3 ClampBelowSurface(Vector3 point)
        {
            float ceiling = SurfaceY();
            if (!float.IsPositiveInfinity(ceiling)) point.y = Mathf.Min(point.y, ceiling - _surfaceMargin);
            return point;
        }

        private void ClampSelf()
        {
            transform.position = ClampBelowSurface(transform.position);
        }
    }
}
