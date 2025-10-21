using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using Zenject;

public class MeYou_CustomVehicle<TSelf, TOwnable, TVehicle> : BaseObjectContext, IMeYou, IVehicle, IVessel, IContext, IContainer, IActivationTokenInspector, ITeamSource, ITier2, IOwned, IOwned<TSelf, MeYou, TOwnable>
    where TSelf : BaseObjectContext, IOwned<TSelf, MeYou, TOwnable>
    where TOwnable : IOwnable
    where TVehicle : ICustomVehicle
{
    public Vehicle VehicleType => DepartmentOfMotorVehicles.GetRegistration<TVehicle>();
    public IOwnable ParentContext_Untyped { get; }
    public Context OwnerContext_Untyped { get; }
    
    [Inject]
    public TOwnable ParentContext { get; private set; }

    [Inject]
    public MeYou OwnerContext { get; private set; }

}