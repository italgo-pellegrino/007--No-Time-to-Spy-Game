using Newtonsoft.Json;
using System;

public class Operation : BaseOperation
{/*modified*/
	public Guid characterId { get; set; }

	//successful ist hier false, da dies der Default Wert ist vom bool Wert, der Client kann nicht bestimmen, ob die Aktion erfolgreich sein wird!!
	public Operation(Guid characterId, OperationEnum type, Point target) : base(type, false, target)
	{
		this.characterId = characterId;
	}

	public Operation() : base(){
		//Default Konstruktor
	}
}
