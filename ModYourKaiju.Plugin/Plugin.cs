using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine.VFX;
using Zenject;

namespace ModYourKaiju.Plugin;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Me, You and Kaiju.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    static int HasShimmed = 0;

    public static VehicleRepositoryPatch vehiclePatch;

    private List<IMykMod> loadedMods = new();

    private List<Type> amcLoaders = new();


    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded! in {Environment.CurrentDirectory}");

        BepLog.OnLog += (s, e) => Logger.LogInfo(e);

        try
        {
            var find = Directory.GetFiles("BepInEx\\plugins\\Mykmyk", "*.dll", SearchOption.AllDirectories);
            foreach (var f in find)
            {
                Logger.LogInfo($"loading mod {f}");
                try
                {
                    var name = AssemblyName.GetAssemblyName(f);
                    using var def = AssemblyDefinition.ReadAssembly(f);
                    // todo: check its a real mod or something?

                    var mmm = Assembly.Load(File.ReadAllBytes(f));
                    var mlts = mmm.GetTypes().Where(t => typeof(IMykMod).IsAssignableFrom(t));
                    if (!mlts.Any())
                    {
                        Logger.LogWarning($"No IMykMod in {f}");
                    }

                    foreach (var mlt in mlts)
                    {
                        var build = (IMykMod)Activator.CreateInstance(mlt);
                        loadedMods.Add(build);
                    }

                    var amcs = mmm.GetTypes().Where(t => typeof(MykMykModule).IsAssignableFrom(t));
                    if (!amcs.Any())
                    {
                        Logger.LogWarning($"No MykMykModules in {f}");
                    }

                    foreach (var tt in amcs)
                    {
                        amcLoaders.Add(tt);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }

            ProjectContext.PreInstall += Shim;
            var theCtx = ProjectContext.Instance;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }
    }

    private void Shim()
    {
        Logger.LogInfo($"Shim at {DateTime.Now}");
        if (HasShimmed >= 2)
        {
            Logger.LogInfo("Already hacked");
            return;
        }

        HasShimmed++;
        try
        {
            var theCtx = ProjectContext.Instance;

            var hacks = theCtx.GetOrAddComponent<ModYourKaijuModule>();

            var cfg = theCtx.GetComponent<ModuleConfigurer>();
            cfg.BehaviourModules.Add(hacks);

            Logger.LogInfo($"mods: {cfg.AssetModules.Count}");

            foreach (var am in cfg.AssetModules)
            {
                if (am is BaseModuleAsset bm)
                    Search(bm);
                else
                    Investigate(am);
            }

            foreach (var lm in loadedMods)
            {
                Logger.LogInfo($"Configuring {lm.GetType().Name}");
                lm.Configure();
            }

            var inst = theCtx.GetComponent<ModulesInstaller>();
            theCtx.Container.RegisterModule(hacks);

            foreach (var mmm in amcLoaders)
            {
                if (!theCtx.gameObject.TryGetComponent(mmm, out var m))
                    m = (MykMykModule)theCtx.gameObject.AddComponent(mmm);
                Logger.LogInfo($"Registering {m.GetType().Name}");
                theCtx.Container.RegisterModule((MykMykModule)m);
            }

        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }
    }

    private void Investigate(AbstractModuleAsset ama)
    {
        if (ama is BaseModuleAsset bma)
            Logger.LogInfo($"{bma.name} +{bma.SubInstallers.Count} subs");
        else
            Logger.LogInfo($"{ama.name}");

        if (ama is MeYouKaijuModule mykm)
        {
            MykAssets.heliDamEfArgs = mykm.HelicopterDamageEffects;
            MykAssets.heliDamAudio = mykm.HelicopterDamageAudioConfig;
            MykAssets.heliGun = mykm.MiniGun;
        }
        if (ama is Myk_CodeGen_Module mykCodeGen)
        {
            var pms = mykCodeGen.DronePrefab.gameObject.GetComponentInChildren<PlayerMarkerSlot>(true);
            var ar = typeof(PlayerMarkerSlot).GetField("_args", BindingFlags.NonPublic | BindingFlags.Instance);
            MykAssets.PlayerMarkerSlotArgs = (PlayerMarkerSlot.Args)ar.GetValue(pms);

            var exh = mykCodeGen.DronePrefab.gameObject.GetComponentInChildren<VisualEffect>(true);
            MykAssets.DefaultVisualEffect = exh.visualEffectAsset;
        }

    }

    private void Search(BaseModuleAsset bma)
    {
        Investigate(bma);

        foreach (var si in bma.SubInstallers)
        {
            if (si is BaseModuleAsset sma)
                Search(sma);
            else
                Investigate(si);
        }
    }
}
