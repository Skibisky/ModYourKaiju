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

    [ContextMenu("check hack")]
    public void CheckHack()
    {
        IsHacked = sceneContext.Installers.Contains(modInstall);
    }

    [ContextMenu("do hack")]
    public void DoHack()
    {
        if (modInstall)
            sceneContext.Installers = sceneContext.Installers.Union(new MonoInstaller[] { modInstall });
        CheckHack();
    }
}
