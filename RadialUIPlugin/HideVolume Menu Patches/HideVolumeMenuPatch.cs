using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using RadialUI.Extensions;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        // Hide Volumes
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> _onHideVolumeCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnHideVolume = new Dictionary<string, List<RadialCheckRemove>>();

        public static void AddOnHideVolume(string key, MapMenu.ItemArgs value, Func<HideVolumeItem, bool> externalCheck = null) => _onHideVolumeCallback.Add(key, (value, externalCheck));
        public static bool RemoveOnHideVolume(string key) => _onHideVolumeCallback.Remove(key);

        // Add RemoveOn
        public static void AddOnRemoveHideVolume(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnHideVolume, key, value, callback);

        // Remove RemoveOn
        public static void RemoveOnRemoveHideVolume(string key, string value) => RemoveRemoveOn(_removeOnHideVolume, key, value);

        internal static HideVolumeItem lastHideVolume;

        public static HideVolumeItem GetLastRadialHideVolume()
        {
            return lastHideVolume;
        }
    }
}

namespace RadialUI.HideVolume_Menu_Patches
{
    [HarmonyPatch(typeof(GMHideVolumeMenuBoardTool), "Begin")]
    internal class HideVolumeMenuPatch
    {
        internal static bool Prefix(HideVolumeItem ____selectedVolume)
        {
            RadialUIPlugin.lastHideVolume = ____selectedVolume; 
            return true;
        }

        internal static void Postfix(HideVolumeItem ____selectedVolume, Vector3 ____selectedPos, ref GMHideVolumeMenuBoardTool __instance)
        {
            MapMenu mapMenu = MapMenuManager.OpenMenu(____selectedPos, true);
            
            var toggleTiles = Reflections.GetMenuItemAction("ToggleTiles", __instance);
            var deleteBlock = Reflections.GetMenuItemAction("DeleteBlock", __instance);
            
            // Add checker to see if below are removed
            mapMenu.AddItem(toggleTiles, "Toggle Visibility");
            mapMenu.AddItem(deleteBlock, "Delete", closeMenuOnActivate: true);
            
            mapMenu.AddItems(RadialUIPlugin._onHideVolumeCallback, ____selectedVolume);
        }
    }
}