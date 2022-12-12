using Bounce.Unmanaged;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RadialUI
{

    public static class Checkers
    {
		/// <summary>
		/// Wraps a checker to see if a player can control mini
		/// </summary>
		/// <param name="s1">Selected Mini</param>
		/// <param name="s2">Mini being targeted</param>
		/// <returns>Determines if player has control</returns>
        public static bool HasOwnership(NGuid s1, NGuid s2)
        {
            return CreatureManager.PlayerCanControlCreature(LocalPlayer.Id, new CreatureGuid(s2));
        }
    }
	public static class Talespire
	{
		public static class RadialMenus
		{
			static BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

			public static List<MapMenu> GetAllOpen()
			{
				var field = MapMenuManager.Instance.GetType().GetField("_allOpenMenus", bindFlags);
				return field.GetValue(MapMenuManager.Instance) as List<MapMenu>;
			}

			public static NGuid GetTargetCreatureId()
			{
				var targetCreature = GetTargetCreature();
				if (targetCreature == null)
					return NGuid.Empty;

				return targetCreature.CreatureId.Value;
			}

			public static float GetHeightDiff()
			{
				CreatureMenuBoardTool creatureMenuBoardTool = GetCreatureMenuBoardTool();
				if (creatureMenuBoardTool == null)
					return 0f;
				FieldInfo mapField = creatureMenuBoardTool.GetType().GetField("_hitHeightDif", bindFlags);
				return (float)mapField.GetValue(creatureMenuBoardTool);
			}

			public static CreatureBoardAsset GetTargetCreature()
			{
				CreatureMenuBoardTool creatureMenuBoardTool = GetCreatureMenuBoardTool();
				FieldInfo mapField = creatureMenuBoardTool.GetType().GetField("_selectedCreature", bindFlags);
				return mapField.GetValue(creatureMenuBoardTool) as CreatureBoardAsset;
			}

			public static HideVolumeItem GetSelectedHideVolumeItem()
			{
				GMHideVolumeMenuBoardTool gmHideVolumeMenuBoardTool = GetGMHideVolumeMenuBoardTool();
				FieldInfo mapField = gmHideVolumeMenuBoardTool.GetType().GetField("_selectedVolume", bindFlags);
				return mapField.GetValue(gmHideVolumeMenuBoardTool) as HideVolumeItem;
			}

			// Get Pos for Hide Volume
			public static Vector3 GetRadialTargetHideVolume()
			{
				GMHideVolumeMenuBoardTool gmHideVolumeMenuBoardTool = GetGMHideVolumeMenuBoardTool();
				FieldInfo mapField = gmHideVolumeMenuBoardTool.GetType().GetField("_selectedPos", bindFlags);
				return (Vector3)mapField.GetValue(gmHideVolumeMenuBoardTool);
			}

			public static CreatureMenuBoardTool GetCreatureMenuBoardTool()
			{
				return GameObject.FindObjectOfType(typeof(CreatureMenuBoardTool)) as CreatureMenuBoardTool;
			}

			public static GMHideVolumeMenuBoardTool GetGMHideVolumeMenuBoardTool()
			{
				return (GMHideVolumeMenuBoardTool)GameObject.FindObjectOfType(typeof(GMHideVolumeMenuBoardTool));
			}
		}
	}
}
