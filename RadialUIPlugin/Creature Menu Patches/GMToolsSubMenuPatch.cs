using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Reflection_Extensions;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuGm = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuGm = new Dictionary<string, List<RadialCheckRemove>>();

        // Same Methods, different Signatures
        public static void AddCustomButtonGMSubmenu(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuGm.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonGMSubmenu(string key) => _onSubmenuGm.Remove(key);
        public static void HideDefaultEmotesGMItem(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuGm, key, value, callback);
        public static void UnHideDefaultGMSubmenuItem(string key, string value) => RemoveRemoveOn(_removeOnSubmenuGm, key, value);

        // Old obsolete Methods
        [Obsolete("This method signature will be replaced with AddCustomButtonGMSubmenu on Version 2.1.0.0")]
        public static void AddOnSubmenuGm(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuGm.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be replaced with RemoveCustomButtonGMSubmenu on Version 2.1.0.0")]
        public static bool RemoveOnSubmenuGm(string key) => _onSubmenuGm.Remove(key);
        [Obsolete("This method signature will be replaced with HideDefaultEmotesGMItem on Version 2.1.0.0")]
        public static void AddOnRemoveSubmenuGm(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuGm, key, value, callback);
        [Obsolete("This method signature will be replaced with UnHideDefaultGMSubmenuItem on Version 2.1.0.0")]
        public static void RemoveOnRemoveSubmenuGm(string key, string value) => RemoveRemoveOn(_removeOnSubmenuGm, key, value);

    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_GMTools")]
    internal class GMToolsSubMenuPatch
    {
        internal static bool Prefix(MapMenu map, object obj, Creature ____selectedCreature, CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;
            
            if (RadialUIPlugin._removeOnSubmenuGm.CanAdd("", miniId.ToString(),
                targetId.ToString()))
            {
                var Menu_SetCreaturePlayerPermission = Reflections.GetMenuAction("Menu_SetCreaturePlayerPermission", __instance);
                var PermissionMenuStyle = CampaignSessionManager.PlayersInfo.Count >= 20 ? MapMenu.MenuType.SUBROOT : MapMenu.MenuType.BRANCH;
                map.AddMenuItem(PermissionMenuStyle, Menu_SetCreaturePlayerPermission, "Player Permission", icon: Icons.GetIconSprite("permission"));
            }

            if (RadialUIPlugin._removeOnSubmenuGm.CanAdd("Rename", miniId.ToString(),
                targetId.ToString()))
            {
                var Menu_RenameCreature = Reflections.GetMenuItemAction("Menu_RenameCreature", __instance);
                map.AddItem(Menu_RenameCreature, "Rename", icon: Icons.GetIconSprite("rename"));
            }

            if (____selectedCreature.IsUnique && RadialUIPlugin._removeOnSubmenuGm.CanAdd("Make Not Unique", miniId.ToString(),
                targetId.ToString()))
            {
                var Menu_MakeNotUnique  = Reflections.GetMenuItemAction("Menu_MakeNotUnique", __instance);
                map.AddItem(Menu_MakeNotUnique, "Make Not Unique", icon: Icons.GetIconSprite("dungeonmaster"), closeMenuOnActivate: true);
            }
            else if (RadialUIPlugin._removeOnSubmenuGm.CanAdd("Make Unique", miniId.ToString(),
                targetId.ToString()))
            {
                var Menu_MakeUnique  = Reflections.GetMenuItemAction("Menu_MakeUnique", __instance);
                map.AddItem(Menu_MakeUnique, "Make Unique", icon: Icons.GetIconSprite("dungeonmaster"), closeMenuOnActivate: true);
            }

            if (RadialUIPlugin._removeOnSubmenuGm.CanAdd("Set Size", miniId.ToString(),
                targetId.ToString()))
            {
                var Menu_SetSize = Reflections.GetMenuAction("Menu_SetSize", __instance);
                map.AddMenuItem(MapMenu.MenuType.BRANCH, Menu_SetSize, "Set Size", icon: Icons.GetIconSprite("creaturesize"));
            }
            if (RadialUIPlugin._removeOnSubmenuGm.CanAdd("BaseColor", miniId.ToString(),
                targetId.ToString()))
            {
                var BaseColor_Menu = Reflections.GetMenuAction("BaseColor_Menu", __instance);
                map.AddMenuItem(MapMenu.MenuType.BRANCH, BaseColor_Menu, "BaseColor",
                    icon: Icons.GetIconSprite("basecolor"));
            }

            return false;
        }

        internal static void Postfix(MapMenu map, object obj, Creature ____selectedCreature)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            foreach (var key in RadialUIPlugin._onSubmenuGm.Keys.Where(key => RadialUIPlugin._onSubmenuGm[key].Item2 == null || RadialUIPlugin._onSubmenuGm[key].Item2(miniId, targetId)))
            {
                map.AddItem(RadialUIPlugin._onSubmenuGm[key].Item1);
            }
        }
    }
}