using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using Sentient.Players.Network;
using Sentient.Players.Skeleton;
using Sentient.Rendering;
using Sentient.Rendering.Player;
using Sentient.XR.Interaction;
using Sentient.XR.Interaction.Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Zenject;

public class ProjectDiHack : MykMykModule
{
    [Header("Injection Hacks")]
    [SerializeField] public ProjectContext ctx;
    [SerializeField] public ModuleConfigurer cfgr;
    [SerializeField] public bool IsHacked;
    [SerializeField] public ModulesInstaller modInstall;

    [Header("Random Args/Config")]
    [SerializeField] public AudioGroups MykAudioGroups;
    [SerializeField] public ConfigureSmoothFollow.Args SmoothFollowArgs;
    [SerializeField] public GameStateTransitionService.Args GameStateTransitionServiceArgs;
    [SerializeField] public PlayerInputUser.Args PlayerInputUserArgs;
    [SerializeField] public PlayerDeviceManager.Args PlayerDeviceManagerArgs;
    [SerializeField] public MykPlayerManager.Args MykPlayerManagerArgs;
    [SerializeField] public VehicleTakeDamageFromPhysics.Args VehicleTakeDamageFromPhysicsArgs;
    [SerializeField] public VehicleDamageEffects.Args VehicleDamageEffectsArgs;
    [SerializeField] public DamageAudio.Args DamageAudioArgs;
    [SerializeField] public DamageOverlay.Args DamageOverlayArgs;
    [SerializeField] public MiniGun.Args MiniGunArgs;
    [SerializeField] public PlayerMarkerSlot.Args PlayerMarkerSlotArgs;
    [SerializeField] public UnityEngine.VFX.VisualEffectAsset TrailVisualEffectAsset;
    [SerializeField] public RingSpawner.Args RingSpawnerArgs;

