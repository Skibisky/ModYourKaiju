using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;

public abstract class AbstractCustomVehicle_GameStateManager<TCustomVehicle, TICustomVehicle, TMeYouCustomVehicle> : SubGameStateManager<Global_GameStateManager, TCustomVehicle, BaseObjectContext>
    where TICustomVehicle : ICustomVehicle
    where TCustomVehicle : OwnableCustomVehicleContext<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>, ISelectableVehicle, ITier1, TICustomVehicle
    where TMeYouCustomVehicle : CustomVehicleContext<TICustomVehicle>, IMeYou, ITier2, TICustomVehicle, IOwned<TMeYouCustomVehicle, MeYou, TCustomVehicle>, IOwned
{

}
