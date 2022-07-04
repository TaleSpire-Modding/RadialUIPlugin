using HarmonyLib;
using RadialUI.Extensions;

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "AddStats")]
    internal class AddStatsMenuPatch
    {
        // ReSharper disable InconsistentNaming
        internal static bool Prefix(ref MapMenu mapMenu, CreatureBoardAsset ____selectedCreature, CreatureMenuBoardTool __instance)
        {
            var miniId = LocalClient.SelectedCreatureId.Value;
            var targetId = ____selectedCreature.CreatureId.Value;

            var menuStats = Reflections.GetMenuAction("Menu_Stats", __instance);

            if (RadialUIPlugin._removeOnCharacter.CanAdd("HP", miniId.ToString(), targetId.ToString()))
                mapMenu.AddStat("HP", ____selectedCreature.CreatureId, -1);
            if (menuStats != null && RadialUIPlugin._removeOnCharacter.CanAdd("Stats", miniId.ToString(), targetId.ToString()))
                mapMenu.AddMenuItem(MapMenu.MenuType.SUBROOT, menuStats, "Stats", icon: Icons.GetIconSprite("stats"));
            return false;
        }
    }
}
