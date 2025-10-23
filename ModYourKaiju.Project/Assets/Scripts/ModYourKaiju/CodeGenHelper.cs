using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

public static class CodeGenHelper
{
    private static AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MykMykDynamicAssembly"), AssemblyBuilderAccess.Run);
    private static ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MykMykDynamicModule");

    private static Dictionary<string, Type> builtTypes = new();
    public static Type GetOrCreateVehicleTier2<T, U>()
        where T : ICustomVehicle
        where U : IContext, IContainer, IActivationTokenInspector
    {
        var baseType = typeof(T);
        var myTypeName = baseType.Name + "_" + typeof(U).Name;

        if (!builtTypes.ContainsKey(myTypeName))
        {
            var typeBuilder = moduleBuilder.DefineType(myTypeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                typeof(CustomVehicleContext<T>), new Type[] { typeof(IGameState), typeof(ITier2), typeof(IGameplay), typeof(T), typeof(U) });
            builtTypes[myTypeName] = typeBuilder.CreateType();
        }

        return builtTypes[myTypeName];
    }

    public static Type GetOrCreateVehicleTier3<T, U>()
        where T : ICustomVehicle
        where U : IContext, IContainer, IActivationTokenInspector
    {
        var baseType = typeof(T);
        var myTypeName = baseType.Name + "_" + typeof(U).Name;

        if (!builtTypes.ContainsKey(myTypeName))
        {
            var typeBuilder = moduleBuilder.DefineType(myTypeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                typeof(CustomVehicleContext<T>), new Type[] { typeof(IGameState), typeof(IMeYou), typeof(ITier3), typeof(IGameplay), typeof(T), typeof(U) });
            builtTypes[myTypeName] = typeBuilder.CreateType();
        }

        return builtTypes[myTypeName];
    }

    public static Type GetOrCreateCustomVehicleGameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>() 
        where TICustomVehicle : ICustomVehicle 
        where TCustomVehicle :  OwnableCustomVehicleContext<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>, ISelectableVehicle, ITier1, TICustomVehicle
        where TMeYouCustomVehicle : CustomVehicleContext<TICustomVehicle>, IMeYou, ITier2, TICustomVehicle, IOwned<TMeYouCustomVehicle, MeYou, TCustomVehicle>, IOwned
    {
        var baseType = typeof(TCustomVehicle);
        var myTypeName = baseType.Name + "_GameStateManager";
        Debug.Log($"{myTypeName}");

        if (!builtTypes.ContainsKey(myTypeName))
        {
            var cv_dm = GetOrCreateVehicleTier2<TICustomVehicle, IDeathmatch>();
            var cv_lb = GetOrCreateVehicleTier2<TICustomVehicle, ILobby>();
            var cv_tt = GetOrCreateVehicleTier2<TICustomVehicle, ITitle>();

            var gsmbase = typeof(CustomVehicle_GameStateManager<,,,,,>);
            var cv_gsm = gsmbase.MakeGenericType(typeof(TCustomVehicle), typeof(TICustomVehicle), typeof(TMeYouCustomVehicle), cv_dm, cv_lb, cv_tt);
            
            TypeBuilder typeBuilder = moduleBuilder.DefineType(myTypeName,
                                                             TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                                                             cv_gsm);
            builtTypes[myTypeName] = typeBuilder.CreateType();
        }
        return builtTypes[myTypeName];
    }

    public static Type GetOrCreateMeYouCustomVehicleGameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>()
        where TICustomVehicle : ICustomVehicle 
        where TCustomVehicle :  OwnableCustomVehicleContext<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>, ISelectableVehicle, ITier1, TICustomVehicle
        where TMeYouCustomVehicle : CustomVehicleContext<TICustomVehicle>, IMeYou, ITier2, TICustomVehicle, IOwned<TMeYouCustomVehicle, MeYou, TCustomVehicle>, IOwned
    {

        var baseType = typeof(TCustomVehicle);
        var myTypeName = "MeYou_" + baseType.Name + "_GameStateManager";
        Debug.Log($"{myTypeName}");

        if (!builtTypes.ContainsKey(myTypeName))
        {
            var mycv_dm = GetOrCreateVehicleTier3<TICustomVehicle, IDeathmatch>();
            var mycv_lb = GetOrCreateVehicleTier3<TICustomVehicle, ILobby>();
            var mycv_tt = GetOrCreateVehicleTier3<TICustomVehicle, ITitle>();

            var my_cv = GetOrCreateCustomVehicleGameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>();

            var gsmbase = typeof(CustomMeYouVehicle_GameStateManager<,,,,,,>);
            var cv_gsm = gsmbase.MakeGenericType(typeof(TICustomVehicle), typeof(TCustomVehicle), typeof(TMeYouCustomVehicle), my_cv, mycv_dm, mycv_lb, mycv_tt);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(myTypeName,
                                                             TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                                                             cv_gsm);
            builtTypes[myTypeName] = typeBuilder.CreateType();
        }
        return builtTypes[myTypeName];
    }

}