    [ContextMenu("check hack")]
    public void CheckHack()
    {
        IsHacked = cfgr.BehaviourModules.Contains(this) && ctx.Installers.Contains(cfgr) && ctx.Installers.Contains(modInstall);
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

    public override void Initialise()
    {
        base.Initialise();
        MykAssets.PlayerMarkerSlotArgs = PlayerMarkerSlotArgs;
        MykAssets.DefaultVisualEffect = TrailVisualEffectAsset;

        MykAssets.heliDamEfArgs = VehicleDamageEffectsArgs;
        MykAssets.heliDamAudio = DamageAudioArgs;
        MykAssets.heliGun = MiniGunArgs;
    }

    private SceneModule snapModule;

    protected override void OnInstall(Context context)
    {
        Debug.Log($"Mykmyk project install {context.name} {context.GetType().Name}", context);
        if (context is ProjectContext)
        {
            Add<PlayerManager>();
            Add<AudioListenerSingleton>();
            Container.BindInterfacesAndSelfTo<ProjectContext>().FromInstance(ProjectContext.Instance).AsSingle();
            Container.BindInterfacesAndSelfTo<Global_GameStateManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Add<MykPlayerManager>(MykPlayerManagerArgs);
            Add<ProfileManager>();
            Add<SpawnPointRepository>();
            Add<GameStateTransitionService>(GameStateTransitionServiceArgs);
            Container.BindInstance(true).AsCached().WhenInjectedInto<Interactable>().NonLazy();
            Add<RingSpawner>(RingSpawnerArgs);

            Add<PlayerDeviceManager>(PlayerDeviceManagerArgs);

            Container.BindInstance(MiniGunArgs).AsSingle().NonLazy();

            Container.BindInstance(new SidewinderLauncher.Args()
            {
            }).AsSingle().NonLazy();

            Container.BindInstance(new HellfireLauncher.Args()
            {
            }).AsSingle().NonLazy();
            Add<ProjectileParent>();
            Add<VehicleRepository>(new VehicleRepository.Args() { });
            Add<TrackableRegistry>();
            Add<ParticleParent>();
            Add<ColourOverlayManager>();
            Expose<AudioGroups>(MykAudioGroups);
            Container.BindInterfacesAndSelfTo<InteractionManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<InputDevice>().FromResolveGetter<ProjectContext, InputDevice>(pc =>
            {
                // shove keyboard in
                if (snapModule && snapModule.testContext == SceneModule.TestContext.KaijuDeathMatch)
                {
                    return (InputSystem.devices.ToList().FirstOrDefault(f => f is UnityEngine.InputSystem.XR.XRHMD));
                }
                return (InputSystem.devices.ToList().FirstOrDefault(f => f is Keyboard));
            }).AsSingle().Lazy();
            
            var snapCtx = context;
            // make a userdata from keyboard
            Container.BindInterfacesAndSelfTo<InputUserData>().FromResolveGetter<InputDevice, InputUserData>(indev =>
            {
                var devmgr = snapCtx.Container.Resolve<PlayerDeviceManager>();
                var uu = devmgr.MakeInputUserForDevice(indev);
                return devmgr.GetCreatedUsers().FirstOrDefault(b => b.InputUser == uu);
            }).AsSingle().Lazy();

            // make a user from data
            Container.BindInterfacesAndSelfTo<PlayerInputUser.Args>().FromResolveGetter< InputUserData, PlayerInputUser.Args>(ud =>
            {
                return new PlayerInputUser.Args()
                {
                    InputUser = ud.InputUser,
                    Controls = ud.Controls,
                    Scheme = ud.Controls.KeyboardScheme,
                };
            }).AsSingle().Lazy();
            
            // create MeYou
            Container.BindInterfacesAndSelfTo<MeYou>().FromResolveGetter<InputUserData, MeYou>(ud =>
            {
                var devmgr = snapCtx.Container.Resolve<PlayerDeviceManager>();

                return devmgr.CreatePlayerForInputUser(ud).GetComponent<MeYou>();
            }).AsSingle().Lazy();


            // create player
            Container.BindInterfacesAndSelfTo<PlayerInputUser>().FromNewComponentOnNewGameObject().AsSingle().Lazy();

        }
        if (context is SceneContext)
        {
            context.TryGetComponent<SceneModule>(out snapModule);

            if (snapModule && snapModule.testContext == SceneModule.TestContext.VehicleDeathmatch)
            {
                Add<CameraManager>();

                var prefs = gameObject.GetOrAddComponent<VehiclePreferences>();

                var pro = typeof(VehiclePreferences).GetProperty("PreferredVehicle");
                pro.SetValue(prefs, (Vehicle)42);

                Expose(prefs);
            }
        }
        if (context is MeYou my)
        {
            my.SetPlayerType(PlayerType.RealPlayer);
            //Container.Bind<MeYou>().FromInstance(my).AsSingle().NonLazy();
            Add<MeYouVehicleService>();
            Add<MeYou_GameStateManager>();
            Add<SmoothFollow>();
            Add<MeYouViewModel>();
            Add<Sentient.MeYouKaiju.PlayerColour>();
            Add<RegisterPlayer>(my);

            if (snapModule && snapModule.testContext == SceneModule.TestContext.VehicleDeathmatch)
            {
                Getter<CameraRig, ProjectContext>( c => FindObjectOfType<CameraRig>());
                Add<ConfigureCameraForMeYou>();
                Add<LookAtVehicle>();
                Add<FakeMeYouRespawner>();
            }

        }
        if (context is IPlayer)
        {
            Add<PlayerServices>();
            Add<PlayerSubContainers>();
            Container.BindInterfacesAndSelfTo<PlayerSkeleton>().FromSubComponent<PlayerServices>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerNetwork>().FromSubComponent<PlayerServices>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerFirstPerson>().FromSubComponent<PlayerServices>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerTransformController>().FromSubComponent<PlayerServices>().AsSingle();
        }
        if (context is IVehicle vehCtx and ITier1)
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
        if (context is IMeYou and IVehicle and ITier2)
        {
            Add<ApplyColourToVessel>();
            Add<DamageOverlay>(DamageOverlayArgs);
        }

        CheckAndInstallMods(context);
    }


    List<MykMykModule> mykMods = null;

    public void CheckAndInstallMods(Context context)
    {
        if (mykMods == null)
        {
            mykMods = this.GetComponents<MykMykModule>().Where(m => m != this).ToList();
        }

        foreach (var mod in mykMods)
        {
            mod.Install(context);
        }

    }

}
