using System;
using System.Collections.Generic;
using BepInEx;
using DataModel;
using HarmonyLib;
using RadialUI.Extensions;
using TaleSpire.GameMaster.Blocks;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnPlaceable = new Dictionary<string, List<RadialCheckRemove>>();
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<AtmosphereBlock, bool>)> _onPlaceable = new Dictionary<string, (MapMenu.ItemArgs, Func<AtmosphereBlock, bool>)>();
        
    }
}

namespace RadialUI.Placeables_Menu_Patches
{
    [HarmonyPatch(typeof(InteractivePlaceableMenuBoardTool), "Begin")]
    internal sealed class InteractivePlaceableMenuBoardToolPatch
    {
        // ReSharper disable InconsistentNaming
        internal static void Postfix()
        {
            
        }
    }

}
