using Newtonsoft.Json;
using System;

public class GadgetAction : Operation
{ /* modified */
	public GadgetEnum gadget { get; set; }

	//successful ist hier false, da dies der Default Wert ist vom bool Wert, der Client kann nicht bestimmen, ob die Aktion erfolgreich sein wird!!
	public GadgetAction(GadgetEnum gadget, Guid characterId, Point target) : base(characterId, OperationEnum.GADGET_ACTION, target)
	{
		this.gadget = gadget;
	}

	public GadgetAction() : base(){
		//Default Konstruktor
	}
}
