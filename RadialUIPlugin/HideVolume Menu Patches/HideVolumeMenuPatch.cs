using System;
using System.Collections.Generic;
using DataModel;
using HarmonyLib;
using PluginUtilities;
using RadialUI.Extensions;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : DependencyUnityPlugin<RadialUIPlugin>
    {
        // Hide Volumes
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<HideVolume, bool>)> _onHideVolumeCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<HideVolume, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnHideVolume = new Dictionary<string, List<RadialCheckRemove>>();

        public static void AddOnHideVolume(string key, MapMenu.ItemArgs value, Func<HideVolume, bool> externalCheck = null) => _onHideVolumeCallback.Add(key, (value, externalCheck));
        public static bool RemoveOnHideVolume(string key) => _onHideVolumeCallback.Remove(key);

        // Add RemoveOn
        public static void AddOnRemoveHideVolume(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnHideVolume, key, value, callback);
        public static void RemoveOnRemoveHideVolume(string key, string value) => RemoveRemoveOn(_removeOnHideVolume, key, value);

        internal static HideVolume lastHideVolume;

        public static HideVolume GetLastRadialHideVolume()
        {
            return lastHideVolume;
        }
    }
}

namespace RadialUI.HideVolume_Menu_Patches
{
    [HarmonyPatch(typeof(GMHideVolumeMenuBoardTool), "Begin")]
    internal sealed class HideVolumeMenuPatch
    {
        // ReSharper disable InconsistentNaming
        internal static bool Prefix(HideVolume ____selectedVolume)
        {
            RadialUIPlugin.lastHideVolume = ____selectedVolume;
            return true;
        }

        public static void Postfix(HideVolume ____selectedVolume, Vector3 ____selectedPos, ref GMHideVolumeMenuBoardTool __instance)
        {
            MapMenu mapMenu = Mapmenu.GetMapMenu();

            mapMenu.TryHideItem(RadialUIPlugin._removeOnHideVolume, "Toggle Visibility", null, null);
            mapMenu.TryHideItem(RadialUIPlugin._removeOnHideVolume, "Filters", null, null);
            mapMenu.TryHideItem(RadialUIPlugin._removeOnHideVolume, "Delete", null, null);

            mapMenu.AddItems(RadialUIPlugin._onHideVolumeCallback, ____selectedVolume);
        }
    }
}