using System;

public class Field
{
	public FieldStateEnum state { get; set; }
	public Gadget gadget { get; set; }

	public bool isDestroyed { get; set; }
	public bool isInverted { get; set; }
	public int chipAmount { get; set; }

	public int safeIndex { get; set; } // > 0 => min 1

	public bool isFoggy { get; set; }
	public bool isUpdated { get; set; }
	public Field(FieldStateEnum state, Gadget gadget, bool isDestroyed, bool isInverted,
		int chipAmount, int safeIndex, bool isFoggy, bool isUpdated)
	{
		this.state = state;
		this.gadget = gadget;
		this.isDestroyed = isDestroyed;
		this.isInverted = isInverted;
		this.chipAmount = chipAmount;
		this.safeIndex = safeIndex;
		this.isFoggy = isFoggy;
		this.isUpdated = isUpdated;
	}

	//default constructor
	public Field() { }

	public FieldStateEnum getState()
	{
		return state;
	}
	public int GetChipAmount(){
		return chipAmount;
	}

	public FieldStateEnum getFieldStateEnum(){
		return state;
	}

	public bool getIsDestroyed(){
		return isDestroyed;
	}

	public bool getIsInverted()
    {
		return isInverted;
    }

	public Gadget GetGadget(){
		return gadget;
	}

	public int getSafeIndex(){
		return safeIndex;
	}

	public bool getIsFoggy()
	{
		return isFoggy;
	}

	/**
	* Method to check if the field is of the given type.
	*
	* @param state The state of the field.
	* @return true if the given field is of the given type, false if not.
	*/
	public Boolean isState(FieldStateEnum state)
	{
		return this.state.Equals(state);
	}

}
