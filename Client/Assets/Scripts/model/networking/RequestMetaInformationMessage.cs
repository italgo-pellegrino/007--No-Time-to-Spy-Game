using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

public class RequestMetaInformationMessage : MessageContainer
{
    public string[] keys;
    public RequestMetaInformationMessage(Guid playerId, string[] keys) : base(playerId, MessageTypeEnum.REQUEST_META_INFORMATION, DateTime.Now)
    {
        List<string> keylist = new List<string>();
        foreach(string s in keys)
        {
            if (keywords.Contains(s))
            {
                keylist.Add(s);
            }
        }
        if(keys.Length == 0)
        {
            this.keys = keywords;
        }
        else
        {
            this.keys = keylist.ToArray();
        }
    }

    public static readonly string[] keywords =
        new string[]
        {
            "Configuration.Scenario",
            "Configuration.Matchconfig",
            "Configuration.CharacterInformation",
            "Faction.Player1",
            "Faction.Player2",
            "Faction.Neutal",
            "Gadgets.Player1",
            "Gadgets.Player2"
        };
}
