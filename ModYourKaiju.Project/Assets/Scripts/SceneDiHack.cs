using Sentient.Injection;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class SceneDiHack : MonoInstaller
{
    public SceneContext sceneContext;
    public bool IsHacked;
    public ModulesInstaller modInstall;


    public SceneModule sceneModule;

    [ContextMenu("check hack")]
    public void CheckHack()
    {
        IsHacked = sceneContext.Installers.Contains(modInstall) && sceneContext.Installers.Contains(this);
    }

    [ContextMenu("do hack")]
    public void DoHack()
    {
        if (modInstall)
            sceneContext.Installers = sceneContext.Installers.Union(new MonoInstaller[] {this, modInstall  });
        CheckHack();
    }

    public override void InstallBindings()
    {
        Debug.Log("scene install");
        Container.Bind<IZenjectModule>().FromInstance(sceneModule);
    }

}
