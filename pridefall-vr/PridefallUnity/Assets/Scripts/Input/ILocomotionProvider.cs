using UnityEngine;

namespace Pridefall.Input
{
    /// <summary>
    /// Abstraction over what drives ground locomotion. The shipping
    /// implementation is OmniOneLocomotionProvider (Virtuix Omni One
    /// treadmill). SimulatedLocomotionProvider drives the editor, and a
    /// thumbstick provider exists as a labeled accessibility fallback.
    ///
    /// Convention: yaw angles are degrees around world Y, relative to the
    /// play space's forward axis at calibration time.
    /// </summary>
    public interface ILocomotionProvider
    {
        /// <summary>Hardware present, tracking, and calibrated.</summary>
        bool IsActive { get; }

        /// <summary>
        /// True when ground motion is artificial (thumbstick/simulated) and
        /// therefore vection-inducing; false when the player's real legs
        /// drive it (treadmill). Comfort systems escalate on artificial
        /// motion automatically.
        /// </summary>
        bool IsArtificial { get; }

        /// <summary>
        /// Direction the player's body (Omni ring) faces, independent of the
        /// headset. Weapons holster to this; the HMD never re-aims the hips.
        /// </summary>
        float BodyYawDegrees { get; }

        /// <summary>
        /// Direction of travel. On the treadmill this usually equals
        /// BodyYawDegrees, but backpedaling reports +180.
        /// </summary>
        float StrideYawDegrees { get; }

        /// <summary>Gait speed in m/s, already smoothed, >= 0.</summary>
        float GaitSpeed { get; }

        /// <summary>True on the frame a physical or assisted jump triggers.</summary>
        bool ConsumeJump();

        /// <summary>Begin/refresh calibration (diegetic "suit fitting").</summary>
        void Calibrate();
    }
}
