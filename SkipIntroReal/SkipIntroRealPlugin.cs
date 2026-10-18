global using Plugin = SkipIntroReal.SkipIntroRealPlugin;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SkipIntroReal;

[BepInPlugin(GUID, NAME, VERSION)]
public class SkipIntroRealPlugin : BaseUnityPlugin
{
    public const string
        GUID = "groovesalad." + NAME,
        NAME = "SkipIntroReal",
        VERSION = "1.0.0";

    public static new ManualLogSource Logger { get; private set; }
    public static Harmony Harmony { get; private set; }

    void Awake()
    {
        Logger = base.Logger;
        Harmony = new Harmony(GUID);

        SkipIntroRealPatch.Init();
#if DEBUG
        DebugPatch.Init();
#endif
    }
}
