using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using System;
using UniRx;
using Zenject;

public abstract class OwnableCustomVehicleContext<T, U, V> : CustomVehicleContext<T>, IOwnable<U, MeYou, V>
    where T : ICustomVehicle
    where U : BaseObjectContext, IOwnable<U, MeYou, V>
    where V : BaseObjectContext, IOwned
{
    public override AutoInitMode AutoInitialise => AutoInitMode.On;

    IOwned IOwnable.OwnedContext_Untyped => OwnedContext;

    Context IOwnable.CurrentOwner_Untyped => CurrentOwner;
    
    IOwned IOwnable.TakeOwnership_Untyped(Context owner)
    {
        if (owner is not MeYou owner2)
        {
            throw new ArgumentException("Owner must be a MeYou", "owner");
        }

        return TakeOwnership(owner2);
    }
    public bool IsOwned => CurrentOwner != null;

    public IObservable<ContextOwnershipEvent> OnOwnershipChanged => _onOwnershipChanged;
    
    public V OwnedContext { get; private set; }

    public MeYou CurrentOwner { get; private set; }

    private readonly Subject<ContextOwnershipEvent> _onOwnershipChanged = new Subject<ContextOwnershipEvent>();
    
    public void RelinquishOwnership()
    {
        if (IsOwned)
        {
            MeYou currentOwner = CurrentOwner;
            CurrentOwner = null;
            V ownedContext = OwnedContext;
            OwnedContext = null;
            if (ownedContext)
                ownedContext.SafeDestroy();

            _onOwnershipChanged.OnNext(new ContextOwnershipEvent(ContextOwnershipEvent.EventType.BecameUnowned, null, currentOwner));
        }
    }

    public V TakeOwnership(MeYou owner)
    {
        if (owner == CurrentOwner)
            return OwnedContext;

        RelinquishOwnership();
        CurrentOwner = owner;
        OwnedContext = SubContainerUtil.CreateFrom<V>(base.Context.AsPrimaryParent(), new AdditionalParent[1] { owner.AsAdditionalParent() });
        _onOwnershipChanged.OnNext(new ContextOwnershipEvent(ContextOwnershipEvent.EventType.BecameOwned, OwnedContext, CurrentOwner));
        return OwnedContext;
    }
}