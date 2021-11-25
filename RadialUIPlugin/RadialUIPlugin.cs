using BepInEx;
using System.Collections.Generic;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace RadialUI
{
    [BepInPlugin(Guid, "RadialUIPlugin", Version)]
	public partial class RadialUIPlugin : BaseUnityPlugin
	{
		// constants
		public const string Guid = "org.hollofox.plugins.RadialUIPlugin";
		public const string Version = "2.0.4.0";

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

        
       private static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnHideVolume = new Dictionary<string, List<RadialCheckRemove>>();

        // Add RemoveOn
        public static void AddOnRemoveHideVolume(string key, string value, ShouldShowMenu callback = null) => AddRemoveOn(_removeOnHideVolume, key, value, callback);

        // Remove RemoveOn
		public static void RemoveOnRemoveHideVolume(string key, string value) => RemoveRemoveOn(_removeOnHideVolume, key, value);
		
		
		internal static void AddRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value, ShouldShowMenu shouldRemoveCallback)
		{
			if (!data.ContainsKey(key))
				data.Add(key, new List<RadialCheckRemove>());
            if (shouldRemoveCallback == null) shouldRemoveCallback = alwaysTrue;
			data[key].Add(new RadialCheckRemove(value, shouldRemoveCallback));
		}

        internal static bool alwaysTrue(string s1, string s2, string s3)
        {
            return true;
        }

        internal static NGuid last;

        public static NGuid GetLastRadialTargetCreature()
        {
			return last;
        }

        internal static HideVolumeItem lastHideVolume;

        public static HideVolumeItem GetLastRadialHideVolume()
        {
            return lastHideVolume;
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