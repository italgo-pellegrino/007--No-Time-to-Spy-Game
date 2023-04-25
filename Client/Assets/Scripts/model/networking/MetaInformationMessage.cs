using System;
using System.Collections.Generic;
using UnityEngine;

public class MetaInformationMessage : MessageContainer
{
    public Dictionary<string, object> information;
    public MetaInformationMessage(Guid playerId, DateTime creationDate, Dictionary<string,object> information) : base(playerId, MessageTypeEnum.META_INFORMATION, creationDate)
    {
        this.information = information;
    }
}
