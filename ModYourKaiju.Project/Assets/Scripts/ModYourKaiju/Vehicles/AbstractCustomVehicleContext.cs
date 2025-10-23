using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using Zenject;

public abstract class AbstractCustomVehicleContext<T, U, V> : CustomVehicleContext<T>, IMeYou, ITier2, ICustomVehicle, IOwned<V, MeYou, U>
    where T : ICustomVehicle
    where U : BaseObjectContext, ICustomVehicle, IOwnable
    where V : AbstractCustomVehicleContext<T, U, V>
{
    public override AutoInitMode AutoInitialise => AutoInitMode.Off;

    [Inject]
    public U ParentContext { get; private set; }

    [Inject]
    public MeYou OwnerContext { get; private set; }

    IOwnable IOwned.ParentContext_Untyped => ParentContext;

    Context IOwned.OwnerContext_Untyped => OwnerContext;

}