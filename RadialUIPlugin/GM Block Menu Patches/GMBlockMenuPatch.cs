using System;
using System.Collections.Generic;
using BepInEx;
using DataModel;
using HarmonyLib;
using RadialUI.Extensions;
using TaleSpire.GameMaster.Blocks;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnGMBlock = new Dictionary<string, List<RadialCheckRemove>>();
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<AtmosphereBlock, bool>)> _onGMBlock = new Dictionary<string, (MapMenu.ItemArgs, Func<AtmosphereBlock, bool>)>();

        public static void HideDefaultGMBlockMenuItem(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnGMBlock, key, value, callback);
        public static void UnHideDefaultGMBlockMenuItem(string key, string value) => RemoveRemoveOn(_removeOnGMBlock, key, value);
        
        public static void AddCustomButtonGMBlock(string key, MapMenu.ItemArgs value, Func<AtmosphereBlock, bool> externalCheck = null) => _onGMBlock.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonGMBlock(string key) => _onGMBlock.Remove(key);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(GMBlockInteractMenuBoardTool), "Begin")]
    internal sealed class GMBlockMenuPatch
    {
        // ReSharper disable InconsistentNaming
        internal static void Postfix(ref GMBlockInteractMenuBoardTool __instance)
        {
            var deleteBlock = Reflections.GetMenuItemAction("DeleteBlock", __instance);

            var map = MapMenuManager.OpenMenu(GMBlockInteractMenuBoardTool.block.WorldPosition, true);
            GMBlockInteractMenuBoardTool.block.OnOpenMenu(map);

            if (deleteBlock != null && RadialUIPlugin._removeOnGMBlock.CanAdd("Delete", null, null))
                map.AddItem((deleteBlock), "Delete", icon: Icons.GetIconSprite("delete"), closeMenuOnActivate: true);
        }
    }

    [HarmonyPatch(typeof(GMBlockButtonAtmosphere), "OnOpenMenu")]
    internal sealed class GMBlockButtonPatch
    {
        internal static bool Prefix(ref MapMenu map, ref GMBlockButtonAtmosphere __instance)
        {
            var onApply = Reflections.GetMenuItemAction("OnApply", __instance);
            var onEdit = Reflections.GetMenuItemAction("OnEdit", __instance);

            if (onApply != null && RadialUIPlugin._removeOnGMBlock.CanAdd("Apply", null, null))
                map.AddItem(onApply, "Apply", icon: Icons.GetIconSprite("apply"), closeMenuOnActivate: true);
            if (onEdit != null && RadialUIPlugin._removeOnGMBlock.CanAdd("Edit", null, null))
                map.AddItem(onEdit, "Edit", icon: Icons.GetIconSprite("edit"), closeMenuOnActivate: true);

            return false;
        }

        internal static void Postfix(MapMenu map, GMDataBlockBase ____base)
        {
            map.AddItems(RadialUIPlugin._onGMBlock, ____base.AtmosphereBlock);
        }
    }
}
