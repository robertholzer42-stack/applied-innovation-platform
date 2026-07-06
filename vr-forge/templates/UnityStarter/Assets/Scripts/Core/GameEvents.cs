using System;

namespace VRForge.Core
{
    /// <summary>
    /// Global event bus. Systems publish here instead of holding direct
    /// references across assemblies (UI, audio, save, enemies). Keep it
    /// small: add an event only when two otherwise-unrelated systems need
    /// the same signal, and keep payloads to plain values.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Player health changed: (current, max).</summary>
        public static event Action<float, float> PlayerHealthChanged;

        /// <summary>Objective text updated for the wrist or world-space HUD.</summary>
        public static event Action<string> ObjectiveUpdated;

        /// <summary>A scene transition was requested by scene name. The scene loader owns the fade and load.</summary>
        public static event Action<string> SceneTransitionRequested;

        public static void RaisePlayerHealthChanged(float current, float max) => PlayerHealthChanged?.Invoke(current, max);
        public static void RaiseObjectiveUpdated(string text) => ObjectiveUpdated?.Invoke(text);
        public static void RaiseSceneTransitionRequested(string sceneName) => SceneTransitionRequested?.Invoke(sceneName);
    }
}
