using Newtonsoft.Json;
using System;
using UnityEngine;

public class Movement : Operation
{/* modified */
	public Point from { get; set; }
	// Zielfeld: Operation.target
	public Movement(Point from, Guid characterId, Point target) : base(characterId, OperationEnum.MOVEMENT, target)
	{
		this.from = from;
	}

	//default constructor
	public Movement() : base() {}
}
