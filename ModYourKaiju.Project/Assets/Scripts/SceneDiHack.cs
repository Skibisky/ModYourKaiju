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

    public void OnValidate()
    {
        if (!sceneContext)
            sceneContext = FindObjectOfType<SceneContext>();
        if (!modInstall)
            modInstall = FindObjectOfType<ModulesInstaller>();
        if (!sceneModule)
            sceneModule = FindObjectOfType<SceneModule>();
    }

    [ContextMenu("check hack")]
    public void CheckHack()
    {
        IsHacked = sceneContext.Installers.Contains(modInstall) && sceneContext.Installers.Contains(this);
    }

    [ContextMenu("do hack")]
    public void DoHack()
    {
        if (modInstall)
            sceneContext.Installers = sceneContext.Installers.Union(new MonoInstaller[] { this, modInstall });
        CheckHack();
    }

    [ContextMenu("prepare scene for exporting")]
    public void SmallHack()
    {
        sceneContext.Installers = sceneContext.Installers.Except(new MonoInstaller[] { this }).Union(new MonoInstaller[] { modInstall });
    }

    public override void InstallBindings()
    {
        Debug.Log("scene install");
        Container.Bind<IZenjectModule>().FromInstance(sceneModule);
    }

}
