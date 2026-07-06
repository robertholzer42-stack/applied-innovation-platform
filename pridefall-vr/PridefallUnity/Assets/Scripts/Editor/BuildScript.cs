using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Pridefall.Editor
{
    /// <summary>
    /// Batchmode build entry points for the PRIDEFALL Quest APK, invoked by
    /// scripts/build-quest.sh via -executeMethod. Configures Android,
    /// IL2CPP, ARM64, Vulkan, ASTC, Linear color space, and minSdk 29;
    /// collects enabled scenes; writes Builds/quest.apk inside the Unity
    /// project folder. Exits the editor with a nonzero code on any failure
    /// so shell scripts and hooks can gate on the result.
    /// </summary>
    public static class BuildQuest
    {
        private const string DefaultAppId = "com.meridian.pridefall";
        private const string ProductName = "PRIDEFALL";

        /// <summary>APK path anchored to the project folder, independent of the process working directory.</summary>
        private static string OutputPath =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Builds", "quest.apk"));

        /// <summary>Development build: script debugging, profiler connectable.</summary>
        public static void Development()
        {
            Run(BuildOptions.Development | BuildOptions.AllowDebugging);
        }

        /// <summary>Release build: no debugging, ship configuration.</summary>
        public static void Release()
        {
            Run(BuildOptions.None);
        }

        /// <summary>Shared build body: target switch, player settings, scene collection, build, exit code.</summary>
        private static void Run(BuildOptions options)
        {
            try
            {
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                {
                    Debug.Log("[BuildQuest] Switching active build target to Android.");
                    if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                    {
                        Debug.LogError("[BuildQuest] Build Failed: could not switch to Android. Is Android Build Support installed?");
                        EditorApplication.Exit(1);
                        return;
                    }
                }

                ConfigurePlayerSettings();

                string[] scenes = EditorBuildSettings.scenes
                    .Where(scene => scene != null && scene.enabled)
                    .Select(scene => scene.path)
                    .ToArray();
                if (scenes.Length == 0)
                {
                    Debug.LogError("[BuildQuest] Build Failed: no enabled scenes in Build Settings. Add a scene first (File > Build Settings); see Assets/Scripts/README-scene-setup.md.");
                    EditorApplication.Exit(1);
                    return;
                }

                string outputPath = OutputPath;
                string outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                var buildOptions = new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = outputPath,
                    target = BuildTarget.Android,
                    targetGroup = BuildTargetGroup.Android,
                    options = options
                };

                BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
                if (report == null || report.summary.result != BuildResult.Succeeded)
                {
                    string result = report != null ? report.summary.result.ToString() : "no report";
                    int errors = report != null ? report.summary.totalErrors : 0;
                    Debug.LogError($"[BuildQuest] Build Failed: result={result}, errors={errors}. See the log above for the first error.");
                    EditorApplication.Exit(1);
                    return;
                }

                double sizeMb = report.summary.totalSize / (1024.0 * 1024.0);
                Debug.Log($"[BuildQuest] Build succeeded: {outputPath} ({sizeMb:F1} MB, {scenes.Length} scene(s)).");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("[BuildQuest] Build Failed with exception: " + exception);
                EditorApplication.Exit(1);
            }
        }

        /// <summary>Applies the Quest 3 player configuration: app id, IL2CPP, ARM64, Vulkan, ASTC, Linear, minSdk 29.</summary>
        private static void ConfigurePlayerSettings()
        {
            string appId = Environment.GetEnvironmentVariable("VRFORGE_APP_ID");
            if (string.IsNullOrWhiteSpace(appId))
            {
                appId = DefaultAppId;
            }

            PlayerSettings.productName = ProductName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, appId);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

            Debug.Log($"[BuildQuest] Configured Android build: product={ProductName}, appId={appId}, IL2CPP, ARM64, Vulkan, ASTC, Linear, minSdk 29.");
        }
    }
}
