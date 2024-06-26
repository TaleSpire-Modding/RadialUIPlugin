﻿using System;
using System.Collections.Generic;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Extensions;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuGm =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuGm =
            new Dictionary<string, List<RadialCheckRemove>>();

        // Same Methods, different Signatures
        public static void AddCustomButtonGMSubmenu(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null)
        {
            _onSubmenuGm.Add(key, (value, externalCheck));

            MapMenu.ItemArgs items = new MapMenu.ItemArgs
            {
                Action = (MapMenuItem, Object) =>
                {
                    if (LocalClient.TryGetLassoedCreatureIds(out var ids))
                    {
                        int num = ids.Length;
                        for (int i = 0; i < num; i++)
                        {
                            value.Action(MapMenuItem, ids[i].Value);
                        }
                    }
                },
                Title = value.Title,
                Icon = value.Icon,
                SubValueText = value.SubValueText,
                Scale = value.Scale,
                CloseMenuOnActivate = value.CloseMenuOnActivate,
                FadeName = value.FadeName,
                ValueText = value.ValueText,
                Obj = value.Obj,
            };

            _onGroupSubmenuGm.Add(key, (items, externalCheck));
        }
        public static bool RemoveCustomButtonGMSubmenu(string key)
        {
            return _onSubmenuGm.Remove(key) && _onGroupSubmenuGm.Remove(key);
        }
        public static void HideDefaultEmotesGMItem(string key, string value, ShouldShowMenu callback = null)
        {
            AddRemoveOn(_removeOnSubmenuGm, key, value, callback);
        }
        public static void UnHideDefaultGMSubmenuItem(string key, string value)
        {
            RemoveRemoveOn(_removeOnSubmenuGm, key, value);
        }
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_GMTools")]
    internal sealed class GMToolsSubMenuPatch
    {
        // ReSharper disable InconsistentNaming
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature,
            CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var menuSetCreaturePlayerPermission =
                Reflections.GetMenuAction("Menu_SetCreaturePlayerPermission", __instance);
            var menuRenameCreature = Reflections.GetMenuItemAction("Menu_RenameCreature", __instance);
            var menuMakeNotUnique = Reflections.GetMenuItemAction("Menu_MakeNotUnique", __instance);
            var menuMakeUnique = Reflections.GetMenuItemAction("Menu_MakeUnique", __instance);
            var menuSetSize = Reflections.GetMenuAction("Menu_SetSize", __instance);
            var baseColorMenu = Reflections.GetMenuAction("BaseColor_Menu", __instance);
            var permissionMenuStyle = CampaignSessionManager.PlayersInfo.Count >= 20
                ? MapMenu.MenuType.SUBROOT
                : MapMenu.MenuType.BRANCH;

            if (menuSetCreaturePlayerPermission != null &&
                RadialUIPlugin._removeOnSubmenuGm.CanAdd("", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(permissionMenuStyle, menuSetCreaturePlayerPermission, "Player Permission",
                    icon: Icons.GetIconSprite("permission"));
            if (menuRenameCreature != null &&
                RadialUIPlugin._removeOnSubmenuGm.CanAdd("Rename", miniId.ToString(), targetId.ToString()))
                map.AddItem(menuRenameCreature, "Rename", icon: Icons.GetIconSprite("rename"));
            if (menuMakeNotUnique != null && ____selectedCreature.IsUnique &&
                RadialUIPlugin._removeOnSubmenuGm.CanAdd("Make Not Unique", miniId.ToString(), targetId.ToString()))
                map.AddItem(menuMakeNotUnique, "Make Not Unique", icon: Icons.GetIconSprite("dungeonmaster"),
                    closeMenuOnActivate: true);
            else if (menuMakeUnique != null &&
                     RadialUIPlugin._removeOnSubmenuGm.CanAdd("Make Unique", miniId.ToString(), targetId.ToString()))
                map.AddItem(menuMakeUnique, "Make Unique", icon: Icons.GetIconSprite("dungeonmaster"),
                    closeMenuOnActivate: true);
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
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onSubmenuGm, targetId);
        }
    }
}