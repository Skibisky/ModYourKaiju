using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using Zenject;

public class MeYou_Biplane : CustomVehicleContext<IBiplane>, IMeYou, ITier2, IBiplane, IOwned<MeYou_Biplane, MeYou, Biplane>
{
    public override AutoInitMode AutoInitialise => AutoInitMode.Off;

    [Inject]
    public Biplane ParentContext { get; private set; }

    [Inject]
    public MeYou OwnerContext { get; private set; }

    IOwnable IOwned.ParentContext_Untyped => ParentContext;

    Context IOwned.OwnerContext_Untyped => OwnerContext;

}
