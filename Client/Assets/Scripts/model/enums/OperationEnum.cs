using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
[JsonConverter(typeof(StringEnumConverter))]
public enum OperationEnum
{
    GADGET_ACTION,
    SPY_ACTION,
    GAMBLE_ACTION,
    PROPERTY_ACTION,
    MOVEMENT,
    CAT_ACTION,
    JANITOR_ACTION,
    EXFILTRATION,
    RETIRE
}
