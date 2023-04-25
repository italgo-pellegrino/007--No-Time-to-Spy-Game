using System;

public class ItemChoiceMessage : MessageContainer
{
	public Guid? chosenCharacterId;
	public GadgetEnum? chosenGadget;
	public ItemChoiceMessage(Guid playerId, DateTime creationDate, Guid? chosenCharacter, GadgetEnum? chosenGadget) : base(playerId, MessageTypeEnum.ITEM_CHOICE, creationDate)
	{
		this.chosenCharacterId = chosenCharacter;
		this.chosenGadget = chosenGadget;
	}
}
