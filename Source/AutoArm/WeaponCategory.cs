namespace SadSeal.MegaMod.AutoArm
{
    public enum WeaponCategory
    {
        None = 0,
        RangedSingleShot = 1, // Tek atış: Tabanca < Tüfek < Sniper
        RangedBurst = 2,      // Seri atış: Otomatik tabanca < Makineli < Taarruz < Minigun/LMG
        MeleeSharp = 3,       // Kesici: Bıçak < Kısa kılıç < Uzun kılıç < Mono/Plazma
        MeleeBlunt = 4        // Ezici: Sopa < Gürz < Savaş çekici < Zeus çekici
    }
}
