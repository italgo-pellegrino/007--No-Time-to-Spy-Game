using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
[JsonConverter(typeof(StringEnumConverter))]
public enum FieldStateEnum
{
    BAR_TABLE,
    ROULETTE_TABLE,
    WALL,
    FREE,
    BAR_SEAT,
    SAFE,
    FIREPLACE
}