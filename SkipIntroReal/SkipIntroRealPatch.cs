using HarmonyLib;
using RoR2;
using RoR2.Networking;
using RoR2.UI.LogBook;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SkipIntroReal;

public static class SkipIntroRealPatch
{
    public static void Init()
    {
        //var locations = Addressables.LoadResourceLocationsAsync("title", typeof(Object)).WaitForCompletion();
        //Plugin.Logger.LogMessage($"title has {locations.Count} preload locations");

        Plugin.Harmony.PatchAll(typeof(SkipIntroRealPatch));
        RoR2Application.onLoad += OnLoad;

        static void OnLoad()
        {
            Plugin.Logger.LogMessage("Start coroutine");
            RoR2Application.instance.StartCoroutine(SetTitleSceneCoroutine());

            static IEnumerator SetTitleSceneCoroutine()
            {
                yield return null;
                //yield return new WaitForSeconds(3);
                Plugin.Logger.LogMessage("submit command!!");
                //RoR2.Console.instance.SubmitCmd(null, "set_scene title");
                NetworkManager.singleton.ServerChangeScene("title");
            }
        }
    }

#if true
    [HarmonyPrefix, HarmonyPatch(typeof(LoadHelper), nameof(LoadHelper.LoadSceneAdditivelyAsync))]
    static bool SkipSplashScene(string _sceneGuid, ref IEnumerator __result)
    {
#if false
        if (_sceneGuid == RoR2_Base_splash.splash_unity && RoR2Application.isLoading)
        {
            static IEnumerator FinishSplashScreen()
            {
                MusicController.Instance.StartIntroMusic();
                yield break;
            }

            __result = FinishSplashScreen();
            return false;
        }
#endif
        if (_sceneGuid == RoR2_Base_intro.intro_unity && RoR2Application.isLoading)
        {
            static IEnumerator FinishSplashScreen()
            {
                yield break;
            }

            __result = FinishSplashScreen();
            return false;
        }
        return true;
    }
#endif

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