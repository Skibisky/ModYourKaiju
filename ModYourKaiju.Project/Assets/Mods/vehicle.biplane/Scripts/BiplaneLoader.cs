using Sentient.MeYouKaiju;
using UnityEngine;
using UnityEngine.VFX;

public class BiplaneLoader : IMykMod
{
    public static GameObject biplanePrefab;
    public static Sprite biplaneSprite;

    public BiplaneLoader()
    {
        var ab = AssetBundle.LoadFromFile("BepInEx\\plugins\\Mykmyk\\Biplane\\vehicle.biplane.mykmyk");
        var names = ab.GetAllAssetNames();
        BepLog.Log($"assets: {string.Join(",", names)}");
        biplanePrefab = ab.LoadAsset<GameObject>("assets/mods/vehicle.biplane/prefabs/biplane.prefab");
        biplaneSprite = ab.LoadAsset<Sprite>("assets/mods/vehicle.biplane/sprites/biplane.bmp");
    }

    public void Configure()
    {
        BepLog.Log("Configuring biplane mod");
        MykAssets.FixPlayerMarker(biplanePrefab.GetComponentInChildren<PlayerMarkerSlot>(true));
        biplanePrefab.GetComponentInChildren<VisualEffect>().visualEffectAsset = MykAssets.DefaultVisualEffect;
        DepartmentOfMotorVehicles.Register<IBiplane>("Biplane", biplaneSprite, biplanePrefab);
    }
}
