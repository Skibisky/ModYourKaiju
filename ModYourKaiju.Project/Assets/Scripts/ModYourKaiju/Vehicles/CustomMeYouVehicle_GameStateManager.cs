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
    where TMYCustomVehicle_DM : BaseObjectContext, TICustomVehicle, IDeathmatch, IMeYou
    where TMYCustomVehicle_L : BaseObjectContext, TICustomVehicle, ILobby, IMeYou
    where TMYCustomVehicle_T : BaseObjectContext, TICustomVehicle, ITitle, IMeYou
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
        Debug.Log($"Constructing MYGSM for {typeof(TCustomVehicle)}");
        _meYouVehicle = meYouVehicle;
        _meYouGsm = meYouGsm;
        _vehicleGsm = vehicleGsm;
        this.WhenActivated(delegate (CompositeDisposable disposer)
        {
            Debug.Log($"MYGSM {typeof(TCustomVehicle).Name} activated");
            meYouGsm.OnEvent.Subscribe(UpdateChildContainer).AddTo(disposer);
            vehicleGsm.OnEvent.Subscribe(UpdateChildContainer).AddTo(disposer);
            UpdateChildContainer();
        }).AddTo(base.gameObject);
    }

    private void UpdateChildContainer()
    {
        Debug.Log($"MYGSM {typeof(TCustomVehicle).Name} update child");
        GameState? gameState = _meYouGsm.CurrentState;
        if (_vehicleGsm.CurrentState != gameState)
            gameState = null;

        if (!base.gameObject.activeInHierarchy || !base.enabled)
            gameState = null;

        if (gameState == _currentState)
            return;

        CleanupChild();
        if (gameState.HasValue)
        {
            Debug.Log($"MYGSM {typeof(TCustomVehicle).Name} for {gameState.Value}");

            _currentState = gameState;
            _childContext = gameState.Value switch
            {
                GameState.Deathmatch => CreateChild<TMYCustomVehicle_DM>(),
                GameState.Lobby => CreateChild<TMYCustomVehicle_L>(),
                GameState.Title => CreateChild<TMYCustomVehicle_T>(),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
        else
        {
            Debug.LogWarning($"MYGSM {typeof(TCustomVehicle).Name} no gamestate");
        }
    }

    private T CreateChild<T>() where T : BaseObjectContext
    {
        Debug.Log($"MYGSM {typeof(TCustomVehicle).Name} create child {typeof(T).Name}");
        return SubContainerUtil.CreateFrom<T>(_meYouVehicle.AsPrimaryParent(parentChildToThis: true, linkChildsLifetimeToThis: true, OnChildDisposed), new AdditionalParent[2]
        {
            _meYouGsm.ChildContext_Untyped.Context.AsAdditionalParent(),
            _vehicleGsm.ChildContext_Untyped.Context.AsAdditionalParent()
        });
    }

    private void OnChildDisposed()
    {
        Debug.Log($"MYGSM {typeof(TCustomVehicle).Name} dispose child");
        if (!_destroyingChild && (bool)this)
        {
            _currentState = null;
            UpdateChildContainer();
        }
    }

    private void CleanupChild()
    {
        Debug.Log($"MYGSM {typeof(TCustomVehicle).Name} cleanup child");
        _destroyingChild = true;
        _childContext.SafeDestroy();
        _destroyingChild = false;
    }
}
