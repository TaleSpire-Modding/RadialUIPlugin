using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BepInEx;
using Bounce.Unmanaged;

namespace RadialUI
{

    [BepInPlugin(Guid, "RadialUIPlugin", Version)]
    public class RadialUIPlugin : BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.RadialUIPlugin";
        private const string Version = "1.1.0.0";

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Logger.LogInfo("In Awake for RadialUI");

            Debug.Log("RadialUI Plug-in loaded");
        }


        // Character Related
        private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid,NGuid, bool>)> _onCharacterCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCanAttack = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();
        private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onCantAttack = new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        // Hide Volumes
        private static readonly Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem,bool>)> _onHideVolumeCallback = new Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem,bool>)>();

        // Add On Character
        public static void AddOnCharacter(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCharacterCallback.Add(key,(value, externalCheck));
        public static void AddOnCanAttack(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCanAttack.Add(key,(value, externalCheck));
        public static void AddOnCantAttack(string key, MapMenu.ItemArgs value, Func<NGuid, NGuid, bool> externalCheck = null) => _onCantAttack.Add(key,(value, externalCheck));

        // Add On HideVolume
        public static void AddOnHideVolume(string key, MapMenu.ItemArgs value, Func<HideVolumeItem,bool> externalCheck = null) => _onHideVolumeCallback.Add(key, (value, externalCheck));

        // Remove On Character
        public static bool RemoveOnCharacter(string key) => _onCharacterCallback.Remove(key);
        public static bool RemoveOnCanAttack(string key) => _onCanAttack.Remove(key);
        public static bool RemoveOnCantAttack(string key) => _onCantAttack.Remove(key);

        // Remove On HideVolume
        public static bool RemoveOnHideVolume(string key) => _onHideVolumeCallback.Remove(key);

        // Check to see if map menu is new
        private static bool last = false;
        
        /// <summary>
        /// Looping method run by plugin
        /// </summary>
        void Update()
        {
            if (MapMenuManager.HasInstance && MapMenuManager.IsOpen)
            {
                if (!last)
                {
                    Probe();
                    last = true;
                }
            }
            else
            {
                last = false;
            }
        }

        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private void Probe()
        { 
            var instance = MapMenuManager.Instance;
            var type = instance.GetType();
            
            var field = type.GetField("_allOpenMenus", bindFlags);
            var menus = (List<MapMenu>) field.GetValue(instance);

            if (menus.Count > 0)
            {
                var map = menus[0];
                var Map = map.transform.GetChild(0).GetChild(0);
                var mapComponent = Map.GetComponent<MapMenuItem>();

                var mapField = mapComponent.GetType().GetField("_title", bindFlags);
                var title = (string)mapField.GetValue(mapComponent);

                var id = LocalClient.SelectedCreatureId.Value;

                if (IsMini(title)) AddCreatureEvent(_onCharacterCallback,id,map);
                if (CanAttack(title)) AddCreatureEvent(_onCanAttack, id, map);
                if (CanNotAttack(title)) AddCreatureEvent(_onCantAttack, id, map);

                if (IsHideVolume(title)) AddHideVolumeEvent(_onHideVolumeCallback, map);
            }
        }

        private void AddCreatureEvent(Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> dic, NGuid myCreature, MapMenu map)
        {
            var target = myCreature;
            if (dic == _onCanAttack || dic == _onCharacterCallback)
            {
                target = GetRadialTargetCreature();
            }
            foreach (var handlers
                in dic.Values
                    .Where(handlers => handlers.Item2 == null
                                       || handlers.Item2(myCreature, target))) map.AddItem(handlers.Item1);
        }

        public static NGuid GetRadialTargetCreature()
        {
            var x = (CreatureMenuBoardTool)GameObject.FindObjectOfType(typeof(CreatureMenuBoardTool));

            FieldInfo mapField = x.GetType().GetField("_selectedCreature", bindFlags);
            var selectedCreature = (Creature)mapField.GetValue(x);
            return selectedCreature.CreatureId.Value;
        }

        private void AddHideVolumeEvent(Dictionary<string, (MapMenu.ItemArgs, Func<HideVolumeItem, bool>)> dic, MapMenu map)
        {
            var target = GetSelectedHideVolumeItem();
            foreach (var handlers
                in dic.Values
                    .Where(handlers => handlers.Item2 == null
                                       || handlers.Item2(target))) map.AddItem(handlers.Item1);
        }

        private static HideVolumeItem GetSelectedHideVolumeItem()
        {
            var tool = (GMHideVolumeMenuBoardTool)GameObject.FindObjectOfType(typeof(GMHideVolumeMenuBoardTool));
            FieldInfo mapField = tool.GetType().GetField("_selectedVolume", bindFlags);
            return (HideVolumeItem) mapField.GetValue(tool);
        }


        // Current ShortHand to see if Mini
        private bool IsMini(string title) => title == "Emotes" || title == "Attacks";
        private bool CanAttack(string title) => title == "Attacks";
        private bool CanNotAttack(string title) => title == "Emotes";
        
        // Current ShortHand to see if HideVolume
        private bool IsHideVolume(string title) => title == "Toggle Visibility";
    }
}
