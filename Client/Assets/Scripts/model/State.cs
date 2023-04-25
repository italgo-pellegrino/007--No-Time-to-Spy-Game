using System.Collections.Generic;

public class State
{
	public int currentRound { get; set; }
	public FieldMap map { get; set; }
	public HashSet<int> mySafeCombinations { get; set; }
	public HashSet<Character> characters { get; set; }
	public Point catCoordinates { get; set; }
	public Point janitorCoordinates { get; set; }

	public State(int currentRound, FieldMap map, HashSet<int> mySafeCombinations, HashSet<Character> characters,
		Point catCoordinates, Point janitorCoordinates)
	{
		this.currentRound = currentRound;
		this.map = map;
		this.mySafeCombinations = mySafeCombinations;
		this.characters = characters;
		this.catCoordinates = catCoordinates;
		this.janitorCoordinates = janitorCoordinates;
	}

	//default constructor, braucht man für JSON Deserialisierung
	public State() { }

    public override string ToString()
    {
		string s = "";
		s += "Current Round: " + currentRound + "\n";
		if(map == null)
        {
			s += "NO MAP";
        }
        else
        {
			s += "FIELDMAP\n" + map.ToString();
		}
		s += "CHARACTERS\n";
		foreach(Character c in characters)
        {
			s += "\t" + c.ToString() + "\n";
        }
		s += "Cat Coordinates: " + catCoordinates + "\n";
		s += "Janitor Coordinates: " + janitorCoordinates + "\n";
		return s;
    }
}
