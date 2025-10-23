using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using System.Linq;
using UnityEngine;
using Zenject;

public class FakeMeYouRespawner : MonoBehaviour
{
    public RingSpawner spawner;
    public MeYouVehicleService vehicleService;

    [ComponentConstructor]
    public void Construct(RingSpawner spawn, MeYouVehicleService serv)
    {
        Debug.Log($"building fake respawn");
        spawner = spawn;
        vehicleService = serv;
    }

    IVehicle lastVehicle;

    public IVehicle Respawn( ISelectableVehicle vehiclePrefab)
    {
        var vehicle = Create(vehiclePrefab);
        if (vehicle != null)
        {
            var result = spawner.Spawn();
            vehicle.Transform.position = result.Position;
            vehicle.Transform.rotation = Quaternion.LookRotation(result.Direction);
            //Mount(vehicle);

            if (lastVehicle != null)
            {
                var kill = lastVehicle.Context;
                vehicleService.Dismount();
                kill.SafeDestroy();
            }

            if (vehicle != null)
            {
                vehicleService.Mount(vehicle);
            }
        }
        return vehicle;
    }

    public IVehicle Create(ISelectableVehicle vehicleProto)
    {
        if (vehicleProto is BaseObjectContext bh)
        {
            var tt = bh.GetType();
            Debug.Log($"attempting spawn of {tt.FullName} with {bh.gameObject}");
            var cont = ProjectContext.Instance.Container;
            return (IVehicle)cont.InstantiatePrefabForComponent(tt, bh.gameObject, null, Enumerable.Empty<object>());
        }
        else
        {
            Debug.LogError($"{vehicleProto} is not a monobehaviour.");
        }
        return null;

    }

}