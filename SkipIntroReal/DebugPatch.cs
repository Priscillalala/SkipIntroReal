#if DEBUG
using HarmonyLib;
using RoR2;
using RoR2.Networking;
using RoR2.UI.LogBook;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace SkipIntroReal;

public static class DebugPatch
{
    static readonly Stopwatch loadStopwatch = new();

    public static void Init()
    {
        Plugin.Harmony.PatchAll(typeof(DebugPatch));
        loadStopwatch.Start();

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "title")
        {
            Plugin.Logger.LogWarning($"loaded into title in {loadStopwatch.ElapsedMilliseconds}ms");
        }
    }

    [HarmonyPrefix, HarmonyPatch(typeof(PreloadContent), nameof(PreloadContent.Preload))]
    static void OnPreloadContent(PreloadContent __instance)
    {
        Plugin.Logger.LogWarning($"PreloadContent: is scene? {__instance.preloadScene} scene name: {__instance.preloadSceneName}");
    }
}
#endif