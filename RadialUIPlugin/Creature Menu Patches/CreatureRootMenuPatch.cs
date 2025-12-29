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
        public static void Postfix(CreatureBoardAsset ____selectedCreature, CreatureMenuBoardTool __instance,
            float ____hitHeightDif)
        {
            NGuid miniId = ____selectedCreature.CreatureId.Value;
            RadialUIPlugin.lastCreature = miniId;

            map = Mapmenu.GetMapMenu();
            map.TryHideItem("Attack");

            map.AddItems(RadialUIPlugin._onCharacterCallback, ____selectedCreature.CreatureId.Value);
        }
    }
}
