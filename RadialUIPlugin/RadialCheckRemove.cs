using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RadialUI
{
	public delegate bool ShouldShowMenu(string menuText, string miniId, string targetId);

    internal static class RCRExt
    {
        public static bool CanAdd(this Dictionary<string, (string, ShouldShowMenu)> dic, string menuText, string miniId, string targetId)
        {
            return dic.Keys.Where(key => dic[key].Item1 == menuText).All(key => dic[key].Item2(menuText, miniId, targetId));
        }
    }

    public class RadialCheckRemove
	{
		public string TitleToRemove { get; set; }
		public ShouldShowMenu ShouldRemoveCallback { get; }

		public RadialCheckRemove()
		{

		}

		public RadialCheckRemove(string titleToRemove, ShouldShowMenu shouldRemoveCallback)
		{
			TitleToRemove = titleToRemove;
			ShouldRemoveCallback = shouldRemoveCallback;
		}
	}
}
