using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Reflection_Extensions;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        // Character Related Add
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCharacterCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCanAttack = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCantAttack = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        // Character Related Remove
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnCharacter = new Dictionary<string, List<RadialCheckRemove>>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnCanAttack = new Dictionary<string, List<RadialCheckRemove>>();
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnCantAttack = new Dictionary<string, List<RadialCheckRemove>>();

        // Add On Character
        public static void AddCustomButtonOnCharacter(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCharacterCallback.Add(key, (value, externalCheck));
        public static bool RemoveCustomButtonOnCharacter(string key) => _onCharacterCallback.Remove(key);
        public static void HideDefaultCharacterMenuItem(string key, string value, ShouldShowMenu callback = null) => RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnCharacter, key, value, callback);
        public static void ShowDefaultCharacterMenuItem(string key, string value) => RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnCharacter, key, value);


        [Obsolete("This method signature will be replaced with AddCustomButtonOnCharacter on Version 2.1.0.0")]
        public static void AddOnCharacter(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCharacterCallback.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be replaced with RemoveCustomButtonOnCharacter on Version 2.1.0.0")]
        public static bool RemoveOnCharacter(string key) => _onCharacterCallback.Remove(key);
        [Obsolete("This method signature will be replaced with HideDefaultCharacterMenuItem on Version 2.1.0.0")]
        public static void AddOnRemoveCharacter(string key, string value, ShouldShowMenu callback = null) => RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnCharacter, key, value, callback);
        [Obsolete("This method signature will be replaced with ShowDefaultCharacterMenuItem on Version 2.1.0.0")]
        public static void RemoveOnRemoveCharacter(string key, string value) => RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnCharacter, key, value);

        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static void AddOnCanAttack(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCanAttack.Add(key, (value, externalCheck));
        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static void AddOnCantAttack(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCantAttack.Add(key, (value, externalCheck));

        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static bool RemoveOnCanAttack(string key) => _onCanAttack.Remove(key);
        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static bool RemoveOnCantAttack(string key) => _onCantAttack.Remove(key);

        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static void AddOnRemoveCanAttack(string key, string value, ShouldShowMenu callback = null) => RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnCanAttack, key, value, callback);
        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static void AddOnRemoveCantAttack(string key, string value, ShouldShowMenu callback = null) => RadialUI.RadialUIPlugin.AddRemoveOn(_removeOnCantAttack, key, value, callback);

        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static void RemoveOnRemoveCanAttack(string key, string value) => RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnCanAttack, key, value);
        [Obsolete("This method signature will be removed on Version 2.1.0.0")]
        public static void RemoveOnRemoveCantAttack(string key, string value) => RadialUI.RadialUIPlugin.RemoveRemoveOn(_removeOnCantAttack, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Begin")]
    internal class CreatureRootMenuPatch
    {
        internal static void Postfix(Creature ____selectedCreature, CreatureMenuBoardTool __instance, 
            ref List<UIWorldIconItem>  ____creatureVisabilityIcons, float ____hitHeightDif, MapMenu.MenuType ___AttackMenuStyle)
        {
            var miniId = NGuid.Empty;
            var targetId = ____selectedCreature.CreatureId.Value;

            var map = MapMenuManager.OpenMenu(____selectedCreature.transform.position + Vector3.up * ____hitHeightDif, true);

            if (LocalClient.SelectedCreatureId != new CreatureGuid() &&
                LocalClient.SelectedCreatureId != ____selectedCreature.CreatureId)
            {
                var Attack_Menu = Reflections.GetMenuAction("Attack_Menu", __instance);
                var AttackMenuStyle = Reflections.CallMethod<MapMenu.MenuType,CreatureMenuBoardTool>("AttackMenuStyle", __instance);
                map.AddMenuItem(AttackMenuStyle,
                    Attack_Menu, "Attacks",
                    icon: Icons.GetIconSprite("Attacks"));
            }

            if (!CreatureManager.PlayerCanControlCreature(LocalPlayer.Id, this._selectedCreature.CreatureId))
                return;
            map.AddMenuItem(new Func<MapMenu.MenuType>(this.EmoteMenuStyle), new System.Action<MapMenu, object>(this.Emote_Menu), "Emotes", icon: Icons.GetIconSprite("emote"));
            map.AddMenuItem(new Func<MapMenu.MenuType>(this.StatusEmoteMenuStyle), new System.Action<MapMenu, object>(this.StatusEmote_Menu), "Status", icon: Icons.GetIconSprite("status_emote"));
            if (____selectedCreature.TorchEnabled)
                map.AddItem(new System.Action<MapMenuItem, object>(this.Menu_DisableTorch), "Disable Torch", icon: Icons.GetIconSprite("torch"), closeMenuOnActivate: true);
            else
                map.AddItem(new System.Action<MapMenuItem, object>(this.Menu_EnableTorch), "Enable Torch", icon: Icons.GetIconSprite("torch"), closeMenuOnActivate: true);
            if (____selectedCreature.Link != null)
                map.AddMenuItem(MapMenu.MenuType.SUBROOT, new System.Action<MapMenu, object>(this.LinkMenu), "Link", icon: Icons.GetIconSprite("link"));
            if (LocalClient.IsInGmMode)
            {
                map.AddItem(new System.Action<MapMenuItem, object>(this.HideCreature), ____selectedCreature.IsExplicitlyHidden ? "Reveal" : "Hide", icon: Icons.GetIconSprite("creaturehide"), closeMenuOnActivate: true);
                map.AddMenuItem(MapMenu.MenuType.SUBROOT, new System.Action<MapMenu, object>(this.Menu_GMTools), "GM Tools", icon: Icons.GetIconSprite("dungeonmaster"));
                map.AddMenuItem(MapMenu.MenuType.BRANCH, new System.Action<MapMenu, object>(this.Menu_KillMenu), "KillMenu", icon: Icons.GetIconSprite("remove"));
                map.AddItem(new System.Action<MapMenuItem, object>(this.EnableFlying), "Fly Toggle", icon: Icons.GetIconSprite("fly"), closeMenuOnActivate: true);
            }


            foreach (var key in RadialUIPlugin._onCharacterCallback.Keys.Where(key => RadialUIPlugin._onCharacterCallback[key].Item2 == null || RadialUIPlugin._onCharacterCallback[key].Item2(miniId, targetId)))
            {
                map.AddItem(RadialUIPlugin._onCharacterCallback[key].Item1);
            }

            // Obsolete below
            foreach (var key in RadialUIPlugin._onCanAttack.Keys.Where(key => RadialUIPlugin._onCanAttack[key].Item2 == null || RadialUIPlugin._onCanAttack[key].Item2(miniId, targetId)))
            {
                map.AddItem(RadialUIPlugin._onCanAttack[key].Item1);
            }
            foreach (var key in RadialUIPlugin._onCantAttack.Keys.Where(key => RadialUIPlugin._onCantAttack[key].Item2 == null || RadialUIPlugin._onCantAttack[key].Item2(miniId, targetId)))
            {
                map.AddItem(RadialUIPlugin._onCantAttack[key].Item1);
            }
        }
    }
}