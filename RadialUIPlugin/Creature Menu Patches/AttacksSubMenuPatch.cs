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
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature,
            CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var attackPlayer = Reflections.GetMenuItemAction("AttackPlayer", __instance);
            var magicMissile = Reflections.GetMenuItemAction("MagicMissile", __instance);
            var redLaser = Reflections.GetMenuItemAction("RedLaser", __instance);
            var greenLaser = Reflections.GetMenuItemAction("GreenLaser", __instance);

            if (attackPlayer != null &&
                RadialUIPlugin._removeOnSubmenuAttacks.CanAdd("Attack", miniId.ToString(), targetId.ToString()))
                map.AddItem(attackPlayer, "Attack", icon: Icons.GetIconSprite("Attacks"), obj: ____selectedCreature,
                    closeMenuOnActivate: true);
            if (magicMissile != null &&
                RadialUIPlugin._removeOnSubmenuAttacks.CanAdd("Magic Missile", miniId.ToString(), targetId.ToString()))
                map.AddItem(magicMissile, "Magic Missile", icon: Icons.GetIconSprite("MagicMissile"),
                    obj: ____selectedCreature, closeMenuOnActivate: true);
            if (redLaser != null &&
                RadialUIPlugin._removeOnSubmenuAttacks.CanAdd("RedLaser", miniId.ToString(), targetId.ToString()))
                map.AddItem(redLaser, "Red Laser", icon: Icons.GetIconSprite("RedLaser"), obj: ____selectedCreature,
                    closeMenuOnActivate: true);
            if (greenLaser != null &&
                RadialUIPlugin._removeOnSubmenuAttacks.CanAdd("GreenLaser", miniId.ToString(), targetId.ToString()))
                map.AddItem(greenLaser, "Green Laser", icon: Icons.GetIconSprite("GreenLaser"),
                    obj: ____selectedCreature, closeMenuOnActivate: true);

            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onSubmenuAttacks, targetId);
        }
    }
}