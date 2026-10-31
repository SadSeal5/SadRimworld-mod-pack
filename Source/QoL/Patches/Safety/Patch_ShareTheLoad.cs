using System;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Safety
{
    /// <summary>
    /// Note: In RimWorld 1.5+, multi-pawn concurrent resource delivery to blueprints and frames (IConstructible)
    /// is natively integrated into the base game engine via IHaulEnroute and EnrouteUtility.
    /// Intercepting ReservationManager or JobDriver_HaulToContainer breaks vanilla's UpdateEnrouteTrackers()
    /// and triggers 'stackCount = All' reservation exceptions. Native engine delivery is active and preserved.
    /// </summary>
    public static class Patch_ShareTheLoad
    {
        // Native IHaulEnroute handles multi-pawn delivery safely without reservation conflicts.
    }
}
