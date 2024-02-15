using UnityEngine;
using Bounce.Unmanaged;
using System.Collections.Generic;
using System;

namespace RadialUI
{
	public class RadialSubmenu
	{
		// Radial menu selected asset
		private static CreatureGuid radialAsset = default;
		private static HideVolumeItem radialHideVolume;

		// Hold sub-entries for main menus
		private static Dictionary<string, List<MapMenu.ItemArgs>> subMenuEntries = new Dictionary<string, List<MapMenu.ItemArgs>>();

		private static Dictionary<MapMenu.ItemArgs, Func<bool>> subMenuChecker =
				new Dictionary<MapMenu.ItemArgs, Func<bool>>();

		/// <summary>
		/// Enumeration for the type of menu. Hidden volumes are not currently supported since their callback is a little
		/// different but they could be added with a little bit of extra effort
		/// </summary>
		public enum MenuType
		{
			character = 1,
			canAttack,
			cantAttack,
			HideVolume,
			GMBlock,
		}

		private static MenuType openMenuType = MenuType.character;

		/*
		/// <inheritdoc cref="EnsureMainMenuItem(string,RadialUI.RadialSubmenu.MenuType,string,UnityEngine.Sprite,System.Func{Bounce.Unmanaged.NGuid,Bounce.Unmanaged.NGuid,bool})"/>
		public static void EnsureMainMenuItem(string mainGuid, MenuType type, string title, Sprite icon,
				Func<NGuid, NGuid, bool> callback) => EnsureMainMenuItem(mainGuid, type, title, icon, callback,null);

		/// <inheritdoc cref="EnsureMainMenuItem(string,RadialUI.RadialSubmenu.MenuType,string,UnityEngine.Sprite,System.Func{HideVolumeItem,bool})"/>
		public static void EnsureMainMenuItem(string mainGuid, MenuType type, string title, Sprite icon,
				Func<HideVolumeItem, bool> callback) => EnsureMainMenuItem(mainGuid, type, title, icon, null, callback);*/

		/// <summary>
		/// Method that a plugin uses to ensure that the desired main (radial) menu item exits.
		/// The method creates the entry if it does not exists and ignore the requested if it already exists.
		/// For multiple plugins to share a main menu entry, they need to use the same guid.
		/// </summary>
		/// <param name="mainGuid">Guid for the main menu entry</param>
		/// <param name="type">Determines which type of radial menu the entry is for</param>
		/// <param name="title">Text that is associated with the entry</param>
		/// <param name="icon">Icon that should be displayed</param>
		public static void EnsureMainMenuItem(string mainGuid, MenuType type, string title, Sprite icon)
		{
			// Don't create the main menu entry multiple times
			if (subMenuEntries.ContainsKey(mainGuid)) { return; }
			// Create the main menu entry based on the type of menu
			switch (type)
			{
				case MenuType.character:
					EnsureMainMenuOnCharacter(mainGuid, title, icon);
					break;
                case MenuType.HideVolume:
					EnsureMainMenuOnHideVolume(mainGuid, title, icon);
					break;
			}
			// Add a list into the dictionary to hold sub-menu entries for the main menu entry
			// (Presence of an entry in this dictionary means the main menu entry has already been created)
			subMenuEntries.Add(mainGuid, new List<MapMenu.ItemArgs>());
		}

		private static void EnsureMainMenuOnHideVolume(string mainGuid, string title, Sprite icon)
		{
			RadialUIPlugin.AddOnHideVolume(mainGuid, NewMainMenu(mainGuid, title, icon), Reporter);
			openMenuType = MenuType.HideVolume;
		}


		private static void EnsureMainMenuOnCharacter(string mainGuid, string title, Sprite icon)
		{
			RadialUIPlugin.AddCustomButtonOnCharacter(mainGuid, NewMainMenu(mainGuid, title, icon), Reporter);
			openMenuType = MenuType.character;
		}

		public static MapMenu.ItemArgs NewMainMenu(string mainGuid, string title, Sprite icon)
		{
			return new MapMenu.ItemArgs()
			{
				// Add mainGuid into the callback so we can look up the corresponding sub-menu entries
				Action = (mmi, obj) => { DisplaySubmenu(mmi, obj, mainGuid); },
				Icon = icon,
				Title = title,
				CloseMenuOnActivate = false
			};
		}

		[Obsolete]
		public static void CreateSubMenuItem(string mainGuid, string title, Sprite icon,
						Action<CreatureGuid, string, MapMenuItem> callback, bool closeMenu)
		{
			CreateSubMenuItem(mainGuid, title, icon, callback, closeMenu, null);
		}

		[Obsolete]
		public static void CreateSubMenuItem(string mainGuid, string title, Sprite icon, 
				Action<CreatureGuid, string, MapMenuItem> callback)
		{
			CreateSubMenuItem(mainGuid, title, icon, callback, true, null);
		}

