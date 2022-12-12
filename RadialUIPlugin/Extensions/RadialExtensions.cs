using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RadialUI.Extensions
{
	public static class RadialExtensions
	{
		private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        public static bool CanShow(this Dictionary<string, List<RadialCheckRemove>> listOfRemovers, string menuId,string minid, string targetid)
        {
            var list = listOfRemovers.Values.SelectMany(l => l.Where(r => r.TitleToRemove == menuId && r.ShouldRemoveCallback != null));
            return list.All(r => !r.ShouldRemoveCallback(menuId, minid, targetid));
        }
    }
}
