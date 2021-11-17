using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using RadialUI.Reflection_Extensions;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        // Hide Volumes
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> _onHideVolumeCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)>();

        public static void AddOnHideVolume(string key, MapMenu.ItemArgs value, Func<HideVolumeItem, bool> externalCheck = null) => _onHideVolumeCallback.Add(key, (value, externalCheck));
        public static bool RemoveOnHideVolume(string key) => _onHideVolumeCallback.Remove(key);
    }
}

namespace RadialUI.HideVolume_Menu_Patches
{
    [HarmonyPatch(typeof(GMHideVolumeMenuBoardTool), "Begin")]
    internal class HideVolumeMenuPatch
    {
        internal static bool Prefix(HideVolumeItem ____selectedVolume)
        {
            var targetId = ____selectedVolume;
            return true;
        }

        internal static void Postfix(HideVolumeItem ____selectedVolume, Vector3 ____selectedPos, ref GMHideVolumeMenuBoardTool __instance)
        {
            RadialUIPlugin.lastHideVolume = ____selectedVolume;
            MapMenu mapMenu = MapMenuManager.OpenMenu(____selectedPos, true);
            
            var toggleTiles = Reflections.GetMenuItemAction("ToggleTiles", __instance);
            var deleteBlock = Reflections.GetMenuItemAction("DeleteBlock", __instance);
            
            // Add checker to see if below are removed
            mapMenu.AddItem(toggleTiles, "Toggle Visibility");
            mapMenu.AddItem(deleteBlock, "Delete", closeMenuOnActivate: true);
            
            // Add new methods here
            foreach (var key in RadialUIPlugin._onHideVolumeCallback.Keys.Where(key => RadialUIPlugin._onHideVolumeCallback[key].Item2 == null || RadialUIPlugin._onHideVolumeCallback[key].Item2(____selectedVolume)))
            {
                mapMenu.AddItem(RadialUIPlugin._onHideVolumeCallback[key].Item1);
            }
        }
    }
}