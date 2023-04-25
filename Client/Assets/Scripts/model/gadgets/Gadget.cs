public class Gadget
{
	public GadgetEnum gadget;
	public int usages;
	public Gadget(GadgetEnum gadget, int usages)
	{
		this.gadget = gadget;
		this.usages = usages;
	}

	//default constructor
	public Gadget() { }

	// nie aufgerufen
	public bool use()
	{
		if (usages <= 0)
		{
			return false;
		}
		usages--;
		return true;
	}
}
