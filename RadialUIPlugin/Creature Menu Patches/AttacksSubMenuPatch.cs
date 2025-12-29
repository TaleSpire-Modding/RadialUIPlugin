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
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuAttacks =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuAttacks =
            new Dictionary<string, List<RadialCheckRemove>>();

        /// <summary>
        /// Registers a custom map menu item in the Attack submenu
        /// </summary>
        /// <param name="key">Unique identifier for the custom map menu item</param>
        /// <param name="value">Properties of the map menu item</param>
        /// <param name="externalCheck"></param>
        public static void AddCustomButtonAttacksSubmenu(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuAttacks.Add(key, (value, externalCheck));

        /// <summary>
        /// Removes a custom map menu item in the Attack submenu
        /// </summary>
        /// <param name="key">Unique identifier for the custom map menu item</param>
        /// <returns>Pass/Fail of custom menu being removed</returns>
        public static bool RemoveCustomButtonAttacksSubmenu(string key) => _onSubmenuAttacks.Remove(key);

        /// <summary>
        /// Hides existing core attack in the Attack submenu
        /// </summary>
        /// <param name="key">Unique identifier for the core map menu item to be hidden</param>
        /// <param name="value">The actual item to hide</param>
        /// <param name="callback">Nullable Custom callback to determine if the entry should be hidden</param>
        public static void HideDefaultAttacksSubmenuItem(string key, string value, ShouldShowMenu callback = null) =>
            AddRemoveOn(_removeOnSubmenuAttacks, key, value, callback);

        /// <summary>
        /// UnHides existing core attack in the Attack submenu
        /// </summary>
        /// <param name="key">Unique identifier for the core map menu item to be hidden</param>
        /// <param name="value">The actual item to hide</param>
        public static void UnHideDefaultAttacksSubmenuItem(string key, string value) =>
            RemoveRemoveOn(_removeOnSubmenuAttacks, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Attack_Menu")]
    internal sealed class AttacksSubMenuPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            NGuid miniId = LocalClient.SelectedCreatureId.Value;
            NGuid targetId = ____selectedCreature.CreatureId.Value;

            // Hide following items if conditions met and able
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuAttacks, "Hit", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuAttacks, "Magic Missile", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuAttacks, "RedLaser", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuAttacks, "GreenLaser", miniId.ToString(), targetId.ToString());

            map.AddItems(RadialUIPlugin._onSubmenuAttacks, targetId);
        }
    }
}