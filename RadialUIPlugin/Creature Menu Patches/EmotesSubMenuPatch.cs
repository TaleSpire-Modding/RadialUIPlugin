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
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuEmotes =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuEmotes =
            new Dictionary<string, List<RadialCheckRemove>>();

        // Same Methods, different Signatures
        public static void AddCustomButtonEmotesSubmenu(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuEmotes.Add(key, (value, externalCheck));

        public static bool RemoveCustomButtonEmotesSubmenu(string key) => _onSubmenuEmotes.Remove(key);

        public static void HideDefaultEmotesSubmenuItem(string key, string value, ShouldShowMenu callback = null) =>
            AddRemoveOn(_removeOnSubmenuEmotes, key, value, callback);

        public static void UnHideDefaultEmotesSubmenuItem(string key, string value) =>
            RemoveRemoveOn(_removeOnSubmenuEmotes, key, value);
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Emote_Menu")]
    internal sealed class EmotesSubMenuPatch
    {
        public static void Postfix(MapMenu map, object obj, List<ActionTimeline> ____emotes, CreatureBoardAsset ____selectedCreature)
        {
            NGuid miniId = LocalClient.SelectedCreatureId.Value;
            NGuid targetId = ____selectedCreature.CreatureId.Value;

            for (int index = 0; index < ____emotes.Count; ++index)
            {
                ActionTimeline emote = ____emotes[index];

                map.TryHideItem(RadialUIPlugin._removeOnSubmenuEmotes, emote.name, miniId.ToString(), targetId.ToString());
            }

            map.AddItems(RadialUIPlugin._onSubmenuEmotes, targetId);
        }
    }
}