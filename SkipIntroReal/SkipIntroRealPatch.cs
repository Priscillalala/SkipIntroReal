using HarmonyLib;
using RoR2;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SkipIntroReal;

public static class SkipIntroRealPatch
{
    public static void Init()
    {
        Plugin.Harmony.PatchAll(typeof(SkipIntroRealPatch));
        RoR2Application.onLoad += OnLoad;

        // Usually IntroCutsceneController.Finish is responsible for transitioning to the title scene, so we have to do it ourselves
        static void OnLoad()
        {
            RoR2Application.instance.StartCoroutine(TransitionToTitleCoroutine());

            // Wait a frame to let the other onLoad subscribers run, because some do asset loading
            // Also lets the logbook finish building, apparently - the timing is pretty confusing
            static IEnumerator TransitionToTitleCoroutine()
            {
                yield return null;

                // Starts the music system for the whole game
                // Usually TriggerIntroMusicStart does this in the splash scene
                MusicController.Instance.StartIntroMusic(); 

                NetworkManager.singleton.ServerChangeScene("title");
            }
        }
    }

    // called from RoR2Application.InitializeGameRoutine
    [HarmonyPrefix, HarmonyPatch(typeof(LoadHelper), nameof(LoadHelper.LoadSceneAdditivelyAsync))]
    static bool SkipSplashScene(string _sceneGuid, ref IEnumerator __result)
    {
        if (_sceneGuid == RoR2_Base_splash.splash_unity && RoR2Application.isLoading)
        {
            __result = FakeLoadSplashCoroutine();
            return false;

            static IEnumerator FakeLoadSplashCoroutine()
            {
                // because the splash scene is never loaded, we must set this manually, or scene transitions break
                FadeToBlackManager.haveLoadedSplash = true;
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