using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRForge.Editor
{
    /// <summary>
    /// Batchmode build entry points for Quest APKs, invoked by
    /// scripts/build-quest.sh via -executeMethod. Configures Android,
    /// IL2CPP, ARM64, Vulkan, and ASTC; collects enabled scenes; writes
    /// Builds/quest.apk. Exits the editor with a nonzero code on any
    /// failure so shell scripts can gate on the result.
    /// </summary>
    public static class BuildQuest
    {
        private const string OutputPath = "Builds/quest.apk";
        private const string DefaultAppId = "com.vrforge.game";

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
                    Debug.LogError("[BuildQuest] Build Failed: no enabled scenes in Build Settings. Add the starter scene first (File > Build Settings).");
                    EditorApplication.Exit(1);
                    return;
                }

                string outputDir = Path.GetDirectoryName(OutputPath);
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                var buildOptions = new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = OutputPath,
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
                Debug.Log($"[BuildQuest] Build succeeded: {OutputPath} ({sizeMb:F1} MB, {scenes.Length} scene(s)).");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("[BuildQuest] Build Failed with exception: " + exception);
                EditorApplication.Exit(1);
            }
        }

        private static void ConfigurePlayerSettings()
        {
            string appId = Environment.GetEnvironmentVariable("VRFORGE_APP_ID");
            if (string.IsNullOrWhiteSpace(appId))
            {
                appId = DefaultAppId;
            }

            string productName = Environment.GetEnvironmentVariable("VRFORGE_PRODUCT_NAME");
            if (!string.IsNullOrWhiteSpace(productName))
            {
                PlayerSettings.productName = productName;
            }

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, appId);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

            Debug.Log($"[BuildQuest] Configured Android build: appId={appId}, IL2CPP, ARM64, Vulkan, ASTC.");
        }
    }
}
