using BepInEx;
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
		public const string Version = "2.0.7.0";

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

		/// <summary>
		/// Adds a checker to a menu to check if it should render.
		/// </summary>
		/// <param name="data">Plugin's Database of checker</param>
        /// <param name="key">The plugin's GUID or key</param>
        /// <param name="value">The specific value being cleared from dictionary's list.</param>
		/// <param name="shouldRemoveCallback">the callback to call upon checking</param>
		internal static void AddRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value, ShouldShowMenu shouldRemoveCallback)
		{
			if (!data.ContainsKey(key))
				data.Add(key, new List<RadialCheckRemove>());
            if (shouldRemoveCallback == null) shouldRemoveCallback = alwaysTrue;
			data[key].Add(new RadialCheckRemove(value, shouldRemoveCallback));
		}

		/// <summary>
		/// Default method callback for RadialCheckRemove.
		/// Paramaters are to satisfy delete struct.
		/// </summary>
		/// <param name="s1">title text</param>
		/// <param name="s2">mini id</param>
		/// <param name="s3">target id</param>
		/// <returns></returns>
        internal static bool alwaysTrue(string s1, string s2, string s3)
        {
            return true;
        }

		/// <summary>
		/// Removes a default hider on an existing dictionary.
		/// </summary>
		/// <param name="data">the dictionary storing the existing menus to hide.</param>
		/// <param name="key">The plugin's GUID or key</param>
		/// <param name="value">The specific value being cleared from dictionary's list.</param>
		/// <returns></returns>
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