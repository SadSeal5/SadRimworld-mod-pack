using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Incubation
{
    public static class GrowthVatUtility
    {
        private static readonly MethodInfo FinishMethod =
            typeof(Building_GrowthVat).GetMethod("Finish", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static void EjectContents(this Building_GrowthVat vat)
        {
            if (vat == null) return;

            try
            {
                if (FinishMethod != null)
                {
                    FinishMethod.Invoke(vat, null);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[SadSeal.Incubation] Failed to eject contents from growth vat: {ex}");
            }
        }

        public static bool IsUnpoweredOrStarving(this Building_GrowthVat vat, IncubationSettings settings)
        {
            if (vat == null || settings == null) return false;
            bool powerRequired = settings.powerUsageMultiplier > 0.001f;
            bool unpowered = powerRequired && !vat.PowerOn;
            bool foodRequired = settings.nutritionUsageMultiplier > 0.001f;
            bool starving = foodRequired && (vat.NutritionStored <= 0f);
            return unpowered || starving;
        }

        public static bool IsInStasis(this Building_GrowthVat vat, IncubationSettings settings)
        {
            return vat != null && settings != null && settings.stasisProtection && vat.IsUnpoweredOrStarving(settings);
        }
    }
}
