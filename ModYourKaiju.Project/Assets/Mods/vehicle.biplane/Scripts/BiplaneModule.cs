using Runtime.Deathmatch;
using Sentient.MeYouKaiju;
using UnityEngine;
using Zenject;

public class BiplaneModule : MykMykModule
{
    protected override void OnInstall(Context context)
    {
        BepLog.Log($"Installing Biplane mod module into {context.name}");
        if (context is Biplane bp)
        {
            InjectCustomVehicle(bp);

            FindOn<BiplaneController>(context.gameObject);

            AddOn<VehicleTakeDamageFromPhysics>(context.gameObject, new VehicleTakeDamageFromPhysics.Args()
            {
                DamagePerImpulse = 0.1f,
                ImpulseThreshold = 400,
            });

            Add<VehicleDamageEffects>(MykAssets.heliDamEfArgs);
            Add<DamageAudio>(MykAssets.heliDamAudio);

            Add<HpConfigurer>(new HpConfigurer.Args() { BaseHp = 200 });
            
            Equip<MiniGun>(MykAssets.heliGun.Prefab, VehicleWeaponSlotType.Primary);
            Equip<SidewinderLauncher>(VehicleWeaponSlotType.Secondary);
            Equip<HellfireLauncher>(VehicleWeaponSlotType.Special);

            Add<Biplane_GameStateManager>();
        }
        if (context is MeYou_Biplane mybp)
        {
            InjectMeYouCustomVehicle(mybp);
            Add<ConnectInputToBiplane>();
            Add<ConfigureSmoothFollow>(new ConfigureSmoothFollow.Args()
            {
                Follow = new SmoothFollow.MotionConfig()
                {
                    Offset_Local = new Vector3(0, 0, 3),
                    Offset_Local_Flat = new Vector2(0, -15),
                    Responsiveness = 7,
                },
                LookAt = new SmoothFollow.MotionConfig()
                {
                    Offset_Local = new Vector3(0, 0, 3),
                    Offset_Local_Flat = new Vector2(0, 15),
                    Responsiveness = 7,
                }
            });
            Add<EnableActionMap>(new EnableActionMap.Args { TargetMapName = "Helicopter", IsExclusiveMap = false });
            Add<MeYou_Biplane_GameStateManager>();
        }
        if (context is IBiplane and ITier2 and IDeathmatch)
        {
            Add<DecrementMeYouLivesOnDestroy>();
        }
    }
}