		/// <summary>
		/// Add sub-menu items to a main menu entry
		/// </summary>
		/// <param name="mainGuid">Guid of the main menu entry</param>
		/// <param name="title">Text associated with the sub-menu item</param>
		/// <param name="icon">Icon associated with the sub-menu item</param>
		/// <param name="callback">Callback that is called when the sub-menu item is selected</param>
		/// <param name="closeMenu">Determines if the menu is closed after sub-menu item is selected</param>
		/// <param name="checker">Optional checker used to determine whether to add button for submenu</param>
		public static void CreateSubMenuItem(string mainGuid, string title, Sprite icon, Action<CreatureGuid, string, MapMenuItem> callback, bool closeMenu = true, Func<bool> checker = null)
		{
			// Check if the main menu Guid exists
			if (!subMenuEntries.ContainsKey(mainGuid))
			{
				Debug.LogWarning($"Main radial menu '{mainGuid}' does not exits. Use EnsureMainMenuItem() before adding sub-menu items.");
				return;
			}
			// Add the item to the sub-menu item dictionary for the main menu entry (indicated by the Guid)
			var item = new MapMenu.ItemArgs
			{
				// Parent plugin specified callback for when the sub-menu item is selected
				Action = (mmi, obj) => { callback(radialAsset, mainGuid, mmi); },
				Icon = icon,
				Title = title,
				CloseMenuOnActivate = closeMenu
			};
			subMenuEntries[mainGuid].Add(item);
			if (checker != null) subMenuChecker.Add(item, checker);
		}

		/// <summary>
		/// Add sub-menu items to a maim menu entry
		/// </summary>
		/// <param name="mainGuid">Guid of the main menu entry</param>
		/// <param name="title">Text associated with the sub-menu item</param>
		/// <param name="icon">Icon associated with the sub-menu item</param>
		/// <param name="callback">Callback that is called when the sub-menu item is selected</param>
		/// <param name="closeMenu">Determines if the menu is closed after sub-menu item is selected</param>
		/// <param name="checker">Optional checker used to determine whether to add button for submenu</param>
		public static void CreateSubMenuItem(string mainGuid, string title, Sprite icon, Action<HideVolumeItem, string, MapMenuItem> callback, bool closeMenu = true, Func<bool> checker = null)
		{
			// Check if the main menu Guid exists
			if (!subMenuEntries.ContainsKey(mainGuid))
			{
				Debug.LogWarning("Main radial menu '" + mainGuid + "' does not exits. Use EnsureMainMenuItem() before adding sub-menu items.");
				return;
			}
			// Add the item to the sub-menu item dictionary for the main menu entry (indicated by the Guid)
			var item = new MapMenu.ItemArgs
			{
				// Parent plugin specified callback for when the sub-menu item is selected
				Action = (mmi, obj) => { callback(radialHideVolume, mainGuid, mmi); },
				Icon = icon,
				Title = title,
				CloseMenuOnActivate = closeMenu
			};
			subMenuEntries[mainGuid].Add(item);
			if (checker != null) subMenuChecker.Add(item, checker);
		}

		/// <summary>
		/// Add sub-menu items to a maim menu entry
		/// </summary>
		/// <param name="mainGuid">Guid of the main menu entry</param>
		/// <param name="item">Uses standard mapmenu item args for greater flexibility</param>
		/// <param name="callback">Callback that is called when the sub-menu item is selected if you want 3 parameter return</param>
		/// <param name="checker">Optional checker used to determine whether to add button for submenu</param>
		public static void CreateSubMenuItem(string mainGuid, MapMenu.ItemArgs item, Action<HideVolumeItem, string, MapMenuItem> callback = null, Func<bool> checker = null)
		{
			// Check if the main menu Guid exists
			if (!subMenuEntries.ContainsKey(mainGuid))
			{
				Debug.LogWarning("Main radial menu '" + mainGuid + "' does not exits. Use EnsureMainMenuItem() before adding sub-menu items.");
				return;
			}

			if (callback != null) item.Action = (mmi, obj) => { callback(radialHideVolume, mainGuid, mmi); };

			// Add the item to the sub-menu item dictionary for the main menu entry (indicated by the Guid)
			subMenuEntries[mainGuid].Add(item);
			if (checker != null) subMenuChecker.Add(item, checker);
		}

		/// <summary>
		/// Please use File Access Plugin to handle loading icons.
		/// </summary>
		[Obsolete]
        public static Sprite GetIconFromFile(string fileName)
        {
            return null;
        }

		/// <summary>
		/// Method used internally for tracking which mini the radial menu was opened on
		/// </summary>
		/// <param name="selected">Selected mini</param>
		/// <param name="radial">Radial menu mini</param>
		/// <returns></returns>
		private static bool Reporter(NGuid selected, NGuid radial)
		{
			radialAsset = new CreatureGuid(radial);
			return true;
		}

		/// <summary>
		/// Method used internally for tracking which mini the radial menu was opened on
		/// </summary>
		/// <param name="item">Radial menu for Hide Volume</param>
		/// <returns></returns>
		private static bool Reporter(HideVolumeItem item)
		{
			radialHideVolume = item;
			return true;
		}

		/// <summary>
		/// Method called by the main menu to generate the corresponding sub-menu
		/// </summary>
		/// <param name="mmi">MapMenuItem associated with the main menu</param>
		/// <param name="obj">Object associated with the main menu</param>
		/// <param name="mainGuid">Guid of the main menu</param>
		private static void DisplaySubmenu(MapMenuItem mmi, object obj, string mainGuid)
		{
			Vector3 result;
			if (openMenuType == MenuType.character)
				result = Talespire.RadialMenus.GetTargetCreature().transform.position + Vector3.up * Talespire.RadialMenus.GetHeightDiff();
			else if (openMenuType == MenuType.HideVolume)
				result = Talespire.RadialMenus.GetRadialTargetHideVolume();
			else
                result = GMBlockInteractMenuBoardTool.block.WorldPosition;

			// Create sub-menu
			MapMenu mapMenu = MapMenuManager.OpenMenu(result, true);

			// Populate sub-menu based on all items added by any plugins for the specific main menu entry
			foreach (MapMenu.ItemArgs item in subMenuEntries[mainGuid])
				if (!subMenuChecker.ContainsKey(item) || subMenuChecker[item]())
					mapMenu.AddItem(item);
        }
	}
}
