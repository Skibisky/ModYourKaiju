using ModestTree;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModYourKaiju.Plugin;

public class LobbyHack : MonoBehaviour
{
    public bool didHack = false;

    [ComponentConstructor]
    public void Construct(LobbySceneContent content, GameplayLevelLoadService loadLevel)
    {
        try
        {
            didHack = true;
            var allVehicleControls = content.gameObject.GetComponentsInChildren<VehicleSelectionControl>();
            Plugin.Logger.LogInfo($"beginning the lobby hack");

            foreach (var vctrl in allVehicleControls)
            {
                var myplayer = vctrl.GetComponentInParent<LobbyMeYouContainer>();
                vctrl.Options.AddRange(DepartmentOfMotorVehicles.RegisteredOptions);

                Plugin.Logger.LogInfo($"{myplayer.gameObject.name} has {vctrl.Options.Count} options");
            }

            var levelControls = content.gameObject.GetComponentInChildren<LevelSelectionControl>();
            var levelSelector = levelControls.transform.parent.GetComponent<SharedAssignableControl>();
            levelSelector.IsReservable = true;
            levelSelector.RemoveInnerControl(levelControls);
            levelSelector.AddInnerControl(levelControls);
            levelControls.Options.AddRange(RealestateOffice.GetSelectOptions());
            var levelCount = CheckLevels(loadLevel);
            GameplayLevelLoaderServicePatch.targetLevel = levelControls.SelectedOption.Title;
            Plugin.Logger.LogInfo($"levels has {levelControls.Options.Count} options, loader has {levelCount}");
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e.ToString());
        }
    }

    protected int CheckLevels(GameplayLevelLoadService loader)
    {
        var ff = typeof(GameplayLevelLoadService).GetField("_gameplaySceneReferences", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var theList = (List<GameplaySceneReference>)ff.GetValue(loader);
        foreach (var r in RealestateOffice.GetReferences())
        {
            if (theList.All(i => i.Name != r.Name))
                theList.Add(r);
        }
        return theList.Count;
    }

    public void Start()
    {
        Plugin.Logger.LogInfo($"hacked? {didHack}");
    }

}
