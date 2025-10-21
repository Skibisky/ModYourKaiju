# Mod Your Kaiju

Mod Your Kaiju for [Me, You and Kaiju](https://store.steampowered.com/app/3092200/Me_You_and_Kaiju/) (Myk Myk) is a [BepInEx](https://github.com/BepInEx/BepInEx) plugin that provides a rough framework for adding Levels, Vehicles, and Kaiju into the game.

## Repo Structure

ModYourKaiju.Plugin is the BepInEx plugin which locates the Dependency Injection used in MYK and installs itself and any mods built against the Api.

ModYourKaiju.Project is a simple Unity Project which is configured to create asset bundles and assemblies able to be loaded into the game.

ModYourKaiju.Api is built out of the Unity project to provide an API to build mods against that also has types visible to ModYourKaiju.Plugin.


## Setup

The Plugin project need assembly references to the dlls contained in your install of MYK (see below) but the references in the csproj are redirected to the `plugins` folder of the Project.

Working with the Unity project, you will need to copy-paste the dlls in the list below into the `plugins` folder. 

### DLL list

The DLLs can be found inside `steamapps\common\Me, You and Kaiju_Data\Managed`, probably just copy all the non-Unity ones until the project stops complaining. The plugin depends on at least:

- Sentient.Injection
- Sentient.MeYouKaiju
- Sentient.Util
- Zenject
- Zenject-usage

## Getting Started

If you are looking to add something to MYK, here is the status of ModYourKaiju injecting content.

Note:  
Because the game is using CodeGen and Zenject, there is a lot of boilerplate C# classes that need to be defined and hooked up, use an example to start.

### Vehicles

Mostly done. Look at the `vehicle.biplane` folder in `Mods`.

The fun thing is making something like `BiplaneController` and `ConnectInputToBiplane`.

This is slapped on the vehicle in `BiplaneModule` where there is other options for configuration.

Make sure you have a Prefab similar to `Biplane.prefab` which has the base custom vehicle script `Biplane`, and `BiplaneController`.

This prefab should be in an AssetBundle which you can build with the `Assets/Build AssetBundles` menu.

In order for the whole thing to work, everything has to be loaded and configured in `BiplaneLoader` where it loads the AssetBundle and does the final linking of in-game assets to your prefabs and scripts.

Make sure that Unity compiles successfully, because you will be grabbing the dll from `ModYourKaiju.Project\Library\ScriptAssemblies` (eg. `vehicle.biplane.dll`).

To load the mod, go to `Me, You and Kaiju\BepInEx\plugins\Mykmyk` and make a folder for your mod (eg. `Biplane`), put the assetbundle here and also the dll from above.

#### Weapons

NYI

### Kaiju

NYI

#### Powers

NYI

### Levels

Upcoming

### Audio

NYI

### Game Modes

NYI