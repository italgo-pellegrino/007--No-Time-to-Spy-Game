using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
[JsonConverter(typeof(StringEnumConverter))]
public enum PropertyEnum
{
    NIMBLENESS,
    SLUGGISHNESS,
    PONDEROUSNESS,
    SPRYNESS,
    AGILITY,
    LUCKY_DEVIL,
    JINX,
    CLAMMY_CLOTHES,
    CONSTANT_CLAMMY_CLOTHES,
    ROBUST_STOMACH,
    TOUGHNESS,
    BABYSITTER,
    HONEY_TRAP,
    BANG_AND_BURN,
    FLAPS_AND_SEALS,
    TRADECRAFT,
    OBSERVATION
}
