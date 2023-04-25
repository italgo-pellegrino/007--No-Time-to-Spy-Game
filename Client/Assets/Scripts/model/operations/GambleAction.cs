using Newtonsoft.Json;
using System;

public class GambleAction: Operation
{/* modified */
	public int stake { get; set; }
	public GambleAction(int stake, Guid characterId, Point target) : base(characterId, OperationEnum.GAMBLE_ACTION, target)
	{
		this.stake = stake;
	}

	//default constructor
	public GambleAction():base(){ }
}
