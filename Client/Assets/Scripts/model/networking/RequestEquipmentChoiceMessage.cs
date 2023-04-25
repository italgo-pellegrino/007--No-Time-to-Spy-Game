using System;
using UnityEngine;
using System.Collections.Generic;


public class RequestEquipmentChoiceMessage : MessageContainer
{
    public List<Guid> chosenCharacterIds;
    public List<GadgetEnum> chosenGadgets;


    public RequestEquipmentChoiceMessage(Guid playerId, DateTime creationDate,
        List<Guid> chosenCharacterIds, List<GadgetEnum> chosenGadgets) : base(playerId, MessageTypeEnum.REQUEST_EQUIPMENT_CHOICE, creationDate)
    {
        this.chosenCharacterIds = chosenCharacterIds;
        this.chosenGadgets = chosenGadgets;
    }
}
