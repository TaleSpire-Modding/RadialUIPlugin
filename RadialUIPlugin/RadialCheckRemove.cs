using System;
using System.Linq;
using System.Collections.Generic;

namespace RadialUI
{
	public delegate bool ShouldShowMenu(string menuText, string miniId, string targetId);
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
