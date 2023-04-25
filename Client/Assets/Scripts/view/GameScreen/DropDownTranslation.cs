/**
    Diese Klasse stellt ein enum bereit, womit dessen der Spielzug validiert wird
    Die Werte werden im Drop Down in englischer Sprache angezeigt.
    Bemerkung: Die Methoden sind static, vlt sollte diese Klasse instanziiert werden und die static Eigenschaft der Methoden entfernt werden
**/
public class DropDownTranslation
{ /* modified */

    /**
        Diese Methode übersetzt die Gagdets in Deutsche,
        diese deutsche Übersetzung dient der Einblendung im DropDown Menü,
        als Hilfsmethode ist sie static definiert

        An Atakan: Diese Methode soll benutzt werden um das Drop Down mit Opionen zu füllen
    **/
    public static string translate(DropDownOption option)
    {
        switch (option)
        {
            case DropDownOption.HAIRDRYER:
                return "Akku-Föhn";
            case DropDownOption.MOLEDIE:
                return "Maulwürfel";
            case DropDownOption.TECHNICOLOUR_PRISM:
                return "Technicolor-Prisma";
            case DropDownOption.BOWLER_BLADE:
                return "Klingen-Hut";
            //Dieses Gadget hat keine aktive Wirkung !!!
            case DropDownOption.MAGNETIC_WATCH:
                return "Magnetfeld-Armbanduhr";
            case DropDownOption.POISON_PILLS:
                return "Giftpillen-Flasche";
            case DropDownOption.LASER_COMPACT:
                return "Laser-Puderdose";
            case DropDownOption.ROCKET_PEN:
                return "Raketenwerfer-Füllfederhalter";
            case DropDownOption.GAS_GLOSS:
                return "Gaspatronen-Lippenstift";
            case DropDownOption.MOTHBALL_POUCH:
                return "Mottenkugel-Beutel";
            case DropDownOption.FOG_TIN:
                return "Nebeldose";
            case DropDownOption.GRAPPLE:
                return "Wurfhaken";
            case DropDownOption.WIRETAP_WITH_EARPLUGS:
                return "Wanze und Ohrstöpsel";
            //Keine aktive Wirkung !!!
            case DropDownOption.DIAMOND_COLLAR:
                return "Diamanthalsband";
            case DropDownOption.JETPACK:
                return "Jetpack";
            case DropDownOption.CHICKEN_FEED:
                return "Chicken Feed";
            case DropDownOption.NUGGET:
                return "Nugget";
            case DropDownOption.MIRROR_OF_WILDERNESS:
                return "Mirror of Wilderness";
            //Keine aktive Wirkung!!!
            case DropDownOption.POCKET_LITTER:
                return "Pocket Litter";
            case DropDownOption.Movement:
                return "Agenten bewegen";
            case DropDownOption.Drinking_Cocktail:
                return "Cocktail trinken";
            case DropDownOption.Collect_Cocktail:
                return "Cocktail aufheben";
            case DropDownOption.Spill_Cocktail:
                return "Cocktail verschütten";
            case DropDownOption.Bang_and_Burn:
                return "Bang and Burn";
            case DropDownOption.Observation:
                return "Observation";
            case DropDownOption.Open_Tresor:
                return "Tresor öffnen";
            case DropDownOption.Spy_People:
                return "Agente ausspionieren";
            case DropDownOption.Play_Roulette:
                return "Roulette spielen";
            case DropDownOption.show_Agent_Information:
                return "Agenteninformationen anderer Fraktionsangehöriger anzeigen";
            //Pause Einstellung
            case DropDownOption.Pause_Setting:
                return "Pause-Einstellung";
            case DropDownOption.Retire:
                return "Spielzug frühzeitig beenden";
        }

        //bei Fehler
        return null;
    }


    /**
        Diese Methode ermöglicht das Übersetzen des Strings im Drop Down in eine Drop Down-Option
     **/
    public static DropDownOption getDropDownOptionByString(string DropDownOptionstring)
    {
        switch (DropDownOptionstring)
        {
            case "Akku-Föhn":
                return DropDownOption.HAIRDRYER;
            case "Maulwürfel":
                return DropDownOption.MOLEDIE;
            case "Technicolor-Prisma":
                return DropDownOption.TECHNICOLOUR_PRISM;
            case "Klingen-Hut":
                return DropDownOption.BOWLER_BLADE;
            case "Magnetfeld-Armbanduhr":
            //return DropDownOption.MAGNETIC_WATCH;
            case "Giftpillen-Flasche":
                return DropDownOption.POISON_PILLS;
            case "Laser-Puderdose":
                return DropDownOption.LASER_COMPACT;
            case "Raketenwerfer-Füllfederhalter":
                return DropDownOption.ROCKET_PEN;
            case "Gaspatronen-Lippenstift":
                return DropDownOption.GAS_GLOSS;
            case "Mottenkugel-Beutel":
                return DropDownOption.MOTHBALL_POUCH;
            case "Nebeldose":
                return DropDownOption.FOG_TIN;
            case "Wurfhaken":
                return DropDownOption.GRAPPLE;
            case "Wanze und Ohrstöpsel":
                return DropDownOption.WIRETAP_WITH_EARPLUGS;
            case "Diamanthalsband":
                return DropDownOption.DIAMOND_COLLAR;
            case "Jetpack":
                return DropDownOption.JETPACK;
            case "Chicken Feed":
                return DropDownOption.CHICKEN_FEED;
            case "Nugget":
                return DropDownOption.NUGGET;
            case "Mirror of Wilderness":
                return DropDownOption.MIRROR_OF_WILDERNESS;
            case "Pocket Litter":
                return DropDownOption.POCKET_LITTER;
            case "Agenten bewegen":
                return DropDownOption.Movement;
            case "Cocktail trinken":
                return DropDownOption.Drinking_Cocktail;
            case "Cocktail aufheben":
                return DropDownOption.Collect_Cocktail;
            case "Cocktail verschütten":
                return DropDownOption.Spill_Cocktail;
            case "Bang and Burn":
                return DropDownOption.Bang_and_Burn;
            case "Observation":
                return DropDownOption.Observation;
            case "Tresor öffnen":
                return DropDownOption.Open_Tresor;
            case "Agente ausspionieren":
                return DropDownOption.Spy_People;
            case "Roulette spielen":
                return DropDownOption.Play_Roulette;
            case "Agenteninformationen anderer Fraktionsangehöriger anzeigen":
                return DropDownOption.show_Agent_Information;
            case "Pause-Einstellung":
                return DropDownOption.Pause_Setting;
            case "Spielzug frühzeitig beenden":
                return DropDownOption.Retire;
        }

        //bei Fehler
        //try catch auch möglich
        return DropDownOption.None;
    }

}