using System;
using System.Collections.Generic;
using Bounce.Unmanaged;
using HarmonyLib;
using PluginUtilities;
using RadialUI.Extensions;

namespace RadialUI
{
    public partial class RadialUIPlugin : DependencyUnityPlugin
    {
        // Internal DB
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onStatCallback =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, (string, ShouldShowMenu)> _hideStat =
            new Dictionary<string, (string, ShouldShowMenu)>();

        // Public Accessor
        public static void AddCustomCharacterSubmenuStat(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null) => _onStatCallback.Add(key, (value, externalCheck));

        public static bool RemoveCustomCharacterSubmenuStat(string key) => _onStatCallback.Remove(key);

        /// <summary>
        /// Prevent a default stat from appearing
        /// </summary>
        /// <param name="key">Id of plugin that wants to hide stat</param>
        /// <param name="value">0 to 7 (id of stat being hidden)</param>
        /// <param name="callback">Optional callback to hide stat</param>
        public static void HideDefaultCharacterSubmenuStat(string key, string value, ShouldShowMenu callback = null) =>
            _hideStat.Add(key, (value, callback));

        public static void HideDefaultCharacterSubmenuStat(string key, RadialCheckRemove remove) =>
            _hideStat.Add(key, (remove.TitleToRemove, remove.ShouldRemoveCallback));

        /// <summary>
        /// Re-allows a stat to show (useful for script engine on unpatch)
        /// </summary>
        /// <param name="key">Id of plugin that hid the stat</param>
        /// <returns></returns>
        public static bool ShowDefaultCharacterSubmenuStat(string key) => _hideStat.Remove(key);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_Stats")]
    internal sealed class StatSubMenuPatch
    {
        // ReSharper disable InconsistentNaming

        public static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            NGuid miniId = LocalClient.SelectedCreatureId.Value;
            NGuid targetId = ____selectedCreature.CreatureId.Value;

            map.AddItems(RadialUIPlugin._onStatCallback, targetId);
        }
    }
}