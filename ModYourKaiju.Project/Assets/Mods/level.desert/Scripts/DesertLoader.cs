using System.Linq;
using UnityEngine;

public class DesertLoader : IMykMod
{
    public static GameObject desertScene;

    public DesertLoader()
    {
        var ab = AssetBundle.LoadFromFile("BepInEx\\plugins\\Mykmyk\\Desert\\level.desert.mykmyk");
        var names = ab.GetAllAssetNames();
        var scenes = ab.GetAllScenePaths();
        BepLog.Log($"assets: {string.Join(",", names)}");
        BepLog.Log($"scenes: {string.Join(",", scenes)}");

        RealestateOffice.RegisterLevel("Featureless Desert", scenes.FirstOrDefault());
    }

    public void Configure()
    {
        BepLog.Log("Configuring desert mod");
    }
}
