﻿using System;
using System.Collections.Generic;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Extensions;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuKill =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuKill =
            new Dictionary<string, List<RadialCheckRemove>>();

        public static void AddCustomButtonOnKillSubmenu(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuKill.Add(key, (value, externalCheck));

        public static bool RemoveCustomButtonOnKillSubmenu(string key) => _onSubmenuKill.Remove(key);

        public static void HideDefaultKillSubmenuItem(string key, string value, ShouldShowMenu callback = null) =>
            RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnSubmenuKill, key, value, callback);

        public static void ShowDefaultKillSubmenuItem(string key, string value) =>
            RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnSubmenuKill, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_KillMenu")]
    internal sealed class KillSubMenuPatch
    {
        // ReSharper disable InconsistentNaming
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature,
            CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var actionKill = Reflections.GetMenuItemAction("Action_Kill", __instance);

            if (actionKill != null &&
                RadialUIPlugin._removeOnSubmenuKill.CanShow("Kill Creature", miniId.ToString(), targetId.ToString()))
                map.AddItem(actionKill, "Kill Creature", icon: Icons.GetIconSprite("remove"), closeMenuOnActivate: true,
                    obj: ____selectedCreature);
            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onSubmenuKill, targetId);
        }
    }
}