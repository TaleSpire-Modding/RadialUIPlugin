using BepInEx;
using Bounce.Unmanaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using TaleSpireCore;
using Object = System.Object;

namespace RadialUI
{

	[BepInPlugin(Guid, "RadialUIPlugin", Version)]
	public class RadialUIPlugin : BaseUnityPlugin
	{
		// constants
		public const string Guid = "org.hollofox.plugins.RadialUIPlugin";
		public const string Version = "1.5.0.0";

		/// <summary>
		/// Awake plugin
		/// </summary>
		void Awake()
		{
			Logger.LogInfo("In Awake for RadialUI");

			Debug.Log("RadialUI Plug-in loaded");
		}


		// Character Related Add
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCharacterCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCanAttack = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCantAttack = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuEmotes = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuKill = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuGm = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuAttacks = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuSize = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

		// Character Related Remove
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnCharacter = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnCanAttack = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnCantAttack = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuEmotes = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuKill = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuGm = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuAttacks = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuSize = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnHideVolume = new Dictionary<string, List<RadialCheckRemove>>();

		// Hide Volumes
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> _onHideVolumeCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)>();

		// Add On Character
		public static void AddOnCharacter(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCharacterCallback.Add(key, (value, externalCheck));
		public static void AddOnCanAttack(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCanAttack.Add(key, (value, externalCheck));
		public static void AddOnCantAttack(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCantAttack.Add(key, (value, externalCheck));
		public static void AddOnSubmenuEmotes(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuEmotes.Add(key, (value, externalCheck));
		public static void AddOnSubmenuKill(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuKill.Add(key, (value, externalCheck));
		public static void AddOnSubmenuGm(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuGm.Add(key, (value, externalCheck));
		public static void AddOnSubmenuAttacks(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuAttacks.Add(key, (value, externalCheck));
		public static void AddOnSubmenuSize(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuSize.Add(key, (value, externalCheck));

		// Add On HideVolume
		public static void AddOnHideVolume(string key, MapMenu.ItemArgs value, Func<HideVolumeItem, bool> externalCheck = null) => _onHideVolumeCallback.Add(key, (value, externalCheck));

		// Remove On Character
		public static bool RemoveOnCharacter(string key) => _onCharacterCallback.Remove(key);
		public static bool RemoveOnCanAttack(string key) => _onCanAttack.Remove(key);
		public static bool RemoveOnCantAttack(string key) => _onCantAttack.Remove(key);
		public static bool RemoveOnSubmenuEmotes(string key) => _onSubmenuEmotes.Remove(key);
		public static bool RemoveOnSubmenuKill(string key) => _onSubmenuKill.Remove(key);
		public static bool RemoveOnSubmenuGm(string key) => _onSubmenuGm.Remove(key);
		public static bool RemoveOnSubmenuAttacks(string key) => _onSubmenuAttacks.Remove(key);
		public static bool RemoveOnSubmenuSize(string key) => _onSubmenuSize.Remove(key);

		
		// Add RemoveOn
		public static void AddOnRemoveSubmenuAttacks(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuAttacks, key, value, callback);
		public static void AddOnRemoveCanAttack(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnCanAttack, key, value, callback);
		public static void AddOnRemoveCantAttack(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnCantAttack, key, value, callback);
		public static void AddOnRemoveCharacter(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnCharacter, key, value, callback);
		public static void AddOnRemoveHideVolume(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnHideVolume, key, value, callback);
		public static void AddOnRemoveSubmenuEmotes(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuEmotes, key, value, callback);
		public static void AddOnRemoveSubmenuGm(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuGm, key, value, callback);
		public static void AddOnRemoveSubmenuKill(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuKill, key, value, callback);
		public static void AddOnRemoveSubmenuSize(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuSize, key, value, callback);

		// Remove RemoveOn
		public static void RemoveOnRemoveSubmenuAttacks(string key, string value) => RemoveRemoveOn(_removeOnSubmenuAttacks, key, value);
		public static void RemoveOnRemoveCanAttack(string key, string value) => RemoveRemoveOn(_removeOnCanAttack, key, value);
		public static void RemoveOnRemoveCantAttack(string key, string value) => RemoveRemoveOn(_removeOnCantAttack, key, value);
		public static void RemoveOnRemoveCharacter(string key, string value) => RemoveRemoveOn(_removeOnCharacter, key, value);
		public static void RemoveOnRemoveHideVolume(string key, string value) => RemoveRemoveOn(_removeOnHideVolume, key, value);
		public static void RemoveOnRemoveSubmenuEmotes(string key, string value) => RemoveRemoveOn(_removeOnSubmenuEmotes, key, value);
		public static void RemoveOnRemoveSubmenuGm(string key, string value) => RemoveRemoveOn(_removeOnSubmenuGm, key, value);
		public static void RemoveOnRemoveSubmenuKill(string key, string value) => RemoveRemoveOn(_removeOnSubmenuKill, key, value);
		public static void RemoveOnRemoveSubmenuSize(string key, string value) => RemoveRemoveOn(_removeOnSubmenuSize, key, value);

		private static void AddRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value, ShouldShowMenu shouldRemoveCallback)
		{
			if (!data.ContainsKey(key))
				data.Add(key, new List<RadialCheckRemove>());
			data[key].Add(new RadialCheckRemove(value, shouldRemoveCallback));
		}

		private static bool RemoveRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value)
		{
			if (!data.ContainsKey(key))
				return false;
			List<RadialCheckRemove> radialCheckRemoves = data[key];
			int countBefore = radialCheckRemoves.Count;
			radialCheckRemoves.RemoveAll(x => x.TitleToRemove == value);
			bool successfullyRemoved = radialCheckRemoves.Count != countBefore;
			if (radialCheckRemoves.Count == 0)
				data.Remove(key);
			return successfullyRemoved;
		}

		// Remove On HideVolume
		public static bool RemoveOnHideVolume(string key) => _onHideVolumeCallback.Remove(key);

		// Check to see if map menu is new
		private static int lastMenuCount = 0;
		private static string lastTitle = "";
		private static bool lastSuccess = true;
		private static DateTime menuExecutionTime;

		// Last Creature/HideVolume
		private static NGuid target;
		private static HideVolumeItem lastHideVolumeItem;

		private (Action<MapMenuItem, Object>, MapMenuItem, Object) pendingMenuAction = (null, null, null);

		/// <summary>
		/// Looping method run by plugin
		/// </summary>
		void Update()
		{
			ExecutePendingMenuActionIfNecessary();

			if (!MapMenuManager.HasInstance || !MapMenuManager.IsOpen)
			{
				if (needToClear)
				{
					needToClear = false;
					lastMenuCount = 0;
					lastTitle = "";
					lastSuccess = true;
					ClearPreviouslyModded();
				}
				return;
			}

			needToClear = true;
			List<MapMenu> menus = Talespire.RadialMenus.GetAllOpen();

			if (menus.Count >= 1 && menus.Count >= lastMenuCount)
			{
				try
				{
					string title = menus[menus.Count - 1].GetTitle();

					if (menus.Count == lastMenuCount && lastTitle != title)
						lastMenuCount -= 1;

					ModifyMenus();
					lastTitle = title;
					lastSuccess = true;
				}
				catch (Exception e)
				{
					// Probably Stat open
					if (lastSuccess)
					{
						Debug.Log($"Error: {e}, most likely stat submenu being opened");
						lastSuccess = false;
					}
				}
			}
			lastMenuCount = menus.Count;
		}

		private void ExecutePendingMenuActionIfNecessary()
		{
			if (pendingMenuAction.Item1 == null || menuExecutionTime == DateTime.MinValue || DateTime.Now <= menuExecutionTime)
				return;

			pendingMenuAction.Item1(pendingMenuAction.Item2, pendingMenuAction.Item3);
			pendingMenuAction = (null, null, null);
			menuExecutionTime = DateTime.MinValue;
		}

		private void ModifyMenus()
		{
			List<MapMenu> menus = Talespire.RadialMenus.GetAllOpen();

			NGuid selectedCreatureId = LocalClient.SelectedCreatureId.Value;

			for (var i = lastMenuCount; i < menus.Count; i++)
			{
				MapMenu mapMenu = menus[i];
				string title = mapMenu.GetTitle();
				Debug.Log(title);

				// Minis Related
				if (IsMini(title)) AddMenuItem(_onCharacterCallback, selectedCreatureId, mapMenu);
				if (CanAttack(title)) AddMenuItem(_onCanAttack, selectedCreatureId, mapMenu);
				if (CanNotAttack(title)) AddMenuItem(_onCantAttack, selectedCreatureId, mapMenu);

				if (IsMini(title)) RemoveMenuItem(_removeOnCharacter, selectedCreatureId, mapMenu);
				if (CanAttack(title)) RemoveMenuItem(_removeOnCanAttack, selectedCreatureId, mapMenu);
				if (CanNotAttack(title)) RemoveMenuItem(_removeOnCantAttack, selectedCreatureId, mapMenu);

				// Minis Submenu
				if (IsEmotes(title)) AddMenuItem(_onSubmenuEmotes, selectedCreatureId, mapMenu);
				if (IsKill(title)) AddMenuItem(_onSubmenuKill, selectedCreatureId, mapMenu);
				if (IsGmMenu(title)) AddMenuItem(_onSubmenuGm, selectedCreatureId, mapMenu);
				if (IsAttacksMenu(title)) AddMenuItem(_onSubmenuAttacks, selectedCreatureId, mapMenu);
				if (IsSizeMenu(title)) AddMenuItem(_onSubmenuSize, selectedCreatureId, mapMenu);

				if (IsEmotes(title)) RemoveMenuItem(_removeOnSubmenuEmotes, selectedCreatureId, mapMenu);
				if (IsKill(title)) RemoveMenuItem(_removeOnSubmenuKill, selectedCreatureId, mapMenu);
				if (IsGmMenu(title)) RemoveMenuItem(_removeOnSubmenuGm, selectedCreatureId, mapMenu);
				if (IsAttacksMenu(title)) RemoveMenuItem(_removeOnSubmenuAttacks, selectedCreatureId, mapMenu);
				if (IsSizeMenu(title)) RemoveMenuItem(_removeOnSubmenuSize, selectedCreatureId, mapMenu);

				// Hide Volumes
				if (IsHideVolume(title)) RemoveMenuItem(_removeOnHideVolume, selectedCreatureId, mapMenu);
				if (IsHideVolume(title)) AddHideVolumeEvent(_onHideVolumeCallback, mapMenu);
			}
		}

		private static void RemoveMenuItem(Dictionary<string, List<RadialCheckRemove>> removeOnCharacter, NGuid miniAtMenu, MapMenu map)
		{
			target = Talespire.RadialMenus.GetTargetCreatureId();
			IEnumerable<string> indexes = removeOnCharacter.SelectMany(i => i.Value.Select(x => x.TitleToRemove)).Distinct();
			Debug.Log("We expect to be removing these titles (perhaps conditionally):");
			foreach (string index in indexes)
				Debug.Log("  " + index);

			Transform Map = map.transform.GetChild(0);
			for (int i = 0; i < Map.childCount; i++)
			{
				Transform transform = Map.GetChild(i);
				MapMenuItem mapMenuItem = transform.GetComponent<MapMenuItem>();
				string menuTitle = mapMenuItem.GetTitle();
				Debug.Log($"Map.GetChild({i}) = \"{menuTitle}\"");

				foreach (string key in removeOnCharacter.Keys)
					foreach (RadialCheckRemove radialCheckRemove in removeOnCharacter[key])
						if (radialCheckRemove.TitleToRemove == menuTitle &&
							(radialCheckRemove.ShouldRemoveCallback == null || radialCheckRemove.ShouldRemoveCallback(menuTitle, miniAtMenu.ToString(), target.ToString())))
						{
							Debug.Log($"RemoveRadialComponent - : {menuTitle}");
							transform.gameObject.SetActive(false);
							return;
						}
			}
		}

		List<MapMenu.ItemArgs> previouslyModded = new List<MapMenu.ItemArgs>();
		bool needToClear;

		public void ClearPreviouslyModded()
		{
			previouslyModded.Clear();
		}

		private void AddMenuItem(Dictionary<string, (MapMenu.ItemArgs mapArgs, Func<NGuid, NGuid, bool> shouldAdd)> dic, NGuid myCreature, MapMenu mapMenu)
		{
			target = Talespire.RadialMenus.GetTargetCreatureId();

			var validMenuMods = new List<(MapMenu.ItemArgs mapArgs, Func<NGuid, NGuid, bool> shouldAdd)>();
			foreach (string key in dic.Keys)
			{
				var menuMod = dic[key];
				if (menuMod.shouldAdd == null || menuMod.shouldAdd(myCreature, target))
					if (!previouslyModded.Contains(menuMod.mapArgs))
					{
						previouslyModded.Add(menuMod.mapArgs);
						validMenuMods.Add(menuMod);
					}
			}

			foreach (var handler in validMenuMods)
			{
				mapMenu.AddItem(CreateMenu(handler));
				int count = mapMenu.transform.childCount;
				Transform lastTransform = mapMenu.transform.GetChild(count - 1);
				RectTransform rectTransform = lastTransform.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
				rectTransform.sizeDelta = new Vector2(48f, 48f);
				//Debug.Log("Set Size 48");
			}
		}

		private MapMenu.ItemArgs CreateMenu((MapMenu.ItemArgs mapArgs, Func<NGuid, NGuid, bool> shouldAdd) handler)
		{
			MapMenu.ItemArgs clonedMenu = handler.mapArgs.Clone();
			clonedMenu.Action = (mmi, obj) =>
			{
				pendingMenuAction = (handler.mapArgs.Action, mmi, obj);
				menuExecutionTime = DateTime.Now.AddMilliseconds(200);
			};
			return clonedMenu;
		}

		/// <summary>
		/// Fetches the last creature the menu has been open on (menu open or closed)
		/// </summary>
		/// <returns>NGuid for the last creature that the Radial Menu has been opened on.</returns>
		public static NGuid GetLastRadialTargetCreature()
		{
			return target;
		}

		/// <summary>
		/// Fetches the last creature the menu has been open on (menu open or closed)
		/// </summary>
		/// <returns>NGuid for the last creature that the Radial Menu has been opened on.</returns>
		public static HideVolumeItem GetLastRadialHideVolume()
		{
			return lastHideVolumeItem;
		}

		private void AddHideVolumeEvent(Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> dic, MapMenu map)
		{
			lastHideVolumeItem = Talespire.RadialMenus.GetSelectedHideVolumeItem();
			foreach (var handlers
					in dic.Values
							.Where(handlers => handlers.Item2 == null
										 || handlers.Item2(lastHideVolumeItem))) map.AddItem(handlers.Item1);
		}


		// Current ShortHand to see if Mini
		private bool IsMini(string title) => title == "Emotes" || title == "Attacks";
		private bool CanAttack(string title) => title == "Attacks";
		private bool CanNotAttack(string title) => title == "Emotes";

		// Mini Submenus
		private bool IsEmotes(string title) => title == "Twirl";
		private bool IsKill(string title) => title == "Kill Creature";
		private bool IsGmMenu(string title) => title == "Player Permission";
		private bool IsAttacksMenu(string title) => title == "Attack";
		private bool IsSizeMenu(string title) => title == "0.5x0.5";

		// Current ShortHand to see if HideVolume
		private bool IsHideVolume(string title) => title == "Toggle Visibility";
	}
}
