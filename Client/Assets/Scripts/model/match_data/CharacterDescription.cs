using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterDescription
{
    public string name;
    public string description;
    public GenderEnum gender;
    public List<PropertyEnum> features;

    public CharacterDescription(string name, string description, GenderEnum gender, List<PropertyEnum> features)
    {
        this.name = name;
        this.description = description;
        this.gender = gender;
        this.features = features;
    }
    public GenderEnum GetGender()
    {
        return gender;
    }

    public string getFeatureString(string trennzeichen)
    {
        string result = "";
        for (int i = 0; i < features.Count; i++)
        {
            if (features.ElementAt(i).Equals(PropertyEnum.NIMBLENESS)) { result += "Flinkheit"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.SLUGGISHNESS)) { result += "Schwerfälligkeit"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.PONDEROUSNESS)) { result += "Behäbigkeit"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.SPRYNESS)) { result += "Behändigkeit"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.AGILITY)) { result += "Agilität"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.LUCKY_DEVIL)) { result += "Glückspilz"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.JINX)) { result += "Pechvogel"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.CLAMMY_CLOTHES)) { result += "Klamme Klamotten"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.CONSTANT_CLAMMY_CLOTHES)) { result += "Konstant Klamme Klamotten"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.ROBUST_STOMACH)) { result += "Robuster Magen"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.TOUGHNESS)) { result += "Zähigkeit"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.BABYSITTER)) { result += "Babysitter"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.HONEY_TRAP)) { result += "Honey Trap"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.BANG_AND_BURN)) { result += "Bang and Burn"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.FLAPS_AND_SEALS)) { result += "Flaps and Seals"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.TRADECRAFT)) { result += "Tradecraft"; }
            else if (features.ElementAt(i).Equals(PropertyEnum.OBSERVATION)) { result += "Observation"; }

            if ((i + 1) < features.Count)
                result += trennzeichen;
        }

        return result;
    }
}

