using UnityEngine;

namespace Pridefall.Core
{
    public enum DamageType
    {
        Kinetic,
        Energy,
        Thermal,
        Corrosive
    }

    public struct DamageInfo
    {
        public float Amount;
        public DamageType Type;
        public Vector3 Point;
        public Vector3 Direction;
        public GameObject Source;

        public DamageInfo(float amount, DamageType type, Vector3 point, Vector3 direction, GameObject source)
        {
            Amount = amount;
            Type = type;
            Point = point;
            Direction = direction;
            Source = source;
        }
    }

    /// <summary>
    /// Anything that can be shot, clawed, burned, or dissolved.
    /// Implemented by PlayerHealth and EnemyBase.
    /// </summary>
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(in DamageInfo info);
    }
}
