using UnityEngine;

namespace VRForge.Core
{
    /// <summary>Category of damage; drives resistances, hit VFX, and audio selection.</summary>
    public enum DamageType
    {
        Generic = 0,
        Impact = 1,
        Fire = 2,
        Energy = 3
    }

    /// <summary>
    /// Immutable description of a single damage event. Passed by readonly
    /// reference (<c>in</c>) so hot combat paths stay allocation-free.
    /// </summary>
    public readonly struct DamageInfo
    {
        /// <summary>Category of this damage event.</summary>
        public readonly DamageType Type;

        /// <summary>Raw damage amount before resistances, >= 0 by convention.</summary>
        public readonly float Amount;

        /// <summary>World-space hit point, or zero when not applicable.</summary>
        public readonly Vector3 Point;

        /// <summary>World-space direction the damage travelled, or zero when not applicable.</summary>
        public readonly Vector3 Direction;

        /// <summary>Object that dealt the damage; may be null (environmental damage).</summary>
        public readonly GameObject Source;

        public DamageInfo(DamageType type, float amount, Vector3 point, Vector3 direction, GameObject source)
        {
            Type = type;
            Amount = amount;
            Point = point;
            Direction = direction;
            Source = source;
        }

        public DamageInfo(DamageType type, float amount)
            : this(type, amount, Vector3.zero, Vector3.zero, null)
        {
        }

        /// <summary>Amount after applying a 0-1 resistance factor; resistance is clamped to that range.</summary>
        public float AmountAfterResistance(float resistance01)
        {
            return Amount * (1f - Mathf.Clamp01(resistance01));
        }

        /// <summary>Copy with the amount multiplied; negative multipliers clamp to zero.</summary>
        public DamageInfo Scaled(float multiplier)
        {
            return new DamageInfo(Type, Amount * Mathf.Max(0f, multiplier), Point, Direction, Source);
        }
    }

    /// <summary>Anything that can receive damage: player, enemies, destructibles.</summary>
    public interface IDamageable
    {
        /// <summary>Apply one damage event. Implementations must tolerate repeated calls after death.</summary>
        void TakeDamage(in DamageInfo info);
    }
}
