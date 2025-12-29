using System;
using System.Collections.Generic;
using DataModel;
using HarmonyLib;
using PluginUtilities;
using RadialUI.Extensions;
using TaleSpire.GameMaster.Blocks;

namespace RadialUI
{
    public partial class RadialUIPlugin : DependencyUnityPlugin
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
        public static void Postfix(ref GMBlockInteractMenuBoardTool __instance)
        {
            MapMenu map = Mapmenu.GetMapMenu();
            map.TryHideItem(RadialUIPlugin._removeOnGMBlock, "Delete", null, null);
        }
    }

    [HarmonyPatch(typeof(GMBlockButtonAtmosphere), "OnOpenMenu")]
    internal sealed class GMBlockButtonPatch
    {
        public static void Postfix(MapMenu map, GMDataBlockBase ____base)
        {
            // Hide following items if conditions met and able
            map.TryHideItem(RadialUIPlugin._removeOnGMBlock, "Apply", null, null);
            map.TryHideItem(RadialUIPlugin._removeOnGMBlock, "Edit", null, null);

            map.AddItems(RadialUIPlugin._onGMBlock, ____base.AtmosphereBlock);
        }
    }
}
