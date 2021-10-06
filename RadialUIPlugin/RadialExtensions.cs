using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RadialUI
{
	public static class RadialExtensions
	{
		private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        public static bool CanShow(this Dictionary<string, List<RadialCheckRemove>> listOfRemovers, string menuId,string minid, string targetid)
        {
            var list = listOfRemovers.Values.SelectMany(l => l.Where(r => r.TitleToRemove == menuId && r.ShouldRemoveCallback != null));
            return list.All(r => !r.ShouldRemoveCallback(menuId, minid, targetid));
        }

        public static void RemoveItemByString(this MapMenu mapMenu, string title)
        {

        }

		public static string GetTitle(this MapMenuItem mapMenuItem)
		{
			if (mapMenuItem == null)
				return null;
			FieldInfo mapField = mapMenuItem.GetType().GetField("_title", bindFlags);
			if (mapField == null)
			{
				GameObject TXT_Title = mapMenuItem.gameObject.FindChild("TXT_Title");
				if (TXT_Title != null)
				{
					Component textMeshProUGUI = TXT_Title.GetComponent("TextMeshProUGUI");
					if (textMeshProUGUI != null)
					{
						PropertyInfo textProperty = textMeshProUGUI.GetType().GetProperty("text", bindFlags);
						if (textProperty != null)
							return textProperty.GetValue(textMeshProUGUI) as string;
					}
				}
				return null;
			}
			return (string)mapField.GetValue(mapMenuItem);
		}

		public static string GetTitle(this MapMenu mapMenu)
		{
			if (mapMenu == null)
				return null;
			Transform transform = mapMenu.transform.GetChild(0).GetChild(0);
			MapMenuItem mapComponent = transform.GetComponent<MapMenuItem>();
			if (mapComponent == null)
				mapComponent = transform.GetComponent<MapMenuStatItem>();
			if (mapComponent == null)
				return null;
			return mapComponent.GetTitle();
		}

		public static MapMenu.ItemArgs Clone(this MapMenu.ItemArgs mapArgs)
		{
			return new MapMenu.ItemArgs
			{
				Title = mapArgs.Title,
				Icon = mapArgs.Icon,
				CloseMenuOnActivate = mapArgs.CloseMenuOnActivate,
				ValueText = mapArgs.ValueText,
				SubValueText = mapArgs.SubValueText,
				FadeName = mapArgs.FadeName,
				Obj = mapArgs.Obj,
			};
		}

		public static GameObject FindChild(this GameObject gameObject, string name, bool includeInactive = false)
		{
			Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>(includeInactive);
			RectTransform[] rectTransforms = gameObject.GetComponentsInChildren<RectTransform>(includeInactive);

			if (childTransforms != null)
				foreach (Transform transform in childTransforms)
				{
					GameObject child = transform.gameObject;
					if (child?.name == name)
						return child;
				}

			if (rectTransforms != null)
				foreach (Transform transform in rectTransforms)
				{
					GameObject child = transform.gameObject;
					if (child?.name == name)
						return child;
				}

			return null;
		}
	}
}
