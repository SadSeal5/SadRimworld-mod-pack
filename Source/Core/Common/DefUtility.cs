using RimWorld;
using Verse;

namespace SadSeal.Common
{
    public static class DefUtility
    {
        public static void HideThingDef(string defName)
        {
            var def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            if (def != null)
            {
                def.designationCategory = null;
            }
        }

        public static void HideRecipeDef(string defName)
        {
            var def = DefDatabase<RecipeDef>.GetNamedSilentFail(defName);
            if (def != null)
            {
                def.recipeUsers?.Clear();
            }
        }

        public static void DisableWorkGiver(string defName)
        {
            var def = DefDatabase<WorkGiverDef>.GetNamedSilentFail(defName);
            if (def != null)
            {
                var workTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
                for (int i = 0; i < workTypes.Count; i++)
                {
                    workTypes[i].workGiversByPriority?.Remove(def);
                }
            }
        }
    }
}

namespace SadSeal.Core.Common
{
    // Alias for explicit SadSeal.Core.Common namespace references
    public static class DefUtility
    {
        public static void HideThingDef(string defName) => SadSeal.Common.DefUtility.HideThingDef(defName);
        public static void HideRecipeDef(string defName) => SadSeal.Common.DefUtility.HideRecipeDef(defName);
        public static void DisableWorkGiver(string defName) => SadSeal.Common.DefUtility.DisableWorkGiver(defName);
    }
}
