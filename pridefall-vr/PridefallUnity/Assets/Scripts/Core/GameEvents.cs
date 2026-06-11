using System;
using UnityEngine;

namespace Pridefall.Core
{
    /// <summary>
    /// Global event bus. Systems publish here instead of holding direct
    /// references across assemblies (UI, audio, save, enemies).
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Player health segment changed: (currentSegments, maxSegments, activeSegmentFill 0-1).</summary>
        public static event Action<int, int, float> PlayerHealthChanged;

        /// <summary>Player died at a world position.</summary>
        public static event Action<Vector3> PlayerDied;

        /// <summary>Player respawned at the latest checkpoint.</summary>
        public static event Action PlayerRespawned;

        /// <summary>A checkpoint was committed (position, checkpoint id).</summary>
        public static event Action<Vector3, string> CheckpointReached;

        /// <summary>An enemy died: (enemy GameObject, scrap dropped).</summary>
        public static event Action<GameObject, int> EnemyKilled;

        /// <summary>Objective text updated for the wrist HUD.</summary>
        public static event Action<string> ObjectiveUpdated;

        /// <summary>Player submerged state changed (true = head underwater).</summary>
        public static event Action<bool> PlayerSubmergedChanged;

        /// <summary>Remaining air while submerged, 0-1.</summary>
        public static event Action<float> PlayerAirChanged;

        /// <summary>Combat intensity hint for the music director, 0 (calm) to 1 (boss).</summary>
        public static event Action<float> CombatIntensityChanged;

        public static void RaisePlayerHealthChanged(int current, int max, float fill) => PlayerHealthChanged?.Invoke(current, max, fill);
        public static void RaisePlayerDied(Vector3 position) => PlayerDied?.Invoke(position);
        public static void RaisePlayerRespawned() => PlayerRespawned?.Invoke();
        public static void RaiseCheckpointReached(Vector3 position, string id) => CheckpointReached?.Invoke(position, id);
        public static void RaiseEnemyKilled(GameObject enemy, int scrap) => EnemyKilled?.Invoke(enemy, scrap);
        public static void RaiseObjectiveUpdated(string text) => ObjectiveUpdated?.Invoke(text);
        public static void RaisePlayerSubmergedChanged(bool submerged) => PlayerSubmergedChanged?.Invoke(submerged);
        public static void RaisePlayerAirChanged(float normalized) => PlayerAirChanged?.Invoke(normalized);
        public static void RaiseCombatIntensityChanged(float intensity) => CombatIntensityChanged?.Invoke(intensity);
    }
}
