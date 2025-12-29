using System.Linq;
using System.Collections.Generic;

namespace RadialUI
{
    public delegate bool ShouldShowMenu(string menuText, string miniId, string targetId);
    public delegate bool ShouldShowHVMenu(string menuText);

    internal static class RCRExt
    {
        public static bool CanAdd(this Dictionary<string, (string, ShouldShowMenu)> dic, string menuText, string miniId, string targetId)
        {
            return dic.Keys.Where(key => dic[key].Item1 == menuText).All(key => dic[key].Item2(menuText, miniId, targetId));
        }

        public static bool CanAdd(this Dictionary<string, List<RadialCheckRemove>> dic, string menuText, string miniId, string targetId)
        {
            IEnumerable<RadialCheckRemove> removers = dic.Values.SelectMany(l => l.Where(i => i.TitleToRemove == menuText));
            return removers.All(remover => !remover.ShouldRemoveCallback(menuText, miniId, targetId));
        }

        public static bool CanAdd(this Dictionary<string, List<RadialCheckRemove>> dic, string menuText)
        {
            IEnumerable<RadialCheckRemove> removers = dic.Values.SelectMany(l => l.Where(i => i.TitleToRemove == menuText));
            return removers.All(remover => !remover.ShouldRemoveHVCallback(menuText));
        }
    }

    public class RadialCheckRemove
    {
        public string TitleToRemove { get; set; }
        public ShouldShowMenu ShouldRemoveCallback { get; }
        public ShouldShowHVMenu ShouldRemoveHVCallback { get; }

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
