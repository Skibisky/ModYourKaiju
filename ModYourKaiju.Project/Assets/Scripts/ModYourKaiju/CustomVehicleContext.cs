using Sentient.Injection;
using Sentient.MeYouKaiju;

public class CustomVehicleContext<T> : BaseObjectContext, IVehicle
    where T : ICustomVehicle
{
    public Vehicle VehicleType => DepartmentOfMotorVehicles.GetRegistration<T>();
}
