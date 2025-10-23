using Cysharp.Threading.Tasks;
using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Rendering;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class SceneModule : MykMykModule
{
    public TestContext testContext;

    public BaseObjectContext testVehiclePrefabScript;
    public CameraRig camRig;
    public FakeRespawn respawner;

    public void OnValidate()
    {
        if (testVehiclePrefabScript is not ISelectableVehicle)
        {
            testVehiclePrefabScript = null;
        }
    }

    public enum TestContext
    {
        None,
        VehicleDeathmatch,
        KaijuDeathMatch,
    }

    protected override void OnInstall(Context context)
    {
        Debug.Log($"Mykmyk scene install {context.name} {context.GetType().Name} during {testContext}", context);
    }

    public void Start()
    {
        Debug.Log($"Scene start {testContext}");
        if (testContext == TestContext.VehicleDeathmatch)
        {
            if (testVehiclePrefabScript is ISelectableVehicle selVel)
            {
                var my = Container.Resolve<MeYou>();
                var resp = my.Container.Resolve<FakeMeYouRespawner>();
                Debug.Log($"legit test {selVel} for {my} using {resp}");
                if (resp)
                {
                    var veh = resp.Respawn(selVel);
                    Health hp = veh.Container.Resolve<Health>();
                    ChainRespawn(hp, resp, selVel, veh);
                }
            }
        }
    }

    public static void ChainRespawn(Health hp, FakeMeYouRespawner resp, ISelectableVehicle selVel, IVehicle veh)
    {
        hp.OnEvent.Where(healthEv => healthEv.Type == Health.EventType.Died).Subscribe(() =>
        {
            UniTask.Delay(TimeSpan.FromSeconds(1)).ContinueWith(() =>
            {
                var newv = resp.Respawn(selVel);
                var newhp = newv.Container.Resolve<Health>();
                ChainRespawn(newhp, resp, selVel, newv);
                return newv;
            });
        }).AddTo(veh);
    }
}
