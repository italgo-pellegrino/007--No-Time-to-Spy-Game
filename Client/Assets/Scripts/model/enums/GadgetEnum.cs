using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum GadgetEnum
{
    //Es wurde ein weiterer Wert eines Enums eingeführt, um mit Fehlersituationen umzugehen
    None,
    HAIRDRYER,
    MOLEDIE,
    TECHNICOLOUR_PRISM,
    BOWLER_BLADE,
    MAGNETIC_WATCH,
    POISON_PILLS,
    LASER_COMPACT,
    ROCKET_PEN,
    GAS_GLOSS,
    MOTHBALL_POUCH,
    FOG_TIN,
    GRAPPLE,
    WIRETAP_WITH_EARPLUGS,
    DIAMOND_COLLAR,
    JETPACK,
    CHICKEN_FEED,
    NUGGET,
    MIRROR_OF_WILDERNESS,
    POCKET_LITTER,
    ANTI_PLAGUE_MASK,
    COCKTAIL
}