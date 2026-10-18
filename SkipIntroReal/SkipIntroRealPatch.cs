using HarmonyLib;
using RoR2;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SkipIntroReal;

// Only skips the intro scene because skipping the splash screen breaks the main menu; don't know why
public static class SkipIntroRealPatch
{
    public static void Init()
    {
        Plugin.Harmony.PatchAll(typeof(SkipIntroRealPatch));
        RoR2Application.onLoad += OnLoad;

        // Usually IntroCutsceneController.Finish is responsible for transitioning to the title scene
        // so we have to do it ourselves
        static void OnLoad()
        {
            RoR2Application.instance.StartCoroutine(TransitionToTitleCoroutine());

            // Wait a frame to let the other onLoad subscribers run, because some load assets
            // Also lets the logbook finish building, apparently - the timing is pretty confusing
            static IEnumerator TransitionToTitleCoroutine()
            {
                yield return null;
                NetworkManager.singleton.ServerChangeScene("title");
            }
        }
    }

    // called from SplashScreenController.Finish
    [HarmonyPrefix, HarmonyPatch(typeof(LoadHelper), nameof(LoadHelper.LoadSceneAdditivelyAsync))]
    static bool SkipIntroScene(string _sceneGuid, ref IEnumerator __result)
    {
        if (_sceneGuid == RoR2_Base_intro.intro_unity)
        {
            __result = EmptyCoroutine();
            return false;

            static IEnumerator EmptyCoroutine()
            {
                yield break;
            }
        }
        return true;
    }

    // called from RoR2Application.InitializeGameRoutine
    [HarmonyPrefix, HarmonyPatch(typeof(LoadHelper), nameof(LoadHelper.StartLoadSceneAdditiveAsync))]
    static bool SkipIntroScene(string _sceneGuid, ref AsyncOperationHandle __result)
    {
        if (_sceneGuid == RoR2_Base_intro.intro_unity && RoR2Application.isLoading)
        {
            __result = default;
            return false;
        }
        return true;
    }
}