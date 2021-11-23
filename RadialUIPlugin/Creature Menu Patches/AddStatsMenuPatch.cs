﻿using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Reflection_Extensions;

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "AddStats")]
    internal class AddStatsMenuPatch
    {
        internal static bool Prefix(ref MapMenu mapMenu, Creature ____selectedCreature, CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            if (RadialUIPlugin._removeOnCharacter.CanAdd("HP", miniId.ToString(), targetId.ToString()))
                mapMenu.AddStat("HP", ____selectedCreature.CreatureId, -1);
            if (RadialUIPlugin._removeOnCharacter.CanAdd("Stats", miniId.ToString(), targetId.ToString()))
                mapMenu.AddMenuItem(MapMenu.MenuType.SUBROOT, Reflections.GetMenuAction("Menu_Stats", __instance), "Stats", icon: Icons.GetIconSprite("stats"));
            return false;
        }
    }
}