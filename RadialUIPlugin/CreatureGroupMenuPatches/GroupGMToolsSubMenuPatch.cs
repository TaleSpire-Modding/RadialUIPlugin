﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Extensions;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onGroupSubmenuGm =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnGroupSubmenuGm =
            new Dictionary<string, List<RadialCheckRemove>>();

    }
}

namespace RadialUI.CreatureGroupMenuPatches
{
    [HarmonyPatch(typeof(CreatureGroupMenuBoardTool), "Menu_GMTools")]
    internal sealed class GroupGMToolsSubMenuPatch
    {
        // ReSharper disable InconsistentNaming
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature,
            CreatureGroupMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var menuSetSize = Reflections.GetMenuAction("Menu_SetSize", __instance);
            var baseColorMenu = Reflections.GetMenuAction("BaseColor_Menu", __instance);
            
            if (menuSetSize != null &&
                RadialUIPlugin._removeOnSubmenuGm.CanAdd("Set Size", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(MapMenu.MenuType.BRANCH, menuSetSize, "Set Size",
                    icon: Icons.GetIconSprite("creaturesize"));
            
            if (baseColorMenu != null &&
                RadialUIPlugin._removeOnSubmenuGm.CanAdd("BaseColor", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(MapMenu.MenuType.BRANCH, baseColorMenu, "BaseColor",
                    icon: Icons.GetIconSprite("basecolor"));

            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            map.AddItems(RadialUIPlugin._onGroupSubmenuGm, LocalClient.SelectedCreatureId.Value);
        }
    }
}