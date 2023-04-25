using System;

public class WiretapWithEarplugs : Gadget
{
	public bool working;
	public Guid activeOn;
	public WiretapWithEarplugs(int usages, bool working) : base(GadgetEnum.WIRETAP_WITH_EARPLUGS, usages)
	{
		this.working = working;
		this.activeOn = Guid.Empty;
	}

	//default constructor, braucht man für JSON Deserialisierung
	public WiretapWithEarplugs() : base() { }

	//nie aufgerufen
	public bool activate(Guid activeOn)
	{
		if (!activeOn.Equals(Guid.Empty))
		{
			return false;
		}
		this.activeOn = activeOn;
		return true;
	}
}
