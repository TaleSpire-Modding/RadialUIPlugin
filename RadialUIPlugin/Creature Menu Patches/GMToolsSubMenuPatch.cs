using System;
using System.Collections.Generic;
using BepInEx;
using Bounce.Unmanaged;
using HarmonyLib;
using RadialUI.Extensions;

namespace RadialUI
{
    public partial class RadialUIPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)> _onSubmenuGm =
            new Dictionary<string, (MapMenu.ItemArgs, Func<NGuid, NGuid, bool>)>();

        internal static readonly Dictionary<string, List<RadialCheckRemove>> _removeOnSubmenuGm =
            new Dictionary<string, List<RadialCheckRemove>>();

        // Same Methods, different Signatures
        public static void AddCustomButtonGMSubmenu(string key, MapMenu.ItemArgs value,
            Func<NGuid, NGuid, bool> externalCheck = null)
        {
            _onSubmenuGm.Add(key, (value, externalCheck));

            MapMenu.ItemArgs items = new MapMenu.ItemArgs
            {
                Action = (MapMenuItem, Object) =>
                {
                    if (LocalClient.TryGetLassoedCreatureIds(out CreatureGuid[] ids))
                    {
                        int num = ids.Length;
                        for (int i = 0; i < num; i++)
                        {
                            value.Action(MapMenuItem, ids[i].Value);
                        }
                    }
                },
                Title = value.Title,
                Icon = value.Icon,
                SubValueText = value.SubValueText,
                Scale = value.Scale,
                CloseMenuOnActivate = value.CloseMenuOnActivate,
                FadeName = value.FadeName,
                ValueText = value.ValueText,
                Obj = value.Obj,
            };

            _onGroupSubmenuGm.Add(key, (items, externalCheck));
        }
        public static bool RemoveCustomButtonGMSubmenu(string key)
        {
            return _onSubmenuGm.Remove(key) && _onGroupSubmenuGm.Remove(key);
        }
        public static void HideDefaultEmotesGMItem(string key, string value, ShouldShowMenu callback = null)
        {
            AddRemoveOn(_removeOnSubmenuGm, key, value, callback);
        }
        public static void UnHideDefaultGMSubmenuItem(string key, string value)
        {
            RemoveRemoveOn(_removeOnSubmenuGm, key, value);
        }
    }
}

namespace RadialUI.Creature_Menu_Patches
{
    [HarmonyPatch(typeof(CreatureMenuBoardTool), "Menu_GMTools")]
    internal sealed class GMToolsSubMenuPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(MapMenu map, object obj, CreatureBoardAsset ____selectedCreature)
        {
            NGuid miniId = LocalClient.SelectedCreatureId.Value;
            NGuid targetId = ____selectedCreature.CreatureId.Value;

            map.TryHideItem(RadialUIPlugin._removeOnSubmenuGm, "", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuGm, "Rename", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuGm, "Make Not Unique", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuGm, "Make Unique", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuGm, "Set Size", miniId.ToString(), targetId.ToString());
            map.TryHideItem(RadialUIPlugin._removeOnSubmenuGm, "BaseCikir", miniId.ToString(), targetId.ToString());

            map.AddItems(RadialUIPlugin._onSubmenuGm, targetId);
        }
    }
}