using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using Sentient.Players;
using System;
using UniRx;
using UnityEngine;

public class CustomMeYouVehicle_GameStateManager<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle, TCustomVehicleGameStateManager, TMYCustomVehicle_DM, TMYCustomVehicle_L, TMYCustomVehicle_T> : MonoBehaviour
    where TICustomVehicle : ICustomVehicle
    where TCustomVehicle : OwnableCustomVehicleContext<TICustomVehicle, TCustomVehicle, TMeYouCustomVehicle>, ISelectableVehicle, ITier1, TICustomVehicle
    where TMeYouCustomVehicle : CustomVehicleContext<TICustomVehicle>, IMeYou, ITier2, TICustomVehicle, IOwned<TMeYouCustomVehicle, MeYou, TCustomVehicle>, IOwned
    where TCustomVehicleGameStateManager : AbstractCustomVehicle_GameStateManager<TCustomVehicle, TICustomVehicle, TMeYouCustomVehicle>
    where TMYCustomVehicle_DM : BaseObjectContext, TICustomVehicle, IDeathmatch
    where TMYCustomVehicle_L : BaseObjectContext, TICustomVehicle, ILobby
    where TMYCustomVehicle_T : BaseObjectContext, TICustomVehicle, ITitle
{
    private TMeYouCustomVehicle _meYouVehicle;

    private MeYou_GameStateManager _meYouGsm;

    private TCustomVehicleGameStateManager _vehicleGsm;

    private BaseObjectContext _childContext;

    private GameState? _currentState;

    private bool _destroyingChild;

    [ComponentConstructor]
    private void Construct(TMeYouCustomVehicle meYouVehicle, MeYou_GameStateManager meYouGsm, TCustomVehicleGameStateManager vehicleGsm)
    {
        _meYouVehicle = meYouVehicle;
        _meYouGsm = meYouGsm;
        _vehicleGsm = vehicleGsm;
        this.WhenActivated(delegate (CompositeDisposable disposer)
        {
            meYouGsm.OnEvent.Subscribe(UpdateChildContainer).AddTo(disposer);
            vehicleGsm.OnEvent.Subscribe(UpdateChildContainer).AddTo(disposer);
            UpdateChildContainer();
        }).AddTo(base.gameObject);
    }

    private void UpdateChildContainer()
    {
        GameState? gameState = _meYouGsm.CurrentState;
        if (_vehicleGsm.CurrentState != gameState)
        {
            gameState = null;
        }

        if (!base.gameObject.activeInHierarchy || !base.enabled)
        {
            gameState = null;
        }

        if (gameState != _currentState)
        {
            CleanupChild();
            if (gameState.HasValue)
            {
                _currentState = gameState;
                _childContext = gameState.Value switch
                {
                    GameState.Deathmatch => CreateChild<TMYCustomVehicle_DM>(),
                    GameState.Lobby => CreateChild<TMYCustomVehicle_L>(),
                    GameState.Title => CreateChild<TMYCustomVehicle_T>(),
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }
    }

    private T CreateChild<T>() where T : BaseObjectContext
    {
        return SubContainerUtil.CreateFrom<T>(_meYouVehicle.AsPrimaryParent(parentChildToThis: true, linkChildsLifetimeToThis: true, OnChildDisposed), new AdditionalParent[2]
        {
            _meYouGsm.ChildContext_Untyped.Context.AsAdditionalParent(),
            _vehicleGsm.ChildContext_Untyped.Context.AsAdditionalParent()
        });
    }

    private void OnChildDisposed()
    {
        if (!_destroyingChild && (bool)this)
        {
            _currentState = null;
            UpdateChildContainer();
        }
    }

    private void CleanupChild()
    {
        _destroyingChild = true;
        _childContext.SafeDestroy();
        _destroyingChild = false;
    }
}
