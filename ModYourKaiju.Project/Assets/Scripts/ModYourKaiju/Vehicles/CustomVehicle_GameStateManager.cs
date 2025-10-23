using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using System;
using UnityEngine;

public class CustomVehicle_GameStateManager<TCustomVehicle, TICustomVehicle, TMeYouCustomVehicle, TCustomVehicle_DM, TCustomVehicle_L, TCustomVehicle_T> : AbstractCustomVehicle_GameStateManager<TCustomVehicle, TICustomVehicle, TMeYouCustomVehicle>
    where TICustomVehicle : ICustomVehicle
    where TCustomVehicle : OwnableCustomVehicleContext<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>, ISelectableVehicle, ITier1, TICustomVehicle
    where TMeYouCustomVehicle : CustomVehicleContext<TICustomVehicle>, IMeYou, ITier2, TICustomVehicle, IOwned<TMeYouCustomVehicle, MeYou, TCustomVehicle>, IOwned
    where TCustomVehicle_DM : BaseObjectContext, TICustomVehicle, IDeathmatch
    where TCustomVehicle_L : BaseObjectContext, TICustomVehicle, ILobby
    where TCustomVehicle_T : BaseObjectContext, TICustomVehicle, ITitle
{
    public void Start()
    {
        Debug.Log($"GSM for {typeof(TCustomVehicle)} started");
    }

    protected override BaseObjectContext CreateChildContext(GameState state)
    {
        Debug.Log($"Creating child context for {typeof(TCustomVehicle)}");
        return state switch
        {
            GameState.Deathmatch => SubContainerUtil.CreateFrom<TCustomVehicle_DM>(_parentContext.AsPrimaryParent(), new AdditionalParent[1] { _globalGameState.ChildContext_Untyped.Context.AsAdditionalParent() }),
            GameState.Lobby => SubContainerUtil.CreateFrom<TCustomVehicle_L>(_parentContext.AsPrimaryParent(), new AdditionalParent[1] { _globalGameState.ChildContext_Untyped.Context.AsAdditionalParent() }),
            GameState.Title => SubContainerUtil.CreateFrom<TCustomVehicle_T>(_parentContext.AsPrimaryParent(), new AdditionalParent[1] { _globalGameState.ChildContext_Untyped.Context.AsAdditionalParent() }),
            _ => throw new ArgumentOutOfRangeException("state", state, null),
        };
    }
}
