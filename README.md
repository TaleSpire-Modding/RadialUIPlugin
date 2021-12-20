# Radial UI Plugin

This is a plugin for TaleSpire using BepInEx.

## Install

Currently you need to either follow the build guide down below or use the R2ModMan. 

## Usage
This plugin is specifically for developers to easily implement extra Radial buttons based on a entity properties.
Developers should reference the DLL for their own projects. This does not provide anything out of the box.

## Example Usage
```csharp
	void Awake()
        {

	    // Adds Callbacks to append new mapmenu item
            AddOnCharacter(Guid, new MapMenu.ItemArgs
            {
                Action = Action,
                Title = "On Character",
                CloseMenuOnActivate = true
            }); // Callback comparator is optional

            AddOnCanAttack(Guid, new MapMenu.ItemArgs
            {
                Action = Action,
                Title = "On Can Attack",
                CloseMenuOnActivate = true
            }, Check);

            AddOnCantAttack(Guid, new MapMenu.ItemArgs
            {
                Action = Action,
                Title = "On Cant Attack",
                CloseMenuOnActivate = true
            }, Check);

            AddOnHideVolume(Guid, new MapMenu.ItemArgs
            {
                Action = Action,
                Title = "On HideVolume",
                CloseMenuOnActivate = true
            }, Check2);
        }

        private Boolean Check(NGuid selectedCreature, NGuid creatureTargetedFromRadial)
        {
            Debug.Log($"{selectedCreature},{creatureTargetedFromRadial}");
            return true;
        }

        private Boolean Check2(HideVolumeItem args2)
        {
            Debug.Log($"{args2}");
            return true;
        }

        private void Action(MapMenuItem args, object args2) => Debug.Log($"{args},{args2}");

```

## Loading a Sprite
Dimensions for sprites should be 32 by 32, Below is an example script supplied by LordAshes loading in 
an image to be used as an icon for the new Radial Component.
```csharp
string dir = "path to directory";
Texture2D tex = new Texture2D(32, 32);
tex.LoadImage(System.IO.File.ReadAllBytes(dir + "Images/Icons/KO.Png"));
Sprite icon = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
```
It is recommended to use the File Access Plugin supplied by Lord Ashes for a simple 1 liner:
```csharp
Sprite icon = LordAshes.FileAccessPlugin.Image.LoadSprite("update.png")
```

## How to Compile / Modify

Open ```RadialUIPlugin.sln``` in Visual Studio.

You will need to add references to:

```
* BepInEx.dll  (Download from the BepInEx project.)
* Bouncyrock.TaleSpire.Runtime (found in Steam\steamapps\common\TaleSpire\TaleSpire_Data\Managed)
* 0Harmony.dll
* UnityEngine.dll
* UnityEngine.CoreModule.dll
* UnityEngine.InputLegacyModule.dll 
* UnityEngine.UI
* Unity.TextMeshPro
```

Build the project.

Browse to the newly created ```bin/Debug``` or ```bin/Release``` folders and copy the ```RadialUIPlugin.dll``` to ```Steam\steamapps\common\TaleSpire\BepInEx\plugins```

## Changelog
- 2.0.5: Fix missed logic in extension.
- 2.0.4: Slight optimization and refactor repeating code.
- 2.0.3: Adjusted last creature targeted instantiation to prefix.
- 2.0.2: GetLastHideVolume is now supported.
- 2.0.1: HideVolume support
- 2.0.0: Completed Refactor, Code has been optimized whilst keeping same Interface
- 1.7.0: Added Stat Submenu and started converting to Harmony Patching.
- 1.6.1: Moved Repo
- 1.6.0: Refactor and Callback for Remove menu, provided by CodeRushed.
- 1.5.1: Fixed problem of infinite spawning of menus due to removal.
- 1.5.0: Fixed problem of patch 15 from BouncyRock
- 1.4.1: Fixed problem of dissapearing root
- 1.4.0: Add ability to remove existing buttons to replace them.
- 1.3.1: fixed backwards compatability due to paramater signature.
- 1.3.0: Expand Submenu to allow a checker
- 1.2.5: Privatized method and new method to return last creature selected from menu.
- 1.2.4: Image not found does not block radial from loading
- 1.2.3: bugfix
- 1.2.2: Add size submenu hook
- 1.2.1: Delay callback to fix forms
- 1.2.0: Add hooks to character submenus, Added code to manage submenus.
- 1.1.1: Add Docs on how to implement submenus
- 1.1.0: Removed Modding Utils and display on main menu
- 1.0.2: Fixed Issue on radial breaking upon leaving board
- 1.0.0: Initial release

## Shoutouts
Shoutout to my Patreons on https://www.patreon.com/HolloFox recognising your
mighty contribution to my caffeine addiciton:
- John Fuller
- [Tales Tavern](https://talestavern.com/) - MadWizard

Lord Ashes providing dependent code unify snippet controlling submenus.
