
    /**
        Diese Enum Liste dient dazu, die ausgewählte Option im Drop Down zu verwerten
    **/
    public enum DropDownOption {
        //Für Fehlerbehandlung
        None,

        //Bewegung auf dem Spielfeld wegen Input Konflikt
        Movement,
        //Cocktail Trinken
        Drinking_Cocktail,
        //Cocktail verschütten
        Spill_Cocktail,
        //Cocktail vom Tisch aufheben
        Collect_Cocktail,
        //Roulette spielen ??
        Play_Roulette,
        //Ausspionieren
        Spy_People,
        //in den Tresor spicken 
        Open_Tresor,
        // alle Gadgets ....
        HAIRDRYER,
        MOLEDIE,
        TECHNICOLOUR_PRISM,
        BOWLER_BLADE,
        MAGNETIC_WATCH,
        POISON_PILLS,
        LASER_COMPACT,
        ROCKET_PEN,
        GAS_GLOSS,
        MOTHBALL_POUCH,
        FOG_TIN,
        GRAPPLE,
        WIRETAP_WITH_EARPLUGS,
        DIAMOND_COLLAR,
        JETPACK,
        CHICKEN_FEED,
        NUGGET,
        MIRROR_OF_WILDERNESS,
        POCKET_LITTER,
        //*** Aktionen für Cocktail siehe oben, es wurde präzisiert ***//

        //COCKTAIL

        //Aktionen, die durch Feautures ermöglicht werden
        Observation,
        Bang_and_Burn,

        //Falls man Agenteninformationen eines eigenen Agenten angezeigt haben möchte ... 
        show_Agent_Information,
        
        //Falls der Spieler die Pause Einstellung beeinflussen möchte ... 
        Pause_Setting,

        //Falls der Spieler seinen Spielzug frühzeitig beenden möchte ...
        Retire

}