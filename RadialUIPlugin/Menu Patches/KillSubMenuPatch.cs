using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Menu_Patches;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuKill = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuKill = new Dictionary<string, List<RadialCheckRemove>>();

        public static void AddCustomButtonOnSubmenuKill(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuKill.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonOnSubmenuKill(string key) => _onSubmenuKill.Remove(key);
        public static void HideDefaultSubmenuKillItem(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuKill, key, value, callback);
        public static void ShowDefaultSubmenuKillItem(string key, string value) => RemoveRemoveOn(_removeOnSubmenuKill, key, value);

        [Obsolete("This method signature will be replaced with AddCustomButtonOnSubmenuKill on Version 2.1.0.0")]
        public static void AddOnSubmenuKill(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuKill.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be replaced with RemoveCustomButtonOnSubmenuKill on Version 2.1.0.0")]
        public static bool RemoveOnSubmenuKill(string key) => _onSubmenuKill.Remove(key);
        [Obsolete("This method signature will be replaced with HideDefaultSubmenuKillItem on Version 2.1.0.0")]
        public static void AddOnRemoveSubmenuKill(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuKill, key, value, callback);
        [Obsolete("This method signature will be replaced with ShowDefaultSubmenuKillItem on Version 2.1.0.0")]
        public static void RemoveOnRemoveSubmenuKill(string key, string value) => RemoveRemoveOn(_removeOnSubmenuKill, key, value);
    }
}

namespace RadialUI.Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_KillMenu")]
    internal class KillSubMenuPatch
    {
        private static void Action_Kill(MapMenuItem item, object obj) => ((Creature)obj).BoardAsset.RequestDelete();

        internal static bool Prefix(MapMenu map, object obj, Creature ____selectedCreature)
        {
            var miniId = NGuid.Empty;
            var targetId = ____selectedCreature.CreatureId.Value;

            if (RadialUIPlugin._removeOnSubmenuKill.CanShow("Kill Creature",miniId.ToString(),targetId.ToString())) map.AddItem(Action_Kill, "Kill Creature", icon: Icons.GetIconSprite("remove"), closeMenuOnActivate: true,obj:____selectedCreature);
            return true;
        }

        internal static void Postfix(MapMenu map, object obj, Creature ____selectedCreature)
        {
            var miniId = NGuid.Empty;
            var targetId = ____selectedCreature.CreatureId.Value;

            foreach (var key in RadialUIPlugin._onSubmenuKill.Keys.Where(key => RadialUIPlugin._onSubmenuKill[key].Item2 == null || RadialUIPlugin._onSubmenuKill[key].Item2(miniId, targetId)))
            {
                map.AddItem(RadialUIPlugin._onSubmenuKill[key].Item1);
            }
        }
    }
}
