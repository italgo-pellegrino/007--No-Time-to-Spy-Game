using System;
using UnityEngine;
using System.Collections.Generic;


public class RequestItemChoiceMessage : MessageContainer
{
    public List<Guid> offeredCharacterIds;
    public List<GadgetEnum> offeredGadgets;


    public RequestItemChoiceMessage(Guid playerId, DateTime creationDate,
        List<Guid> offeredCharacterIds,List<GadgetEnum> offeredGadgets) : base(playerId, MessageTypeEnum.REQUEST_ITEM_CHOICE, creationDate)
    {
        this.offeredCharacterIds = offeredCharacterIds;
        this.offeredGadgets = offeredGadgets;
    }
}
