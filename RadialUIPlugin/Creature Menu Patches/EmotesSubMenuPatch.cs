using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {

    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Emote_Menu")]
    internal class EmotesSubMenuPatch
    {
        internal static bool Prefix(MapMenu map, object obj, Creature ____selectedCreature)
        {
            var miniId = NGuid.Empty;
            var targetId = ____selectedCreature.CreatureId.Value;

            return true;
        }

        internal static void Postfix(MapMenu map, object obj, Creature ____selectedCreature)
        {
            var miniId = NGuid.Empty;
            var targetId = ____selectedCreature.CreatureId.Value;
        }
    }
}