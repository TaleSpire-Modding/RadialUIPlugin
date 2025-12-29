using System;
using BepInEx;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx.Logging;
using PluginUtilities;

namespace RadialUI
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(SetInjectionFlag.Guid)]

    public sealed partial class RadialUIPlugin : BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.RadialUIPlugin";
        public const string Version = "0.0.0.0";
        public const string Name = "RadialUIPlugin";
        internal static ManualLogSource logger;

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            logger = Logger;
            Logger.LogInfo("In Awake for RadialUI");

            Harmony harmony = new Harmony(Guid);
            try
            {
                harmony.PatchAll();
                Logger.LogDebug("RadialUI Plug-in loaded");
                ModdingTales.ModdingUtils.AddPluginToMenuList(this, "HolloFoxes'");
            }
            catch (Exception e)
            {
                Logger.LogDebug("RadialUI Failed to patch");
                Logger.LogError(e);
                harmony.UnpatchSelf();
                Logger.LogDebug("unpatching RadialUI");
            }
        }

        /// <summary>
        /// Adds a checker to a menu to check if it should render.
        /// </summary>
        /// <param name="data">Plugin's Database of checker</param>
        /// <param name="key">The plugin's GUID or key</param>
        /// <param name="value">The specific value being cleared from dictionary's list.</param>
        /// <param name="shouldRemoveCallback">the callback to call upon checking</param>
        public static void AddRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value,
            ShouldShowMenu shouldRemoveCallback)
        {
            if (!data.ContainsKey(key))
                data.Add(key, new List<RadialCheckRemove>());
            if (shouldRemoveCallback == null) shouldRemoveCallback = AlwaysTrue;
            data[key].Add(new RadialCheckRemove(value, shouldRemoveCallback));
        }

        /// <summary>
        /// Default method callback for RadialCheckRemove.
        /// Parameters are to satisfy delete struct.
        /// </summary>
        /// <param name="s1">title text</param>
        /// <param name="s2">mini id</param>
        /// <param name="s3">target id</param>
        /// <returns>True</returns>
        public static bool AlwaysTrue(string s1, string s2, string s3)
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
        public static bool RemoveRemoveOn(Dictionary<string, List<RadialCheckRemove>> data, string key, string value)
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