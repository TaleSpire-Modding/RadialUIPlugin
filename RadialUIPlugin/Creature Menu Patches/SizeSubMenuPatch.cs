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
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuSize = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuSize = new Dictionary<string, List<RadialCheckRemove>>();

        public static void AddCustomButtonOnSizeSubmenu(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuSize.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonOnSizeSubmenu(string key) => _onSubmenuSize.Remove(key);
        public static void HideDefaultSizeSubmenuItem(string key, string value, ShouldShowMenu callback = null) => RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnSubmenuSize, key, value, callback);
        public static void ShowDefaultSizeSubmenuItem(string key, string value) => RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnSubmenuSize, key, value);

        [Obsolete("This method signature will be replaced with AddCustomButtonOnSizeSubmenu on Version 2.1.0.0")]
        public static void AddOnSubmenuSize(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuSize.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be replaced with RemoveCustomButtonOnSizeSubmenu on Version 2.1.0.0")]
        public static bool RemoveOnSubmenuSize(string key) => _onSubmenuSize.Remove(key);
        [Obsolete("This method signature will be replaced with HideDefaultSizeSubmenuItem on Version 2.1.0.0")]
        public static void AddOnRemoveSubmenuSize(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuSize, key, value, callback);
        [Obsolete("This method signature will be replaced with ShowDefaultSizeSubmenuItem on Version 2.1.0.0")]
        public static void RemoveOnRemoveSubmenuSize(string key, string value) => RemoveRemoveOn(_removeOnSubmenuSize, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_SetSize")]
    internal class SizeSubMenuPatch
    {
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature, CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;
            
            var menuScale = Reflections.GetMenuItemAction("Menu_Scale", __instance);
            
            if (menuScale == null) return false;
            
            if (RadialUIPlugin._removeOnSubmenuSize.CanAdd("0.5x0.5", miniId.ToString(),targetId.ToString()))
                map.AddItem(menuScale, "0.5x0.5", icon: Icons.GetIconSprite("05x05"), obj: ((object)0.5f), closeMenuOnActivate: true);
            if (RadialUIPlugin._removeOnSubmenuSize.CanAdd("1x1", miniId.ToString(), targetId.ToString()))
                map.AddItem(menuScale, "1x1", icon: Icons.GetIconSprite("1x1"), obj: ((object)1f), closeMenuOnActivate: true);
            if (RadialUIPlugin._removeOnSubmenuSize.CanAdd("2x2", miniId.ToString(), targetId.ToString()))
                map.AddItem(menuScale, "2x2", icon: Icons.GetIconSprite("2x2"), obj: ((object)2f), closeMenuOnActivate: true);
            if (RadialUIPlugin._removeOnSubmenuSize.CanAdd("3x3", miniId.ToString(), targetId.ToString()))
                map.AddItem(menuScale, "3x3", icon: Icons.GetIconSprite("3x3"), obj: ((object)3f), closeMenuOnActivate: true);
            if (RadialUIPlugin._removeOnSubmenuSize.CanAdd("4x4", miniId.ToString(), targetId.ToString()))
                map.AddItem(menuScale, "4x4", icon: Icons.GetIconSprite("4x4"), obj: ((object)4f), closeMenuOnActivate: true);
            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onSubmenuSize,targetId);
        }
    }
}