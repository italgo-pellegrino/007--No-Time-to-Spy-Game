using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
[JsonConverter(typeof(StringEnumConverter))]
public enum ErrorTypeEnum
{
    NAME_NOT_AVAILABLE,
    ALREADY_SERVING,
    SESSION_DOES_NOT_EXIST,
    ILLEGAL_MESSAGE,
    TOO_MANY_STRIKES,
    GENERAL
}
