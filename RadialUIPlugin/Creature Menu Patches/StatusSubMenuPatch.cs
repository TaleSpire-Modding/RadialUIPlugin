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
        // Emotes Submenu Extra Components
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>
            _onSubmenuStatusEmotes = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuStatusEmotes =
            new Dictionary<string, List<RadialCheckRemove>>();

        // Same Methods, different Signatures
        public static void AddCustomButtonStatusEmotesSubmenu(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuStatusEmotes.Add(key, (value, externalCheck));

        public static bool RemoveCustomButtonStatusEmotesSubmenu(string key) => _onSubmenuStatusEmotes.Remove(key);

        public static void
            HideDefaultEmotesStatusSubmenuItem(string key, string value, ShouldShowMenu callback = null) =>
            AddRemoveOn(_removeOnSubmenuStatusEmotes, key, value, callback);

        public static void UnHideDefaultEmotesStatusSubmenuItem(string key, string value) =>
            RemoveRemoveOn(_removeOnSubmenuStatusEmotes, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "StatusEmote_Menu")]
    internal sealed class StatusSubMenuPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(MapMenu map, object obj, List<ActionTimeline> ____statusEmotes, CreatureBoardAsset ____selectedCreature)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            for (int index = 0; index < ____statusEmotes.Count; ++index)
            {
                ActionTimeline statusEmote = ____statusEmotes[index];
                map.TryHideItem(RadialUIPlugin._removeOnSubmenuStatusEmotes, statusEmote.name, miniId.ToString(), targetId.ToString());
            }

            map.AddItems(RadialUIPlugin._onSubmenuStatusEmotes, targetId);
        }
    }
}