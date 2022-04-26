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
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuKill = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuKill = new Dictionary<string, List<RadialCheckRemove>>();

        public static void AddCustomButtonOnKillSubmenu(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuKill.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonOnKillSubmenu(string key) => _onSubmenuKill.Remove(key);
        public static void HideDefaultKillSubmenuItem(string key, string value, ShouldShowMenu callback = null) => RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnSubmenuKill, key, value, callback);
        public static void ShowDefaultKillSubmenuItem(string key, string value) => RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnSubmenuKill, key, value);

        [Obsolete("This method signature will be replaced with AddCustomButtonOnKillSubmenu on Version 2.1.0.0")]
        public static void AddOnSubmenuKill(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuKill.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be replaced with RemoveCustomButtonOnKillSubmenu on Version 2.1.0.0")]
        public static bool RemoveOnSubmenuKill(string key) => _onSubmenuKill.Remove(key);
        [Obsolete("This method signature will be replaced with HideDefaultKillSubmenuItem on Version 2.1.0.0")]
        public static void AddOnRemoveSubmenuKill(string key, string value, ShouldShowMenu callback = null) => RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnSubmenuKill, key, value, callback);
        [Obsolete("This method signature will be replaced with ShowDefaultKillSubmenItem on Version 2.1.0.0")]
        public static void RemoveOnRemoveSubmenuKill(string key, string value) => RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnSubmenuKill, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_KillMenu")]
    internal class KillSubMenuPatch
    {
        
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature, CreatureMenuBoardTool __instance)
        {
            
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var Action_Kill = Reflections.GetMenuItemAction("Action_Kill", __instance);

            if (RadialUIPlugin._removeOnSubmenuKill.CanShow("Kill Creature",miniId.ToString(),targetId.ToString())) map.AddItem(Action_Kill, "Kill Creature", icon: Icons.GetIconSprite("remove"), closeMenuOnActivate: true,obj:____selectedCreature);
            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onSubmenuKill, targetId);
        }
    }
}