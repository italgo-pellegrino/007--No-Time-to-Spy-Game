using Newtonsoft.Json;
using System;

public class PropertyAction : Operation
{/* modified */
	public PropertyEnum usedProperty { get; set; }
	public bool isEnemy { get; set; }

	/**
		Konstruktor zum Senden, daher isEnemy = false, da der Client nicht von vornherein entscheiden kann, ob der Gegner ein Feind ist
	**/
	public PropertyAction(PropertyEnum usedProperty, Guid characterId, Point target) : base(characterId, OperationEnum.PROPERTY_ACTION, target)
	{
		this.usedProperty = usedProperty;
		this.isEnemy = false;
	}

	public PropertyAction():base()
	{
		//Default Konstruktor
	}
}
