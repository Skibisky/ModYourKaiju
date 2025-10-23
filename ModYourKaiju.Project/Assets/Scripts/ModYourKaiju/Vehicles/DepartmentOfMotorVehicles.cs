using Sentient.MeYouKaiju;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DepartmentOfMotorVehicles
{
    static int next = 20;

    private static Dictionary<Type, int> regos = new();
    private static HashSet<int> registered = new();

    public static List<VehicleControlOptions> RegisteredOptions = new();

    public static Dictionary<int, GameObject> RegisteredVehicles = new();

    public static bool IsRegistered(Vehicle vehicle)
    {
        return registered.Contains((int)vehicle);
    }


    public static Vehicle Register<T>(string lobbyName, Sprite sprite, GameObject prefab)
        where T : ICustomVehicle
    {
        BepLog.Log($"Registering {typeof(T).Name} with {lobbyName}, {sprite} & {prefab}");

        if (regos.ContainsKey(typeof(T)))
            return (Vehicle)GetRegistration(typeof(T));
            //throw new InvalidOperationException($"{typeof(T)} is already registered!");

        var num = GetRegistration(typeof(T));

        RegisteredVehicles.Add(num, prefab);
        RegisteredOptions.Add(new VehicleControlOptions()
        {
            VehicleSprite = sprite,
            VehicleTitle = lobbyName,
            VehicleType = (Vehicle)num
        });

        return (Vehicle)num;
    }

    public static Vehicle GetRegistration<T>()
    {
        if (!regos.ContainsKey(typeof(T)))
            throw new ArgumentOutOfRangeException($"{typeof(T).Name} not a registered vehicle");

        return (Vehicle)GetRegistration(typeof(T));
    }

    public static int GetRegistration(Type vehicleBaseClass)
    {
        BepLog.Log($"Checking rego for {vehicleBaseClass.Name}");
        if (!regos.TryGetValue(vehicleBaseClass, out int number))
        {
            number = next++;
            regos[vehicleBaseClass] = number;
            registered.Add(number);
        }
        return number;
    }

}
