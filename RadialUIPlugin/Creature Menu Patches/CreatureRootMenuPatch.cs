using System;
using System.Collections.Generic;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Extensions;
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
        public static void HideDefaultCharacterMenuItem(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnCharacter, key, value, callback);
        public static void UnHideDefaultCharacterMenuItem(string key, string value) => RemoveRemoveOn(_removeOnCharacter, key, value);

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

        internal static NGuid lastCreature;

        public static NGuid GetLastRadialTargetCreature()
        {
            return lastCreature;
        }
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Begin")]
    internal class CreatureRootMenuPatch
    {
        /// <summary>
        /// Track the last targeted creature before everything else.
        /// </summary>
        internal static void Prefix(CreatureBoardAsset ____selectedCreature)
        {
            var miniId = ____selectedCreature.CreatureId.Value;
            RadialUIPlugin.lastCreature = miniId;
        }

        /// <summary>
        /// A lot of this code is a re-write but uses reflection to maintain code invocation.
        /// </summary>
        internal static void Postfix(CreatureBoardAsset ____selectedCreature, CreatureMenuBoardTool __instance, float ____hitHeightDif)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var map = MapMenuManager.OpenMenu(____selectedCreature.transform.position + Vector3.up * ____hitHeightDif, true);

            if (LocalClient.SelectedCreatureId != new CreatureGuid() &&
                LocalClient.SelectedCreatureId != ____selectedCreature.CreatureId)
            {
                if (RadialUIPlugin._removeOnCharacter.CanAdd("Attacks", miniId.ToString(), targetId.ToString()))
                {
                    map.AddMenuItem(
                        Reflections.CallMethod<MapMenu.MenuType, CreatureMenuBoardTool>("AttackMenuStyle", __instance),
                        Reflections.GetMenuAction("Attack_Menu", __instance), "Attacks",
                        icon: Icons.GetIconSprite("Attacks"));
                }
            }

            if (!CreatureManager.PlayerCanControlCreature(LocalPlayer.Id, ____selectedCreature.CreatureId))
                return;

            if (RadialUIPlugin._removeOnCharacter.CanAdd("Emotes", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(Reflections.CallMethod<MapMenu.MenuType, CreatureMenuBoardTool>("EmoteMenuStyle", __instance), Reflections.GetMenuAction("Emote_Menu", __instance), "Emotes", icon: Icons.GetIconSprite("emote"));
            if (RadialUIPlugin._removeOnCharacter.CanAdd("Status", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(Reflections.CallMethod<MapMenu.MenuType, CreatureMenuBoardTool>("StatusEmoteMenuStyle", __instance), Reflections.GetMenuAction("StatusEmote_Menu", __instance), "Status", icon: Icons.GetIconSprite("status_emote"));

            if (RadialUIPlugin._removeOnCharacter.CanAdd("Torch", miniId.ToString(), targetId.ToString()))
                map.AddItem(
                    Reflections.GetMenuItemAction(____selectedCreature.TorchEnabled ? "Menu_DisableTorch" : "Menu_EnableTorch", __instance)
                    , ____selectedCreature.TorchEnabled ? "Disable Torch" : "Enable Torch"
                    , icon: Icons.GetIconSprite("torch"), closeMenuOnActivate: true);

            if (____selectedCreature.Link != null)
            {
                if (RadialUIPlugin._removeOnCharacter.CanAdd("Link", miniId.ToString(), targetId.ToString()))
                    map.AddMenuItem(MapMenu.MenuType.SUBROOT, Reflections.GetMenuAction("LinkMenu", __instance), "Link", icon: Icons.GetIconSprite("link"));
            }
            if (LocalClient.IsInGmMode)
            {
                if (RadialUIPlugin._removeOnCharacter.CanAdd("Hide", miniId.ToString(), targetId.ToString()))
                    map.AddItem(Reflections.GetMenuItemAction("HideCreature", __instance), ____selectedCreature.IsExplicitlyHidden ? "Reveal" : "Hide", icon: Icons.GetIconSprite("creaturehide"), closeMenuOnActivate: true);
                if (RadialUIPlugin._removeOnCharacter.CanAdd("GM Tools", miniId.ToString(), targetId.ToString()))
                    map.AddMenuItem(MapMenu.MenuType.SUBROOT, Reflections.GetMenuAction("Menu_GMTools", __instance) , "GM Tools", icon: Icons.GetIconSprite("dungeonmaster"));
                if (RadialUIPlugin._removeOnCharacter.CanAdd("KillMenu", miniId.ToString(), targetId.ToString()))
                    map.AddMenuItem(MapMenu.MenuType.BRANCH, Reflections.GetMenuAction("Menu_KillMenu", __instance) , "KillMenu", icon: Icons.GetIconSprite("remove"));
                if (RadialUIPlugin._removeOnCharacter.CanAdd("Fly Toggle", miniId.ToString(), targetId.ToString()))
                    map.AddItem(Reflections.GetMenuItemAction("EnableFlying", __instance), "Fly Toggle", icon: Icons.GetIconSprite("fly"), closeMenuOnActivate: true);
            }

            Reflections.CallMethod("AddStats", __instance,new object[] {map});

            map.AddItems(RadialUIPlugin._onCharacterCallback, targetId);
        }
    }
}