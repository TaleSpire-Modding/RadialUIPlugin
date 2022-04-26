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
        // Internal DB
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onStatCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, (string,ShouldShowMenu)> _hideStat = new Dictionary<string, (string, ShouldShowMenu)>();

        // Public Accessor
        public static void AddCustomCharacterSubmenuStat(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onStatCallback.Add(key, (value, externalCheck));
        public static bool RemoveCustomCharacterSubmenuStat(string key) => _onStatCallback.Remove(key);

        /// <summary>
        /// Prevent a default stat from appearing
        /// </summary>
        /// <param name="key">Id of plugin that wants to hide stat</param>
        /// <param name="value">0 to 7 (id of stat being hidden)</param>
        /// <param name="callback">Optional callback to hide stat</param>
        public static void HideDefaultCharacterSubmenuStat(string key, string value, ShouldShowMenu callback = null) => _hideStat.Add( key, (value, callback));
        public static void HideDefaultCharacterSubmenuStat(string key, RadialCheckRemove remove) => _hideStat.Add( key, (remove.TitleToRemove, remove.ShouldRemoveCallback));

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
    internal class StatSubMenuPatch
    {
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var statNames = CampaignSessionManager.StatNames;
            for (var i = 0; i < statNames.Length; i++)
            {
                if (RadialUIPlugin._hideStat.CanAdd(i.ToString(), miniId.ToString(), targetId.ToString())) map.AddStat(statNames[i], ____selectedCreature.CreatureId, i);
            }

            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onStatCallback, targetId);
        }
    }
}