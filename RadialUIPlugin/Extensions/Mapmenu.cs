﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.Unmanaged;
using DataModel;
using NetMiniZ;

namespace RadialUI.Extensions
{
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
            var miniId = LocalClient.SelectedCreatureId.Value;
            foreach (var key in list.Keys.Where(key => list[key].Item2 == null || list[key].Item2(miniId, targetId)))
            {
                list[key].Item1.Obj = miniId;
                map.AddItem(list[key].Item1);
            }
        }

        /// <summary>
        /// Generic Method to add many menus to a mapmenu for a hide volume from an existing data source.
        /// </summary>
        /// <param name="map">the mapmenu that is getting new items appended too.</param>
        /// <param name="list">the data source which the menus originate from.</param>
        /// <param name="selectedVolume">The specific hide volume that's being targeted.</param>
        internal static void AddItems(this MapMenu map, Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> list, HideVolumeItem selectedVolume)
        {
            foreach (var key in list.Keys.Where(key => list[key].Item2 == null || list[key].Item2(selectedVolume)))
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
            foreach (var key in list.Keys.Where(key => list[key].Item2 == null || list[key].Item2(selectedVolume)))
            {
                list[key].Item1.Obj = list[key].Item1.Obj ?? selectedVolume;
                map.AddItem(list[key].Item1);
            }
        }
    }
}
