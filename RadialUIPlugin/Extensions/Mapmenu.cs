using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.Unmanaged;
using DataModel;
using HarmonyLib;

namespace RadialUI.Extensions
{
    [HarmonyPatch(typeof(MapMenuManager), nameof(MapMenuManager.OpenMenu))]
    public static class MapMenuManagerPatch
    {
        public static MapMenu mapMenu;

        public static void Postfix(ref MapMenu __result)
        {
            mapMenu = __result;
            //MapMenuAddItemPatch.mapMenuItems.Clear();
        }
    }

    public static class Mapmenu
    {
        /// <summary>
        /// Generic Method to add many menus to a mapmenu for a creature from an existing data source.
        /// </summary>
        /// <param name="map">the mapmenu that is getting new items appended too.</param>
        /// <param name="list">the data source which the menus originate from.</param>
        /// <param name="targetId">The target creature the mapmenu is targeting.</param>
        internal static void AddItems(this MapMenu map, Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> list, NGuid targetId)
        {
            NGuid miniId = LocalClient.SelectedCreatureId.Value;
            foreach ((MapMenu.ItemArgs, Func<NGuid, NGuid, bool>) entry in list.Values.Where(entry => entry.Item2 == null || entry.Item2(miniId, targetId)))
            {
                entry.Item1.Obj = miniId;
                map.AddItem(entry.Item1);
            }
        }

        /// <summary>
        /// Returns opened map menu
        /// </summary>
        /// <returns></returns>
        internal static MapMenu GetMapMenu()
        {
            RadialUIPlugin.logger.LogInfo($"Fetching MapMenu: {MapMenuManagerPatch.mapMenu != null}");
            return MapMenuManagerPatch.mapMenu;
        }

        internal static bool TryHideItem(this MapMenu map, Dictionary<string, List<RadialCheckRemove>> checker, string itemName, string miniId, string targetId)
        {
            // Early return as not implemented
            return TryHideItem(map, itemName);
            /*
            var mapMepItems = map.transform.Find("_MAP");
            mapMepItems.Children().Where(c => c.Find("TXT_Title"));
            */
        }

        internal static bool TryHideItem(this MapMenu map, string itemName)
        {
            // Early return as not implemented
            return false;
            /*
            var mapMepItems = map.transform.Find("_MAP");
            mapMepItems.Children().Where(c => c.Find("TXT_Title"));
            */
        }

        /// <summary>
        /// Generic Method to add many menus to a mapmenu for a hide volume from an existing data source.
        /// </summary>
        /// <param name="map">the mapmenu that is getting new items appended too.</param>
        /// <param name="list">the data source which the menus originate from.</param>
        /// <param name="selectedVolume">The specific hide volume that's being targeted.</param>
        internal static void AddItems(this MapMenu map, Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> list, HideVolumeItem selectedVolume)
        {
            foreach (string key in list.Keys.Where(key => list[key].Item2 == null || list[key].Item2(selectedVolume)))
            {
                map.AddItem(list[key].Item1);
            }
        }

        /// <summary>
        /// Generic Method to add many menus to a mapmenu for a hide volume from an existing data source.
        /// </summary>
        /// <param name="map">the mapmenu that is getting new items appended too.</param>
        /// <param name="list">the data source which the menus originate from.</param>
        /// <param name="selectedVolume">The specific hide volume that's being targeted.</param>
        internal static void AddItems(this MapMenu map, Dictionary<string, (MapMenu.ItemArgs, Func<AtmosphereBlock, bool>)> list, AtmosphereBlock selectedVolume)
        {
            foreach (string key in list.Keys.Where(key => list[key].Item2 == null || list[key].Item2(selectedVolume)))
            {
                list[key].Item1.Obj = list[key].Item1.Obj ?? selectedVolume;
                map.AddItem(list[key].Item1);
            }
        }
    }
}
