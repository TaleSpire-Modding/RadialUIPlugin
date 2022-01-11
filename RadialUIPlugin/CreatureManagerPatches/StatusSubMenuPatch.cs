using BepInEx;
using DataModel;
using HarmonyLib;
using UnityEngine;

namespace RadialUI
{
    [HarmonyPatch(typeof(CreatureMoveBoardTool), "MaybePickUpCreature")]
    internal class CreatureMoveBoardToolPatch
    {
        internal static bool Prefix(CameraController.CameraClickEvent click)
        {
            CreatureBoardAsset creatureBoardAsset;
            var r = PixelPickingManager.TryGetPicked(out _, out PlaceableRef _, out creatureBoardAsset, out Component _);
            if (r == PixelPickingManager.PickedKind.CreatureBoardAsset && Input.GetKey(KeyCode.LeftControl))
            {
                var current = creatureBoardAsset;
                if (RadialUIPlugin.SelectedCreatures.Contains(current.Creature))
                    RadialUIPlugin.SelectedCreatures.Remove(current.Creature);
                else RadialUIPlugin.SelectedCreatures.Add(current.Creature);
                return false;
            }
            return true;
        }
    }
}