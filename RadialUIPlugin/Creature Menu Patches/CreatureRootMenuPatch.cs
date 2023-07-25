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
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCharacterCallback =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCanAttack =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCantAttack =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        // Character Related Remove
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnCharacter =
            new Dictionary<string, List<RadialCheckRemove>>();

        // Add On Character
        public static void AddCustomButtonOnCharacter(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null) => _onCharacterCallback.Add(key, (value, externalCheck));

        public static bool RemoveCustomButtonOnCharacter(string key) => _onCharacterCallback.Remove(key);

        public static void HideDefaultCharacterMenuItem(string key, string value, ShouldShowMenu callback = null) =>
            AddRemoveOn(_removeOnCharacter, key, value, callback);

        public static void UnHideDefaultCharacterMenuItem(string key, string value) =>
            RemoveRemoveOn(_removeOnCharacter, key, value);

        internal static NGuid lastCreature;

        /// <summary>
        /// Returns the ID of last creature targeted.
        /// </summary>
        /// <returns>NGUID of the last creature</returns>
        public static NGuid GetLastRadialTargetCreature()
        {
            return lastCreature;
        }
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Begin")]
    internal sealed class CreatureRootMenuPatch
    {
        internal static MapMenu map;

        // ReSharper disable InconsistentNaming
        internal static void Postfix(CreatureBoardAsset ____selectedCreature, CreatureMenuBoardTool __instance,
            float ____hitHeightDif) //, List<UIWorldIconItem> ____creatureVisabilityIcons)
        {
            var miniId = ____selectedCreature.CreatureId.Value;
            RadialUIPlugin.lastCreature = miniId;

            miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            map = MapMenuManager.OpenMenu(____selectedCreature.transform.position + Vector3.up * ____hitHeightDif,
                true);

            var attackMenu = Reflections.GetMenuAction("Attack_Menu", __instance);

            if (LocalClient.SelectedCreatureId != new CreatureGuid() &&
                LocalClient.SelectedCreatureId != ____selectedCreature.CreatureId && attackMenu != null &&
                RadialUIPlugin._removeOnCharacter.CanAdd("Attacks", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(
                    Reflections.CallMethod<MapMenu.MenuType, CreatureMenuBoardTool>("AttackMenuStyle", __instance),
                    attackMenu, "Attacks", icon: Icons.GetIconSprite("Attacks"));

            if (CreatureManager.PlayerCanControlCreature(LocalPlayer.Id, ____selectedCreature.CreatureId))
            {

            var emoteMenu = Reflections.GetMenuAction("Emote_Menu", __instance);
            var statusEmoteMenu = Reflections.GetMenuAction("StatusEmote_Menu", __instance);

            var torchToggle = Reflections.GetMenuItemAction("Menu_ToggleTorch", __instance);

            if (emoteMenu != null &&
                RadialUIPlugin._removeOnCharacter.CanAdd("Emotes", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(
                    Reflections.CallMethod<MapMenu.MenuType, CreatureMenuBoardTool>("EmoteMenuStyle", __instance),
                    emoteMenu, "Emotes", icon: Icons.GetIconSprite("emote"));
            if (statusEmoteMenu != null &&
                RadialUIPlugin._removeOnCharacter.CanAdd("Status", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(
                    Reflections.CallMethod<MapMenu.MenuType, CreatureMenuBoardTool>("StatusEmoteMenuStyle", __instance),
                    statusEmoteMenu, "Status", icon: Icons.GetIconSprite("status_emote"));
            if (torchToggle != null &&
                RadialUIPlugin._removeOnCharacter.CanAdd("Torch", miniId.ToString(), targetId.ToString()))
                map.AddToggleItem(____selectedCreature.TorchEnabled, torchToggle, "Disable Torch", "Enable Torch",
                    icon: Icons.GetIconSprite("torch"));

            if (____selectedCreature.Link != null)
            {
                var linkMenu = Reflections.GetMenuAction("LinkMenu", __instance);

                if (linkMenu != null &&
                    RadialUIPlugin._removeOnCharacter.CanAdd("Link", miniId.ToString(), targetId.ToString()))
                    map.AddMenuItem(MapMenu.MenuType.SUBROOT, linkMenu, "Link", icon: Icons.GetIconSprite("link"));
            }

            if (LocalClient.IsInGmMode)
            {
                var hideCreature = Reflections.GetMenuItemAction("HideCreature", __instance);
                var menuGmTools = Reflections.GetMenuAction("Menu_GMTools", __instance);
                var menuKillMenu = Reflections.GetMenuAction("Menu_KillMenu", __instance);
                var toggleFlying = Reflections.GetMenuItemAction("ToggleFlying", __instance);

                if (hideCreature != null &&
                    RadialUIPlugin._removeOnCharacter.CanAdd("Hide", miniId.ToString(), targetId.ToString()))
                    map.AddToggleItem(____selectedCreature.IsExplicitlyHidden, hideCreature, "Reveal", "Hide",
                        icon: Icons.GetIconSprite("creaturehide"));
                if (menuGmTools != null &&
                    RadialUIPlugin._removeOnCharacter.CanAdd("GM Tools", miniId.ToString(), targetId.ToString()))
                    map.AddMenuItem(MapMenu.MenuType.SUBROOT, menuGmTools, "GM Tools",
                        icon: Icons.GetIconSprite("dungeonmaster"));
                if (menuKillMenu != null &&
                    RadialUIPlugin._removeOnCharacter.CanAdd("KillMenu", miniId.ToString(), targetId.ToString()))
                    map.AddMenuItem(MapMenu.MenuType.BRANCH, menuKillMenu, "KillMenu",
                        icon: Icons.GetIconSprite("remove"));
                if (toggleFlying != null &&
                    RadialUIPlugin._removeOnCharacter.CanAdd("Fly Toggle", miniId.ToString(), targetId.ToString()))
                    map.AddToggleItem(____selectedCreature.Flying, toggleFlying, "Disable Flying", " Enable Flying",
                        icon: Icons.GetIconSprite("fly"));
            }

            var morphsMenu = Reflections.GetMenuAction("Morphs_Menu", __instance);
            if (morphsMenu != null &&
                RadialUIPlugin._removeOnCharacter.CanAdd("Morphs", miniId.ToString(), targetId.ToString()))
                map.AddMenuItem(MapMenu.MenuType.BRANCH, morphsMenu, "Morphs", icon: Icons.GetIconSprite("character"));

            Reflections.CallMethod("AddStats", __instance, new object[] { map });
            }
            map.AddItems(RadialUIPlugin._onCharacterCallback, ____selectedCreature.CreatureId.Value);
        }

        /*internal static void Postfix(CreatureBoardAsset ____selectedCreature)
        {
            //var targetId = ____selectedCreature.CreatureId.Value;
            //map.AddItems(RadialUIPlugin._onCharacterCallback, targetId);
        }*/
    }
}
