#if DEBUG
using HarmonyLib;
using RoR2;
using RoR2.Networking;
using RoR2.UI.LogBook;
using RoR2.UI.MainMenu;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.PlayerLoop;
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

    [HarmonyPrefix, HarmonyPatch(typeof(TransitionCommand), nameof(TransitionCommand.CCTransitionCommand))]
    static void DebugCCTransitionCommand(ConCommandArgs args)
    {
        Plugin.Logger.LogWarning($"CCTransitionCommand: request pending? {TransitionCommand.requestPending}, command: {args[0]}");
        RoR2Application.onUpdate += LogUpdateOnce;

        static void LogUpdateOnce()
        {
            Plugin.Logger.LogMessage($"update ran");
            RoR2Application.onUpdate -= LogUpdateOnce;
        }
    }

#if false
    [HarmonyPostfix, HarmonyPatch(typeof(TitleMenuController), nameof(TitleMenuController.CanTransitionOutOfTitleMenu))]
    static void DebugCanTransitionOutOfTitleMenu(ref bool __result)
    {
        Plugin.Logger.LogWarning($"CanTransitionOutOfTitleMenu: {__result}");
        Plugin.Logger.LogWarning($"RoR2Application.IsMainMenuInitialized: {RoR2Application.IsMainMenuInitialized}");
        Plugin.Logger.LogWarning($"RoR2Application.IsEntitlementsUpdated: {RoR2Application.IsEntitlementsUpdated}");
        Plugin.Logger.LogWarning($"!FadeToBlackManager.IsFading(): {!FadeToBlackManager.IsFading()}");
    }
#endif
}
#endif