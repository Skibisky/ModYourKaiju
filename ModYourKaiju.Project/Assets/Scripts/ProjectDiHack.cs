using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using Sentient.XR.Interaction;
using Sentient.XR.Interaction.Interactables;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ProjectDiHack : MykMykModule
{
    [SerializeField] public GameStateTransitionService.Args GameStateTransitionServiceArgs;
    [SerializeField] public PlayerInputUser.Args PlayerInputUserArgs;
    [SerializeField] public PlayerDeviceManager.Args PlayerDeviceManagerArgs;
    [SerializeField] public MykPlayerManager.Args MykPlayerManagerArgs;
    [SerializeField] public int BiplaneHealth = 200;
    [SerializeField] public VehicleTakeDamageFromPhysics.Args VehicleTakeDamageFromPhysicsArgs;
    [SerializeField] public VehicleDamageEffects.Args VehicleDamageEffectsArgs;
    [SerializeField] public DamageAudio.Args DamageAudioArgs;
    [SerializeField] public DamageOverlay.Args DamageOverlayArgs;

    [SerializeField] public ConfigureSmoothFollow.Args SmoothFollowArgs;

    [SerializeField] public ProjectContext ctx;
    [SerializeField] public ModuleConfigurer cfgr;
    [SerializeField] public bool IsHacked;
    [SerializeField] public ModulesInstaller modInstall;

    [ContextMenu("check hack")]
    public void CheckHack()
    {
        IsHacked = cfgr.BehaviourModules.Contains(this) && ctx.Installers.Contains(modInstall);
    }

    [ContextMenu("do hack")]
    public void DoHack()
    {
        if (!cfgr.BehaviourModules.Contains(this))
            cfgr.BehaviourModules.Add(this);

        if (modInstall)
            ctx.Installers = ctx.Installers.Union(new MonoInstaller[] { cfgr, modInstall });
        CheckHack();
    }

    protected override void OnInstall(Context context)
    {
        Debug.Log($"shim install {context.name} {context.GetType().Name}", context);
        if (context is ProjectContext)
        {
            Container.BindInterfacesAndSelfTo<ProjectContext>().FromInstance(ProjectContext.Instance).AsSingle();
            Container.BindInterfacesAndSelfTo<Global_GameStateManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Add<MykPlayerManager>(MykPlayerManagerArgs);
            Add<ProfileManager>();
            Add<GameStateTransitionService>(GameStateTransitionServiceArgs);
            Container.BindInstance(true).AsCached().WhenInjectedInto<Interactable>().NonLazy();
            Add<PlayerDeviceManager>(PlayerDeviceManagerArgs);

            Getter<PlayerInputUser.Args, PlayerDeviceManager>(devmgr =>
            {
                var keys = InputSystem.devices.ToList().FirstOrDefault(f => f is Keyboard);
                var uu = devmgr.MakeInputUserForDevice(keys);
                var ee = devmgr.GetCreatedUsers().FirstOrDefault(b => b.InputUser == uu);

                MykControls c = new();

                return new PlayerInputUser.Args()
                {
                    InputUser = uu,
                    Controls = ee.Controls,
                    Scheme = c.KeyboardScheme,
                };
            });
            
            Container.BindInstance(new MiniGun.Args()
            {
                CooldownBehaviourArgs = new GenericWeaponCooldownBehaviour.Args()
                {
                    RateOfFire = new RateOfFire()
                    {
                        Value = 720,
                    }
                },
                FireBehaviourArgs = new MiniGunVehicleWeaponFireBehaviour.Args()
                {

                },
                Reticle = new MiniGunReticle.Args()
                {
                    
                },
            }).AsSingle().NonLazy();

            Container.BindInstance(new SidewinderLauncher.Args()
            {
            }).AsSingle().NonLazy();

            Container.BindInstance(new HellfireLauncher.Args()
            {
            }).AsSingle().NonLazy();
            Add<ProjectileParent>();
            //Add<PlayerManager>();
            Add<VehicleRepository>(new VehicleRepository.Args() { });
            Add<TrackableRegistry>();
            Add<ParticleParent>();
            Add<PlayerInputUser>();
            Expose<AudioGroups>(new AudioGroups()
            {
            });
        }
        if (context is SceneContext)
        {
            Container.BindInterfacesAndSelfTo<InteractionManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
        if (context is IVehicle vehCtx)
        {
            Expose((IOwnable)context);
            Expose(vehCtx.Context);
            Add<Health>();
            Add<Trackable>();
            Add<RegisterVehicle>();
            Add<VehicleWeaponService>();
            Add<PilotedWeaponSlotManager>();
            Add<DestroyVehicleOnExitGamemode>();
            Find<VehicleTransforms>();
            Find<VehicleTriggerContainer>();
            AddOn<CrateInteractor>(Container.Resolve<VehicleTriggerContainer>().gameObject);
        }
        if (context is IMeYou)
        {
            Add<ApplyColourToVessel>();
            Add<DamageOverlay>(DamageOverlayArgs);
        }
        if (context is Biplane bp)
        {
            //Add<HpConfigurer>(new HpConfigurer.Args() { BaseHp = BiplaneHealth });
            FindOn<BiplaneController>(context.gameObject);
            FindOn<Rigidbody>(context.gameObject);
            Find<ParticleRoot>();
            AddOn<VehicleTakeDamageFromPhysics>(context.gameObject, VehicleTakeDamageFromPhysicsArgs);
            Add<VehicleDamageEffects>(VehicleDamageEffectsArgs);

            Add<DamageAudio>(DamageAudioArgs);

            Find<PlayerMarkerSlot>();
            FindAll<Hurtbox>();

            Equip<MiniGun>(VehicleWeaponSlotType.Primary);
            Equip<SidewinderLauncher>(VehicleWeaponSlotType.Secondary);
            Equip<HellfireLauncher>(VehicleWeaponSlotType.Special);

            FindAll<PilotedWeaponSlotContext>();
        }
        if (context is MeYou_Biplane)
        {
            Add<ConnectInputToBiplane>();
            Add<ConfigureSmoothFollow>(SmoothFollowArgs);
            Add<EnableActionMap>(new EnableActionMap.Args { TargetMapName = "Helicopter", IsExclusiveMap = false });
            Add<PostVehicleHealthToMeYouVM>();
            Add<PostCurrentVehicleWeaponsToVM>();
        }
        if (context is ITier3)
        {

        }
        if (context is IBiplane and IDeathmatch and ITier2)
        {
            //Add<DecrementMeYouLivesOnDestroy>();
        }
    }

}
