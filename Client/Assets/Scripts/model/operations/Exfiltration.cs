using Newtonsoft.Json;
using System;
using UnityEngine;

public class Exfiltration : Operation
{
	public Point from { get; set; }
	public Exfiltration(Guid characterId, Point target, Point from) : base(characterId, OperationEnum.EXFILTRATION, target)
	{
		this.from = from;
	}

	//default constructor
	public Exfiltration() : base() { }
}
