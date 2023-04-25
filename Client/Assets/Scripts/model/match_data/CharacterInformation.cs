using System;
using System.Collections.Generic;

public class CharacterInformation: CharacterDescription {
    public Guid characterId;

    public CharacterInformation(string name, string description, GenderEnum gender,
        List<PropertyEnum> features, Guid characterId) : base(name,description,gender,features)
    {
        this.characterId = characterId;
    }

    public Guid getCharacterId(){
        return characterId;
    }
}
