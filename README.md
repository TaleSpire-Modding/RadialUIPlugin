# Radial UI Plugin
[![Push to nuget feed on release](https://github.com/TaleSpire-Modding/RadialUIPlugin/actions/workflows/release.yml/badge.svg)](https://github.com/TaleSpire-Modding/RadialUIPlugin/actions/workflows/release.yml) [![Doxygen GitHub Pages Deploy Action](https://github.com/TaleSpire-Modding/RadialUIPlugin/actions/workflows/doxygen-gh-pages.yml/badge.svg)](https://github.com/TaleSpire-Modding/RadialUIPlugin/actions/workflows/doxygen-gh-pages.yml)

This is a plugin for TaleSpire using BepInEx.

## Install

Currently you need to either follow the build guide down below or use the R2ModMan. 

## Usage
This plugin is specifically for developers to easily implement extra Radial buttons based on a entity properties.
Developers should reference the DLL for their own projects. This does not provide anything out of the box. You can find the developer documentation [here](https://talespire-modding.github.io/RadialUIPlugin/).

## How to Compile / Modify

Open ```RadialUIPlugin.sln``` in Visual Studio.

Build the project.

Browse to the newly created ```bin/Debug``` or ```bin/Release``` folders and copy the ```RadialUIPlugin.dll``` to ```Steam\steamapps\common\TaleSpire\BepInEx\plugins```

## Changelog
```
- 2.4.1: Fix suggested by Nekomancer
- 2.4.0: Started populating code to allow for grouping with creatures.
- 2.3.2: Fixed LOS with Moe
- 2.3.1: Fix melee attack
- 2.3.0: Documentation moved to https://talespire-modding.github.io/RadialUIPlugin/
- 2.2.0: Added in sys reflec checkers
- 2.1.0: GMBlock compatibility added.
- 2.0.0: Completed Refactor, Code has been optimized whilst keeping same Interface
```

## Shoutouts
Shoutout to my Patreons on https://www.patreon.com/HolloFox recognising your
mighty contribution to my caffeine addiciton:
- John Fuller
- [Tales Tavern](https://talestavern.com/) - MadWizard
- Joaqim Planstedt

Lord Ashes providing dependent code unify snippet controlling submenus.
