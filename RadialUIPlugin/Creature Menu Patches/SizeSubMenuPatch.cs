using System;
using System.Collections.Generic;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Extensions;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuSize =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuSize =
            new Dictionary<string, List<RadialCheckRemove>>();

        public static void AddCustomButtonOnSizeSubmenu(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuSize.Add(key, (value, externalCheck));

        public static bool RemoveCustomButtonOnSizeSubmenu(string key) => _onSubmenuSize.Remove(key);

        public static void HideDefaultSizeSubmenuItem(string key, string value, ShouldShowMenu callback = null) =>
            RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnSubmenuSize, key, value, callback);

        public static void ShowDefaultSizeSubmenuItem(string key, string value) =>
            RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnSubmenuSize, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_SetSize")]
    internal sealed class SizeSubMenuPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            NGuid miniId = LocalClient.SelectedCreatureId.Value;
            NGuid targetId = ____selectedCreature.CreatureId.Value;

            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "0.5x0.5", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "1x1", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "2x2", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "3x3", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "4x4", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "6x6", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "8x8", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "10x10", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuSize, "12x12", miniId.ToString(), targetId.ToString());

            map.AddItems(RadialUIPlugin._onSubmenuSize, targetId);
        }
    }
}