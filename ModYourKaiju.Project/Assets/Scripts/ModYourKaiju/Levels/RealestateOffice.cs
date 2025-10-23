using Sentient.MeYouKaiju;
using System;
using System.Collections.Generic;
using System.Linq;

public static class RealestateOffice
{
    public static Dictionary<string, string> knownProperties = new();

    public static bool CheckLevel(string fancyName)
    {
        return knownProperties.ContainsKey(fancyName);
    }

    public static void RegisterLevel(string fancyName, string sceneAssetBundlePath)
    {
        if (knownProperties.ContainsKey(fancyName))
            throw new InvalidOperationException($"A level has already been added called '{fancyName}'");

        knownProperties[fancyName] = sceneAssetBundlePath;
    }

    public static GameplaySceneReference GetReferenceFor(string fancyName)
    {
        if (!knownProperties.ContainsKey(fancyName))
            throw new ArgumentOutOfRangeException($"No level called {fancyName} registered.");
        return new GameplaySceneReference()
        {
            Name = fancyName,
            SceneReference = new SceneReference()
            {
                ScenePath = knownProperties[fancyName]
            }
        };
    }

    public static LevelSelectControlOption GetSelectOptionFor(string fancyName)
    {
        if (!knownProperties.ContainsKey(fancyName))
            throw new ArgumentOutOfRangeException($"No level called {fancyName} registered.");
        return new LevelSelectControlOption()
        {
            Title = fancyName,
        };
    }

    public static IEnumerable<GameplaySceneReference> GetReferences()
    {
        return knownProperties.Select(k => new GameplaySceneReference()
        {
            Name = k.Key,
            SceneReference = new SceneReference()
            {
                ScenePath = k.Value
            }
        });
    }

    public static IEnumerable<LevelSelectControlOption> GetSelectOptions()
    {
        return knownProperties.Select(k => new LevelSelectControlOption()
        {
            Title = k.Key,
        });
    }

}
