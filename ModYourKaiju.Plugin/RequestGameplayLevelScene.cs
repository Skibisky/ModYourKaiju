using Sentient;
using Sentient.Injection;
using Sentient.MeYouKaiju;
using UnityEngine;

namespace ModYourKaiju.Plugin;

public class RequestGameplayLevelScene : MonoBehaviour
{
    private GameplayLevelLoadService _gameplayLevelLoadService;
    private bool _hasRequested = false;
    private string theScene;

    [ComponentConstructor]
    private void Construct(GameplayLevelLoadService gameplayLevelLoadService)
    {
        theScene = GameplayLevelLoaderServicePatch.targetLevel;
        _gameplayLevelLoadService = gameplayLevelLoadService;
    }

    [UnityCallback]
    private void OnEnable()
    {
        if(_hasRequested)
            return;
            
        _gameplayLevelLoadService.RequestGameplayLevelLoad(theScene);
    }

    [UnityCallback]
    private void OnDisable()
    {
        _gameplayLevelLoadService.RequestGameplayLevelUnload(theScene);
    }
}