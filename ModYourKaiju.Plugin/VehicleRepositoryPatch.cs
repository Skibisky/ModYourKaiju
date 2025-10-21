using MonoMod.RuntimeDetour;
using Sentient.MeYouKaiju;
using System;
using System.Reflection;
using Zenject;

namespace ModYourKaiju.Plugin;

public class VehicleRepositoryPatch
{
    public static Hook theHook;
    public static Hook theHook2;

    public void Patch()
    {
        Plugin.Logger.LogInfo("Checking vehicle patch");
        try
        {
            if (theHook == null)
            {
                MethodInfo source = typeof(VehicleRepository).GetMethod("CreateVehicle", new Type[] { typeof(Vehicle), typeof(DiContainer) });
                theHook = new Hook(source, this.CreateVehicle);
            }
            if (!theHook.IsApplied)
                theHook.Apply();

            if (theHook2 == null)
            {
                MethodInfo source = typeof(Vehicles).GetMethod("IsSelectable", new Type[] { typeof(Vehicle) });
                theHook2 = new Hook(source, this.IsSelectable);
            }
            if (!theHook2.IsApplied)
                theHook2.Apply();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError(ex.ToString());
        }
    }

    public bool IsSelectable(Vehicle vehicle)
    {
        Plugin.Logger.LogInfo($"pick something good! {vehicle}");

        return vehicle switch
        {
            Vehicle.Drone => true,
            Vehicle.Helicopter => true,
            Vehicle.Javelin => false,
            _ => true,
        };
    }

    public IVehicle CreateVehicle(VehicleRepository og, Vehicle vehicleType, DiContainer container = null)
    {
        IVehicle returningVehicle = null;
        Plugin.Logger.LogInfo($"hacked vehicles! {vehicleType}");

        try
        {
            theHook.Undo();
            returningVehicle = og.CreateVehicle(vehicleType, container);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            container ??= ProjectContext.Instance.Container;

            if (DepartmentOfMotorVehicles.IsRegistered(vehicleType))
            {
                return container.InstantiatePrefabForComponent<IVehicle>(DepartmentOfMotorVehicles.RegisteredVehicles[(int)vehicleType]);
            }

            Plugin.Logger.LogInfo($"sus vehicle detected! {vehicleType}");
            throw;
        }
        finally
        {
            theHook.Apply();
        }

        return returningVehicle;
    }
}