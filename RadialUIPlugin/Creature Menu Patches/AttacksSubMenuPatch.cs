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
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuAttacks = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuAttacks = new Dictionary<string, List<RadialCheckRemove>>();

        // Same Methods, different Signatures
        public static void AddCustomButtonAttacksSubmenu(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuAttacks.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonAttacksSubmenu(string key) => _onSubmenuAttacks.Remove(key);
        public static void HideDefaultAttacksSubmenuItem(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuAttacks, key, value, callback);
        public static void UnHideDefaultAttacksSubmenuItem(string key, string value) => RemoveRemoveOn(_removeOnSubmenuAttacks, key, value);

        [Obsolete("This method signature will be replaced with AddCustomButtonAttacksSubmenu on Version 2.1.0.0")]
        public static void AddOnSubmenuAttacks(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuAttacks.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be replaced with RemoveCustomButtonAttacksSubmenu on Version 2.1.0.0")]
        public static bool RemoveOnSubmenuAttacks(string key) => _onSubmenuAttacks.Remove(key);
        [Obsolete("This method signature will be replaced with HideDefaultAttacksSubmenuItem on Version 2.1.0.0")]
        public static void AddOnRemoveSubmenuAttacks(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuAttacks, key, value, callback);
        [Obsolete("This method signature will be replaced with UnHideDefaultAttacksSubmenuItem on Version 2.1.0.0")]
        public static void RemoveOnRemoveSubmenuAttacks(string key, string value) => RemoveRemoveOn(_removeOnSubmenuAttacks, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Attack_Menu")]
    internal class AttacksSubMenuPatch
    {
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature, CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var AttackPlayer = Reflections.GetMenuItemAction("AttackPlayer", __instance);
            var MagicMissile = Reflections.GetMenuItemAction("MagicMissile", __instance);

            if (RadialUIPlugin._removeOnSubmenuAttacks.CanAdd("Attack", miniId.ToString(), targetId.ToString()))
                map.AddItem(AttackPlayer, "Attack", icon: Icons.GetIconSprite("Attacks"), obj: ____selectedCreature, closeMenuOnActivate: true);
            if (RadialUIPlugin._removeOnSubmenuAttacks.CanAdd("Magic Missile", miniId.ToString(), targetId.ToString()))
                map.AddItem(MagicMissile, "Magic Missile", icon: Icons.GetIconSprite("MagicMissile"), obj: ____selectedCreature, closeMenuOnActivate: true);

            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onSubmenuAttacks, targetId);
        }
    }
}