using System.Collections.Generic;
using MultiSelect;

namespace RadialUI.Soft_Dependencies
{
    public static class MultiSelectWrapper
    {
        public static List<Creature> GetMultiSelectCreatures()
        {
            List<Creature> SelectedCreatures;
            try
            {
                SelectedCreatures = MultiSelectPlugin.SelectedCreatures;
            }
            catch
            {
                SelectedCreatures = new List<Creature>();
            }
            return SelectedCreatures;
        }
    }
}