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

		public static string GetTitle(this MapMenuItem mapMenuItem)
		{
			FieldInfo mapField = mapMenuItem.GetType().GetField("_title", bindFlags);
			return (string)mapField.GetValue(mapMenuItem);
		}

		public static string GetTitle(this MapMenu mapMenu)
		{
			Transform transform = mapMenu.transform.GetChild(0).GetChild(0);
			MapMenuItem mapComponent = transform.GetComponent<MapMenuItem>();
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
	}
}
