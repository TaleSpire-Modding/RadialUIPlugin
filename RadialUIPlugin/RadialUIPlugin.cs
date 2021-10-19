using BepInEx;
using Bounce.Unmanaged;
using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace RadialUI
{

	[BepInPlugin(Guid, "RadialUIPlugin", Version)]
	public partial class RadialUIPlugin : BaseUnityPlugin
	{
		// constants
		public const string Guid = "org.hollofox.plugins.RadialUIPlugin";
		public const string Version = "1.7.0.0";

		/// <summary>
		/// Awake plugin
		/// </summary>
		void Awake()
		{
			Logger.LogInfo("In Awake for RadialUI");
            Debug.Log("RadialUI Plug-in loaded");

            var harmony = new Harmony(Guid);
            harmony.PatchAll();
		}


		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuEmotes = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuGm = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuAttacks = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
		private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuSize = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuEmotes = new Dictionary<string, List<RadialCheckRemove>>();
        private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuGm = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuAttacks = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuSize = new Dictionary<string, List<RadialCheckRemove>>();
		private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnHideVolume = new Dictionary<string, List<RadialCheckRemove>>();

		public static void AddOnSubmenuEmotes(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuEmotes.Add(key, (value, externalCheck));
        public static void AddOnSubmenuGm(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuGm.Add(key, (value, externalCheck));
		public static void AddOnSubmenuAttacks(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuAttacks.Add(key, (value, externalCheck));
		public static void AddOnSubmenuSize(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onSubmenuSize.Add(key, (value, externalCheck));

		// Add On HideVolume
		
		// Remove On Character
        public static bool RemoveOnSubmenuEmotes(string key) => _onSubmenuEmotes.Remove(key);
        public static bool RemoveOnSubmenuGm(string key) => _onSubmenuGm.Remove(key);
		public static bool RemoveOnSubmenuAttacks(string key) => _onSubmenuAttacks.Remove(key);
		public static bool RemoveOnSubmenuSize(string key) => _onSubmenuSize.Remove(key);

		
		// Add RemoveOn
		public static void AddOnRemoveSubmenuAttacks(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuAttacks, key, value, callback);
        public static void AddOnRemoveHideVolume(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnHideVolume, key, value, callback);
		public static void AddOnRemoveSubmenuEmotes(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuEmotes, key, value, callback);
		public static void AddOnRemoveSubmenuGm(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuGm, key, value, callback);
        public static void AddOnRemoveSubmenuSize(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnSubmenuSize, key, value, callback);

		// Remove RemoveOn
		public static void RemoveOnRemoveSubmenuAttacks(string key, string value) => RemoveRemoveOn(_removeOnSubmenuAttacks, key, value);
        public static void RemoveOnRemoveHideVolume(string key, string value) => RemoveRemoveOn(_removeOnHideVolume, key, value);
		public static void RemoveOnRemoveSubmenuEmotes(string key, string value) => RemoveRemoveOn(_removeOnSubmenuEmotes, key, value);
		public static void RemoveOnRemoveSubmenuGm(string key, string value) => RemoveRemoveOn(_removeOnSubmenuGm, key, value);
        public static void RemoveOnRemoveSubmenuSize(string key, string value) => RemoveRemoveOn(_removeOnSubmenuSize, key, value);

		internal static void AddRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value, ShouldShowMenu shouldRemoveCallback)
		{
			if (!data.ContainsKey(key))
				data.Add(key, new List<RadialCheckRemove>());
			data[key].Add(new RadialCheckRemove(value, shouldRemoveCallback));
		}

		internal static bool RemoveRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value)
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
    }
}
