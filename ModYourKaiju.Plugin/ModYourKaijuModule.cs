using Runtime.Deathmatch;
using Sentient.MeYouKaiju;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace ModYourKaiju.Plugin;

public class ModYourKaijuModule : MykMykModule
{
    protected override void OnInstall(Context context)
    {
        Plugin.Logger.LogInfo($"install {context.name} {context.GetType().Name} with already {Container.AllContracts.Count()} contracts + {Container.AllProviders.Count()} providers!");
        if (context is ProjectContext pc)
        {
            Add<Debug_SetInitialGameState>(GameState.Title);
            Plugin.vehiclePatch = new VehicleRepositoryPatch();
            Plugin.levelPatch = new GameplayLevelLoaderServicePatch();
            try
            {
                Plugin.vehiclePatch.Patch();
                Plugin.levelPatch.Patch();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError(ex.ToString());
            }
        }
        if (context is Lobby lb)
        {
            Container.BindInterfacesAndSelfTo<LobbyHack>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
        if (context is Deathmatch and ITier1)
        {
            Add<RequestGameplayLevelScene>();
        }
    }
}
