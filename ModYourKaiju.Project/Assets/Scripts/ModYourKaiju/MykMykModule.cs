using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

public abstract class MykMykModule : AbstractModuleComponent
{

    protected void InjectCustomVehicle<T>(T context) where T : BaseObjectContext, IVehicle, ITier1
    {
        // todo: HpConfigurer w/ Args
        FindOn<Rigidbody>(context.gameObject);
        Find<ParticleRoot>();
        Find<PlayerMarkerSlot>();
        FindAll<Hurtbox>();
        FindAll<PilotedWeaponSlotContext>();
    }

    protected void InjectMeYouCustomVehicle<T>(T context) where T : BaseObjectContext, IVehicle, ITier2
    {
        Add<PostVehicleHealthToMeYouVM>();
        Add<PostCurrentVehicleWeaponsToVM>();
    }

    protected void GenerateGameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>() 
        where TICustomVehicle : ICustomVehicle 
        where TCustomVehicle :  OwnableCustomVehicleContext<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>, ISelectableVehicle, ITier1, TICustomVehicle
        where TMeYouCustomVehicle : CustomVehicleContext<TICustomVehicle>, IMeYou, ITier2, TICustomVehicle, IOwned<TMeYouCustomVehicle, MeYou, TCustomVehicle>, IOwned
    {
        var gsmt = CodeGenHelper.GetOrCreateCustomVehicleGameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>();

        Add(gsmt);
    }

    protected void GenerateMeYouGameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>() 
        where TICustomVehicle : ICustomVehicle 
        where TCustomVehicle :  OwnableCustomVehicleContext<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>, ISelectableVehicle, ITier1, TICustomVehicle
        where TMeYouCustomVehicle : CustomVehicleContext<TICustomVehicle>, IMeYou, ITier2, TICustomVehicle, IOwned<TMeYouCustomVehicle, MeYou, TCustomVehicle>, IOwned
    {
        var mygsmt = CodeGenHelper.GetOrCreateMeYouCustomVehicleGameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>();

        Add(mygsmt);
    }

    protected void Expose<T>(T instance)
    {
        Container.BindInterfacesAndSelfTo<T>().FromInstance(instance).AsSingle().NonLazy();
    }

    protected void Add<T>(params object[] args)
    {
        Container.BindInterfacesAndSelfTo<T>().FromNewComponentOnNewGameObject().AsSingle().WithArguments(args).NonLazy();
    }
    protected void Add(Type t, params object[] args)
    {
        Container.BindInterfacesAndSelfTo(t).FromNewComponentOnNewGameObject().AsSingle().WithArguments(args).NonLazy();
    }

    protected void AddOn<T>(GameObject go, params object[] args)
    {
        Container.BindInterfacesAndSelfTo<T>().FromNewComponentOn(go).AsSingle().WithArguments(args).NonLazy();
    }

    protected void Find<T>()
    {
        Container.BindInterfacesAndSelfTo<T>().FromComponentInHierarchy(true).AsSingle().NonLazy();
    }

    protected void FindOn<T>(GameObject go)
    {
        Container.BindInterfacesAndSelfTo<T>().FromComponentOn(go).AsSingle().NonLazy();
    }

    protected void FindAll<T>()
    {
        Container.BindInterfacesAndSelfTo<T>().FromComponentsInHierarchy().AsSingle().NonLazy();
    }

    protected void Getter<TTarget, TSource>(Func<TSource, TTarget> meth)
    {
        Container.Bind<TTarget>().FromResolveGetter(meth).AsSingle().NonLazy();
    }

    protected void Equip<T>(VehicleWeaponSlotType slot) where T : MonoBehaviour, IVehicleWeapon
    {
        Container.Bind<WeaponLoadout>().WithId(typeof(T)).ToSelf().FromNewComponentOnNewGameObject().AsTransient().WithArguments(typeof(T), slot).NonLazy();
    }

    protected void Equip<T>(T prefab, VehicleWeaponSlotType slot) where T : MonoBehaviour, IVehicleWeapon
    {
        Container.Bind<WeaponLoadout>().WithId(typeof(T)).ToSelf().FromNewComponentOnNewGameObject().AsTransient().WithArguments(prefab, typeof(T), slot).NonLazy();
    }

}
