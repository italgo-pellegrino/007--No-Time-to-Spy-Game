using System.Collections;
using System.Collections.Generic;
using System;

public class EquipmentChoiceMessage : MessageContainer
{
    public Dictionary<Guid,HashSet<GadgetEnum>> equipment;

    public EquipmentChoiceMessage(Guid playerId, DateTime creationDate,Dictionary<Guid,HashSet<GadgetEnum>> equipment) : base(playerId, MessageTypeEnum.EQUIPMENT_CHOICE, creationDate)
    {
        this.equipment = equipment;
    }
}
