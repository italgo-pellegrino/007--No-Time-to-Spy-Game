using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
[JsonConverter(typeof(StringEnumConverter))]
public enum GenderEnum {
//eigene Konstante, gibt an, dass das Geschlecht unbekannt ist
None,
MALE,
FEMALE,
DIVERSE
}
