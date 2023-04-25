public class Cocktail : Gadget
{
	public bool isPoisoned;
	public Cocktail(int usages, bool isPoisoned) : base(GadgetEnum.COCKTAIL, usages)
	{
		this.isPoisoned = isPoisoned;
	}

	//default constructor, braucht man für JSON Deserialisierung
	public Cocktail() : base() { }

	//nie aufgerufen
	public void poison()
	{
		this.isPoisoned = true;
	}
}
