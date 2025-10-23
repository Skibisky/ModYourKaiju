using MonoMod.RuntimeDetour;
using Sentient.MeYouKaiju;
using Sentient.MeYouKaiju.SelectableCollectionControl;
using System;
using System.ComponentModel;
using System.Reflection;
using Zenject;

namespace ModYourKaiju.Plugin;

public class GameplayLevelLoaderServicePatch
{
    public static Hook theHook;
    public static Hook theHook2;

    public static string targetLevel;

    public void Patch()
    {
        Plugin.Logger.LogInfo("Checking level patch");
        try
        {
            if (theHook == null)
            {
                MethodInfo source = typeof(LevelSelectionControl).GetMethod("OnOptionSelected", BindingFlags.NonPublic | BindingFlags.Instance );
                theHook = new Hook(source, this.OnOptionSelected);
            }
            if (!theHook.IsApplied)
                theHook.Apply();

            //if (theHook2 == null)
            //{
            //    MethodInfo source = typeof(GameplayLevelLoadService).GetMethod("IsSelectable", new Type[] { typeof(Vehicle) });
            //    theHook2 = new Hook(source, this.IsSelectable);
            //}
            //if (!theHook2.IsApplied)
            //    theHook2.Apply();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError(ex.ToString());
        }
    }

    public void OnOptionSelected(LevelSelectionControl og, LevelSelectControlOption option)
    {
        Plugin.Logger.LogInfo($"hacked level select! {option}");
        targetLevel = option.Title;

        try
        {
            theHook.Undo();
            MethodInfo source = typeof(LevelSelectionControl).GetMethod("OnOptionSelected", BindingFlags.NonPublic | BindingFlags.Instance );
            source.Invoke(og, new object[] { option });
        }
        finally
        {
            theHook.Apply();
        }

    }
}
