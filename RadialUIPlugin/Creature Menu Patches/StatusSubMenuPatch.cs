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
        internal static bool Prefix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature,
            List<ActionTimeline> ____statusEmotes, CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var CallStatusEmote = Reflections.GetMenuItemAction("CallStatusEmote", __instance);

            if (CallStatusEmote == null) return false;

            for (int index = 0; index < ____statusEmotes.Count; ++index)
            {
                ActionTimeline statusEmote = ____statusEmotes[index];
                if (RadialUIPlugin._removeOnSubmenuStatusEmotes.CanAdd(statusEmote.name, miniId.ToString(),
                        targetId.ToString()))
                {
                    bool toggleValue =
                        ____selectedCreature.IsPersistantEmoteEnabled(____statusEmotes[index].ActionTimelineId);
                    map.AddToggleItem(toggleValue, CallStatusEmote, statusEmote.DisplayName,
                        icon: Icons.GetIconSprite(statusEmote.IconName), obj: ((object)____statusEmotes[index]),
                        fadeName: false);
                }
            }

            return false;
        }

        internal static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            var targetId = ____selectedCreature.CreatureId.Value;
            map.AddItems(RadialUIPlugin._onSubmenuStatusEmotes, targetId);
        }
    }
}