using Bounce.Unmanaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RadialUI
{
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
				Creature targetCreature = GetTargetCreature();
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

			public static Creature GetTargetCreature()
			{
				CreatureMenuBoardTool creatureMenuBoardTool = GetCreatureMenuBoardTool();
				FieldInfo mapField = creatureMenuBoardTool.GetType().GetField("_selectedCreature", bindFlags);
				return mapField.GetValue(creatureMenuBoardTool) as Creature;
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
