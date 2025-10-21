using Sentient.Injection;
using Sentient.MeYouKaiju;
using System;
using UnityEngine;

namespace ModYourKaiju.Plugin;

public class LobbyHack : MonoBehaviour
{
    public bool didHack = false;

    [ComponentConstructor]
    public void Construct(LobbySceneContent content)
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

        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e.ToString());
        }
    }

    public void Start()
    {
        Plugin.Logger.LogInfo($"hacked? {didHack}");
    }

}
