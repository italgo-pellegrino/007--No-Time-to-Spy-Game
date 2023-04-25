using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum RoleEnum
{
    SPECTATOR,
    PLAYER,
    AI
}
