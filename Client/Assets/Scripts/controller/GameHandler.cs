//HashSet
using System;
//Guid
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler
{
    /*
     * spieler id
     */
    public Guid playerId;

    /**
     * partie config
     */
    public Guid sessionId;
    public Scenario level;
    public Matchconfig settings;
    /*
     * Dieses Array enthält alle (redundanten) Informationen zu den Charakteren,
     * Erhalt bei HelloReply Nachricht,
     * wichtig zur Abfrage des Geschlechts
     */
    public CharacterInformation[] characterSettings;

    /**
     * Grösse des Spielfeldes -> x-Zeilen (y-Koordinate im Spiel) , y-Spalten (x Koordinate im Spiel)
     */
    public Point mapSize;
    public Guid p1id; // nur um Zuschauer spaeter mitzuteilen wer gewonnen hatte
    private bool userIsPlayerOne, userIsPlayerTwo; // wichtig beim Zuschauerscreen

    /**
     * zeigt an, welcher Character momentan am Zug ist, s. RequestGameOperationMessage - muss geupdated werden
     */
    public Guid activeCharacterGuid;

    //zeigt an wie vielte Runde (spielt keine wichtige Rolle, gut beim Testen)
    private int currentRound;

    /**
    s. State - Objekt: Liste aller Charaktere auf dem Spielfeld, muss durch GameStatus Nachrichten geupdatet werden
    **/
    private HashSet<Character> characters = new HashSet<Character>();

    /** 
        Geheimnisse, die ein Client/Spieler kennt
    **/
    public HashSet<int> mySafeCombinations { get; set; }

    //FieldMap des Spiels, das den globalen Zustand des Spiels enthält, gemaß Standard
    //Zum Aufbauen der Map sollte man sich an dieses Attribut richten!!! -> buildMap Methode umschreiben !!!
    private FieldMap map;
    //logische Koordinaten von Katze und Hausmeister
    public Point catCoordinates, janitorCoordinates;

    public bool isPaused; // pause flag -> hilft gamescreen
    public bool isMyTurn; // Flag ob ich am Zug bein -> hilft gamescreen

    // flag -> gibt an ob charaktere und gadgets zufaellig gewaehlt werden
    private bool random_mode; // because why not



    //Konstruktor, we kinda need it
    public GameHandler()
    {
    }

    /**
     * GamesScreen Attribut, damit wir nicht alles statisch machen
     * zum Abruf der View Funktionen
     */
    public GameScreen gamescreen;

    /**
     * Getters/Setters
     */
    public void setGameScreen(GameScreen s)
    {
        gamescreen = s;
    }

    public void setRandomMode(bool random)
    {
        random_mode = random;
    }
    public Guid getPlayerId()
    {
        return playerId;
    }

    /**
     * ANKOMMENDE NACHRICHTEN
     * @param meistens die aus JSON geparste Nachricht Objekte
     */
    /**
     * onHelloReply
     * legt client id im gamehandler fest
     * laedt PartieConfig
     */
    public void onHelloReply(HelloReplyMessage message)
    {
        Debug.Log("Processing Hello Reply...");
        this.playerId = message.clientId;
        this.sessionId = message.sessionId;
        this.level = message.level;
        //set mapsize as Point
        this.mapSize = new Point(this.level.getScenario().GetLength(0), this.level.getScenario().GetLength(1));
        this.settings = message.settings;
        this.characterSettings = message.characterSettings;
        // set list of characters
        setCharacterList();
    }
    public Scenario getLevel()
    {
        return level;
    }

    /**
     * Belegt die Liste von Charakteren, die im Partie
     * sein sollten
     */
    public void setCharacterList()
    {
        foreach(CharacterInformation info in characterSettings)
        {
            var properties = new HashSet<PropertyEnum>(info.features);
            Character ch = new Character(info.characterId, info.name, properties);
            characters.Add(ch);
        }
    }

    /**
     * onGameStarted
     * stellt fest ob es um Spieler oder Zuschauer handelt
     * Szenenwechsel in
     *   Wahlphase (falls Spieler, nicht Zufallsmodus)
     *   GameScene (Spieler-Zufallsmodus oder Zuschauer)
     */
    public void onGameStarted(GameStartedMessage message)
    {
        Debug.Log("Processing Game Started...");
        p1id = message.playerOneId;
        userIsPlayerOne = playerId.Equals(message.playerOneId);
        userIsPlayerTwo = playerId.Equals(message.playerTwoId);
        if (SceneManager.GetActiveScene().name.Equals("MenuScene"))
        {
            if (userIsPlayerOne || userIsPlayerTwo)
            {
                MenuEvents.gameHasStartedForPlayer(random_mode);
            }
            else
            {
                MenuEvents.gameHasStartedForSpectator();
            }
        }
    }

    /**
     * WAHLPHASE & AUSRUSTUNGSPHASE
     */
    /**
        Verweis wird im Editor eingestellt
    **/
    public GameObject cardManager;

    /**
     * onRequestItemChoice - Wahlphasenfunktionen
     * s. CardManagerScript
     * falls Zufallsmodus waehlt zufaellig ein Item aus und
     * sendet Nachricht zum Server
     */
    public void onRequestItemChoice(RequestItemChoiceMessage message)
    {
        Debug.Log("Processing Request Item Choice...");

        if (random_mode) //Zufallsmodus
        {
            //Char Liste
            List<Guid> characters = message.offeredCharacterIds;
            List<GadgetEnum> gadgets = message.offeredGadgets;
            //Gadget Liste
            Guid[] charactersArray = characters.ToArray();
            GadgetEnum[] gadgetsArray = gadgets.ToArray();

            //Wahl passiert in chooseItem Funktion
            if (charactersArray.Length == 0)
            {
                chooseItem(charactersArray, gadgetsArray, false);
            }
            else if (gadgetsArray.Length == 0)
            {
                chooseItem(charactersArray, gadgetsArray, true);
            }
            else
            {
                System.Random r = new System.Random();
                int a = r.Next(1, 3); //generates no between 1 and 2
                chooseItem(charactersArray, gadgetsArray, a % 2 == 0);
            }
        }
        else
        {
            //Gibt die Daten die vom Server kommen, dem CardManager weiter.
            CardManagerScript.charactersArray = message.offeredCharacterIds.ToArray();
            CardManagerScript.gadgetsArray = message.offeredGadgets.ToArray();

            //Das Zeichen für den CardManager das eine Neue ItemChoice oder EquipmentChoice  Nachricht angekommen ist
            CardManagerScript.eventComing = true;
        }
    }

    /**
     * Waehlt im Zufallsmodus eine Karte aus
     * @args bool choosingChar: gibt an ob ich char waehle oder Gadget
     * wird zufaellig im onRequestItemChoice gesetzt
     */
    private void chooseItem(Guid[] chars, GadgetEnum[] gadgets, bool choosingChar)
    {
        System.Random r = new System.Random();
        if (choosingChar)
        {
            sendItemChoiceMessage(new ItemChoiceMessage(playerId, DateTime.Now,
                chars[r.Next(0, chars.Length)], null));
        }
        else
        {
            sendItemChoiceMessage(new ItemChoiceMessage(playerId, DateTime.Now,
                null, gadgets[r.Next(0, gadgets.Length)]));
        }
    }

    /**
     * Senden vom ausgewaehlten Item
     */
    public void sendItemChoiceMessage(ItemChoiceMessage itemChoice)
    {
        Debug.Log("Sending item choice...");
        Connection.SendWebSocketMessage(itemChoice);
    }

    /**
     * onRequestEquipmentChoice
     * s. CardManagerScript
     * die ausgewaehlten Agenten hier zum Fraktion eingefügt
     * falls Zufallsmodus: Gadgets nacheinander zu Charaktere gegeben
     */
    public void onRequestEquipmentChoice(RequestEquipmentChoiceMessage message)
    {
        Debug.Log("Processing Request Equipment Choice...");
        if (random_mode)
        {
            List<Guid> characters = message.chosenCharacterIds;
            List<GadgetEnum> gadgets = message.chosenGadgets;

            //Charaktere zum Fraktion einfügen
            Guid[] charactersArray = characters.ToArray();
            addToFraction(charactersArray);
            GadgetEnum[] gadgetsArray = gadgets.ToArray();

            //Dictionary zur Abgabe (Senden)
            Dictionary<Guid, HashSet<GadgetEnum>> d = new Dictionary<Guid, HashSet<GadgetEnum>>();
            int cl = charactersArray.Length;
            //Gadget Set jedes Charakters
            HashSet<GadgetEnum>[] hss = new HashSet<GadgetEnum>[cl];
            for (int i = 0; i < cl; i++)
            {
                //initialiseren von Gadget Sets
                hss[i] = new HashSet<GadgetEnum>();
            }

            for (int i = 0; i < gadgetsArray.Length; i++)
            {
                //Gadgets nacheinander zum naechten Char geben
                hss[i % cl].Add(gadgetsArray[i]);
            }

            for (int i = 0; i < cl; i++)
            {
                d.Add(charactersArray[i], hss[i]);
            }

            //senden
            EquipmentChoiceMessage m = new EquipmentChoiceMessage(playerId, DateTime.Now, d);
            sendEquipmentChoiceMessage(m);
        }
        else
        {
            //Charaktere zum Fraktion einfügen
            List<Guid> characters = message.chosenCharacterIds;
            Guid[] charactersArray = characters.ToArray();
            addToFraction(charactersArray);

            //Gibt die Daten die vom Server kommen, dem CardManager weiter. 
            CardManagerScript.charactersArray = message.chosenCharacterIds.ToArray();
            CardManagerScript.gadgetsArray = message.chosenGadgets.ToArray();

            //Das Zeichen für den CardManager das eine Neue ItemChoice oder EquipmentChoice Nachricht angekommen ist
            CardManagerScript.eventComing = true;
        }
    }

    /**
     * Senden von EquipmentChoice
     * random_mode hier zurückgesetzt (fürs naechste Mal)
     */
    public void sendEquipmentChoiceMessage(EquipmentChoiceMessage equipmentChoice)
    {
        Debug.Log("Sending equipment choice...");
        Connection.SendWebSocketMessage(equipmentChoice);
        random_mode = false;
    }

    /**
     * onGameStatus
     * Behandlung aktueller Spielzustandes
     * erst werden logisch Variablen gesetzt
     * dann wird animiert
     * zuletzt wird Zustand im View aktualisiert
     * (falls was beim Animationen schiefgehen)
     */
    public void onGameStatus(GameStatusMessage message)
    {
        Debug.Log("Processing Game Status..");
        //logische GameHandler Variablen
        if(message.activeCharacterId != null)
        {
            setActiveCharacter((Guid) message.activeCharacterId);
        }

        // falls User ist Zuschauer, aendere GameScreen entsprechend
        // wird nur im ersten GameStatus Nachricht gemacht
        // Frage nach Fraktionen der Spieler
        if(!userIsPlayerOne && !userIsPlayerTwo && !gamescreen.isSpectatorScreen) //Erste game status Nachricht an Zuschauer
        {
            sendMetaInfoRequest(new string[] { RequestMetaInformationMessage.keywords[3], RequestMetaInformationMessage.keywords[4] });
            // beim ersten GameStatus Message Animationen nicht spielen nur Zustand anzeigen
            message.operations = new List<BaseOperation>();
            gamescreen.spectatorScreen();
        }

        State state = message.state;
        currentRound = state.currentRound;
        map = state.map;
        mySafeCombinations = state.mySafeCombinations;
        characters = state.characters;
        catCoordinates = state.catCoordinates;

        //Animieren
        animateOperations(message);
    }

    /** 
     * Setter für activeCharacter
     */
    public void setActiveCharacter(Guid guid)
    {
        activeCharacterGuid = guid;
    }

    /** 
     * Getter für activeCharacter, um Informationen auszulesen,
     * Enuerator iteriert durch die Datenstruktur
     * gibt das Charakter Objekt des aktiven Charakters zurück
     */
    public Character getActiveCharacter()
    {
        HashSet<Character>.Enumerator enu = characters.GetEnumerator();
        while (enu.MoveNext())
        {
            Character ch = enu.Current;
            if (ch.getGuid().Equals(activeCharacterGuid)) return ch;
        }
        //NULL zeigt einen Fehler an!
        return null;
    }

    /**
     * Gibt der Charakter mit dem übergebenen ID zurück
     * Enumerator iteriert durch die Datenstruktur
     */
    public Character getCharacterById(Guid charId)
    {
        HashSet<Character>.Enumerator enu = characters.GetEnumerator();
        while (enu.MoveNext())
        {
            Character ch = enu.Current;
            if (ch.getGuid().Equals(charId)) return ch;
        }
        //NULL zeigt einen Fehler an!
        return null;
    }

    /**
     * Gibt der Charakter mit dem übergebenen Name zurück
     * Enumerator iteriert durch die Datenstruktur
     */
    public Character getCharacterByName(string charName)
    {
        HashSet<Character>.Enumerator enu = characters.GetEnumerator();
        while (enu.MoveNext())
        {
            Character ch = enu.Current;
            if (ch.getName().Equals(charName)) return ch;
        }
        //NULL zeigt einen Fehler an!
        return null;
    }

    /**
     * Gibt der Charakter auf dem übergebenen Position zurück
     * Enumerator iteriert durch die Datenstruktur
     * (könnte viel einfacher implementiert werden (mit toList unf ElementAt
     * aber egal wird sowieso nie aufgerufen)
     */
    public Character getCharacterByIndex(int charIndex)
    {
        int count = 0;
        HashSet<Character>.Enumerator enu = characters.GetEnumerator();
        while (enu.MoveNext())
        {
            if(count == charIndex)
            {
                return enu.Current;
            }
            count++;
        }
        //NULL zeigt einen Fehler an!
        return null;
    }

    /**
     * Gibt an ob der übergebene Charakter Objekt,
     * das vom aktiven Charakters ist
     */
    public bool isActiveCharacter(Character c)
    {
        return c.getGuid().Equals(activeCharacterGuid);
    }

    /**
    Diese Liste verwaltet alle UUIDs der Charaktere in der eigenen Fraktion,
    Auffüllung geschieht durch die Nachrichten in der Wahl - und Ausrüstungsphase,
    bei Übergabe des Nuggets ??? - Standard
    **/
    private List<Guid> fraction = new List<Guid>();

    public List<Guid> getFraction()
    {
        return fraction;
    }

    /**
     * Füge neue ID in Fraktion ein
     */
    public void addToFraction(Guid newchar)
    {
        fraction.Add(newchar);
    }


    /**
     * Füge mehrere neue IDs in Fraktion ein
     */
    public void addToFraction(Guid[] newchars)
    {
        foreach(Guid g in newchars)
        {
            fraction.Add(g);
        }
    }

    /**
     * List der Agenten IDs der gegnerischen Fraktion
     * wird nur aufgefüllt bei erfolgreichem Observation Aktion
     * oder falls ein NPC dich ausspioniert
     */
    private List<Guid> EnemyFraction = new List<Guid>();

    public List<Guid> getEnemyFraction()
    {
        return EnemyFraction;
    }

    /**
     * Füge neue ID in das gegnerische Fraktion ein
     */
    public void addToEnemyFraction(Guid newchar)
    {
        EnemyFraction.Add(newchar);
    }

    /**
     * Gibt an ob der Agenten mit dem übergebenen ID
     * im gegnerischen Fraktion ist
     */
    public bool IsEnemyFractionMember(Guid agent)
    {
        return EnemyFraction.Contains(agent);
    }

    /**
     * Alle restliche Charaktere müssen im Gegner Fraktion sein
     * Wird aufgerufen nachdem Hausmeister ankommt
     */
    public void autoFillEnemyFraction()
    {
        HashSet<Character>.Enumerator enu = characters.GetEnumerator();
        while (enu.MoveNext())
        {
            Character c = enu.Current;
            if(!IsOwnFractionMember(c) && !IsEnemyFractionMember(c.getGuid()))
            {
                EnemyFraction.Add(c.getGuid());
                gamescreen.markAgent(c.getGuid(), true);
            }
        }
    }

    /**
     * allgemeine Animationsfunktion im GameScreen wird hier aufgerufen
     * Da wir auf Animationen warten möchten, ist der Rückgabetyp der
     * Funktion im GameScreen ein IEnumerator
     * d.h. sie muss mit StartCoroutine aufgerufen werden
     */
    private void animateOperations(GameStatusMessage message)
    {
        gamescreen.StartCoroutine(gamescreen.animate(message));
    }

    /**
     * Nachdem alle Animationen in der Liste durch sind
     * wird die View so aktualisiert dass sie mit dem logischen
     * Spielzustand übereinstimmt
     */
    public void updateState(GameStatusMessage message)
    {
        //Felder aktualisieren (falls Gadgets rumlegen,
        //Cocktails weg oder Waende zerstört sind,...)
        gamescreen.updateMap(map.getMap());
        State state = message.state; //Spielzustand wieder laden, da Existenz des Hausmeister nicht fest

        //Charakter GameObjekte aktualisieren (auf richtigen Stellen bewegen, falls sie nach Animationen
        //falsch rumliegen oder Sprite aendern falls sie Cocktail haben,...)
        //gefaehrliche Funktion, weil falls mehrere Fehler vorliegen Zustandsupdate wird nicht alles korrigieren
        if (state.janitorCoordinates == null) // falls kein Hausmeister
        {
            gamescreen.updateChars(this.characters,
                Point.point_to_vector3int(this.catCoordinates),
                null);
        }
        else // falls Hausmeister (overtime)
        {
            janitorCoordinates = state.janitorCoordinates;
            autoFillEnemyFraction();
            gamescreen.updateChars(this.characters,
                Point.point_to_vector3int(this.catCoordinates),
                Point.point_to_vector3int(this.janitorCoordinates));
        }

        // default state vom Agenteninfobox (links im View) aktualisieren
        if (!activeCharacterGuid.Equals(Guid.Empty))
        {
            Character c = getActiveCharacter();
            if (c != null)
            {
                gamescreen.fill_AgentInfo(c.getName(), GetGender(c), c.getChips(),
                    mySafeCombinations, c.getProperties(), c.GetGadgets(),
                    c.getHp(), c.getIp(), c.getAp(), c.getMp());
            }
        }

        // initially last functions of ongamestatus
        gamescreen.setTransparency(map.getMap().GetLength(0), map.getMap().GetLength(1));

        // aktuelle Runde anzeigen
        gamescreen.updateRoundLabel(currentRound);
    }

    //Getter FieldMap
    public FieldMap getFieldMap()
    {
        return map;
    }

    /**
     * onRequestGameOperation
     * GameScreen wird so vorbereitet, dass Benutzer ein Aktion ausführen darf
     */
    public void onRequestGameOperation(RequestGameOperationMessage message)
    {
        Debug.Log("Processing Game Operation Request...");
        isMyTurn = true; //Beim Senden eines Messages oder Strike wieder zurückgesetzt
        //aktive Char aktualisieren
        setActiveCharacter(message.characterId);
        Character active_char = getActiveCharacter();
        //default Zustand des Charakterinfoboxes (links im View) wird aktualisiert
        gamescreen.fill_AgentInfo(active_char.getName(), GetGender(active_char), active_char.getChips(),
            mySafeCombinations, active_char.getProperties(), active_char.GetGadgets(),
            active_char.getHp(), active_char.getIp(), active_char.getAp(), active_char.getMp());

        //Falls Charakter im Nebel ist, AP auf 0 setzen (er darf keine Aktion machen)
        if (map.getField(active_char.getCoordinates()).getIsFoggy())
        {
            active_char.setAp(0);
        }

        //GameScreen Dropdown (wie man Aktionen ausführt) wird aktiviert (Optionen nach Eigenschaften des Charakters)
        gamescreen.resetDropdownChoice();
        gamescreen.activateDropdown(true);
        gamescreen.fillDropdown(active_char);
        //View Bemerkungen einschalten (man sieht somit dass er dran ist und der aktive GameObject)
        gamescreen.toggleTurnInfo(true);
        gamescreen.markActiveChar(activeCharacterGuid, true);
    }

    /**
     * onStatistics
     * Stats Canvas im View anzeigen (falls man im View ist)
     * sonst auf Hauptmenü
     * dabei variablen entsprechend gelesenen Werten setzen
     */
    public void onStatistics(StatisticsMessage message)
    {
        Debug.Log("Processing statistics message...");
        bool winner = message.winner.Equals(playerId);
        if (SceneManager.GetActiveScene().name.Equals("GameScene"))
        {
            //Stats Canvas anzeigen
            gamescreen.showStats(winner, message.reason, message.statistics.entries, userIsPlayerOne, userIsPlayerTwo, message.winner.Equals(p1id));
        }
        else
        {
            SceneManager.LoadScene("Scenes/MenuScene");
        }
    }

    /**
     * onGameLeft
     * ich glaub alle Funktionen die hier sein könnten, sind
     * schon woanders implementiert
     * hoff ich :D
     */
    public void onGameLeft(GameLeftMessage message)
    {
        Debug.Log("Processing Game Left...");
    }

    /**
     * onGamePause
     * Pause Canvas im GameScreen anzeigen
     * falls Server die Pause enforced hat, pause button deaktivieren
     * Im View gibts eine PauseButton, die zum Pausieren oder zum Fortsetzen
     * geeignet ist
     */
    public void onGamePauseMessage(GamePauseMessage gamePauseMessage)
    {
        Debug.Log("Processing Game Pause...");
        this.isPaused = gamePauseMessage.gamePaused;

        // in case server forces the pause
        if (gamePauseMessage.serverEnforced && isPaused)
        {
            gamescreen.pausebutton_active(false);
        }
        else
        {
            gamescreen.pausebutton_active(true);
        }
        gamescreen.togglePause(isPaused);
    }

    /**
     * onMetaInfo
     * Falls Zuschauer Fraktion der p1 und p2 auffüllen
     */
    public void onMetaInformation(MetaInformationMessage message)
    {
        if(!userIsPlayerOne && !userIsPlayerTwo)
        {
            foreach(KeyValuePair<string, object> info in message.information)
            {
                if (info.Key.Equals(RequestMetaInformationMessage.keywords[3]))
                {
                    Guid[] p1_ids = info.Value as Guid[];
                    foreach(Guid id in p1_ids)
                    {
                        addToFraction(id); //here p1 fraction
                        gamescreen.markAgent(id, false);
                    }
                }
                else if (info.Key.Equals(RequestMetaInformationMessage.keywords[4]))
                {
                    Guid[] p2_ids = info.Value as Guid[];
                    foreach (Guid id in p2_ids)
                    {
                        addToEnemyFraction(id); // here p2 fraction
                        gamescreen.markAgent(id, true);
                    }
                }
            }
        }
    }
    
    /**
     * onStrike
     * Falls wir im Spiel sind, ich bin nich nehr dran
     * ka ob es sonst wo was machen soll
     */
    public void onStrike(StrikeMessage message)
    {
        Debug.Log("Processing strike message: " + message.reason + ", " + message.debugMessage);
        if(!activeCharacterGuid.Equals(Guid.Empty))
        {
            isMyTurn = false;
            gamescreen.activateDropdown(false);
            gamescreen.deactivateCommitment();
            gamescreen.hideButtons();
            gamescreen.markActiveChar(activeCharacterGuid, false);
            gamescreen.toggleTurnInfo(false);
        }
    }

    /**
     * onError
     * Errors are bitches
     * Einfach zurück in das Hauptmenü
     */
    public void onError(ErrorMessage message)
    {
        Debug.Log("Processing error message: " + message.reason + ", " + message.debugMessage);
        SceneManager.LoadScene("Scenes/MenuScene");
        //MenuEvents.changeLabelText("ConnectionProblem", message.reason);
    }

    // SENDERS
    /**
     * sendHello
     * Schickt Hello Nachricht mit einem zufaelligen ID (Server sagt mir meine ID spaeter)
     * übergebenen Name und Role (Spieler/Zuschauer)
     */
    public void sendHelloMessage(string name, RoleEnum role)
    {
        Debug.Log("Saying hello to server...");
        HelloMessage message = new HelloMessage(Guid.NewGuid(), DateTime.Now, name, role);
        Connection.SendWebSocketMessage(message);
    }

    /**
     * sendReconnect
     * Sendet eine Reconnect Nachricht
     * wird glaub nie aufgerufen (weil es schon in Connection Klasse behandelt wird?)
     * but dont quote me on that
     */
    public void sendReconnectMessage()
    {
        Debug.Log("Reconnecting...");
        ReconnectMessage message = new ReconnectMessage(playerId, sessionId, DateTime.Now);
        Connection.SendWebSocketMessage(message);
    }

    /**
     *   Diese Methode wird aufgerufen, wenn das Spiel verlassen wird und dafür eine Nachricht gesendent werden soll
     */
    public void sendGameLeaveMessage()
    {
        Debug.Log("Sending leave request...");
        GameLeaveMessage message = new GameLeaveMessage(playerId);
        Connection.SendWebSocketMessage(message);
        Connection.closeWebsocket();
        SceneManager.LoadScene("Scenes/MenuScene");
    }

    /**
     * Sendet eine PauseAnfrage zum Server
     * fortsetzen, falls spiel pausiert
     * pausieren, falls spiel laeuft
     */
    public void sendPauseRequest()
    {
        Debug.Log("Sending pause request...");
        RequestGamePauseMessage message = new RequestGamePauseMessage(playerId,!isPaused);
        Connection.SendWebSocketMessage(message);
    }

    /**
     * RequestMetaInfo
     * Rufen wir nie auf
     * Vielleicht beim Zuschauen
     */
    public void sendMetaInfoRequest(string[] keys)
    {
        Debug.Log("Gettin them meta infoo...");
        RequestMetaInformationMessage message = new RequestMetaInformationMessage(playerId, keys);
        Connection.SendWebSocketMessage(message);
    }

    /**
        Diese Methode gibt das Gender des Charakteren zurück
        @param Charakter: 

        return GenderEnum

        Bemerkung: Geschlecht ist ein optionales Feld!!!!!!!!
    **/
    public GenderEnum GetGender(Character ch){
        Guid uuid = ch.getGuid();
        CharacterInformation info = null;
        for(int i = 0; i < characterSettings.Length; i++){
            if(characterSettings[i].getCharacterId().Equals(uuid)) info = characterSettings[i];
        }
        if (info != null)
        {
            GenderEnum gender = info.GetGender();
            return gender;
        }
        return GenderEnum.None;
    }

    /**
        Diese Methode überprüft, ob der Client das Geheimnis zum Tresor kennt oder nicht
        @param safeIndex: Index des Safes

        return true, falls der Client das Geheimnis zu diesem Tresor kennt, somst false
    **/
    private bool isTresorSecretKnown(int safeIndex){
        if (mySafeCombinations != null)
        {
            HashSet<int>.Enumerator enu = mySafeCombinations.GetEnumerator();
            while (enu.MoveNext())
            {
                int secret = enu.Current;
                if (secret == safeIndex) return true;
            }
        }
        return false;
    }

    /**
        Diese Methode erfrägt, ob der Charakter der eigenen Fraktion angehört
        @param ch: Charakter auf der Spielfeld

        return true, falls der Charakter der eigenen Fraktion angehört sonst false
    **/
    public bool IsOwnFractionMember(Character ch){
        Guid uuid = ch.getGuid();
        return fraction.Contains(uuid);
    }

    /**
        Diese Methode erfrägt, ob der Charakter der eigenen Fraktion angehört
        @param ch: Charakter ID des Charakters

        return true, falls der Charakter der eigenen Fraktion angehört sonst false
    **/
    public bool IsOwnFractionMember(Guid ch)
    {
        return fraction.Contains(ch);
    }

    /**
        Diese Methode überprüft, ob auf einem gegebenen Feld ein Charakter existiert
        @param pos: Position des Feldes
        return Character, falls einer auf diesem Feld existiert, sonst null
    **/
    public Character ExistsCharacter(Point pos){
        HashSet<Character>.Enumerator enu = characters.GetEnumerator();
        while(enu.MoveNext()){
            Character ch = enu.Current;
            if(ch.getCoordinates().EqualsPoint(pos)) return ch;
        }
        //null, falls auf dieser Position kein Charakter existiert
        return null;
    }


    /**
        Diese Methode git das Feld an der gegeben Position zurück
        @param pos: Position im Spielfeld

        return Field
    **/
    public Field GetFieldOnPos(Point pos){
        if(map == null)
        {
            return null;
        }
        Field[,] gameField = map.getMap();
        return gameField[pos.y,pos.x];
    }

    /**
        Diese Methode errechnet die Entfernung gemäß Definiton aus dem Lastenheft,
        da diese Methode eine Hilfsmethode ist, ist sie static
        Bemerkung: Diese Methode geht von einem "kontinuierlichen" Spieleld aus

        //Diese Methode gilt nur solang das Spielfeld rechteckig ist
    **/
    public static int calculateDistance(Point a, Point b){
        int a_x, a_y, b_x, b_y;
        a_x = a.x;
        a_y = a.y;
        b_x = b.x;
        b_y = b.y;

        int x_distance = Math.Abs((a_x - b_x));
        int y_distance = Math.Abs((a_y - b_y));

        return Math.Max(x_distance, y_distance);

    }

    /**
        Diese Methode validiert den Zug des active Charakters, der vom Spieler angeregt wird.
        Sie wird aufgerufen, wenn ein Spieler eine Gadget Aktion oder Ähnliches ausführen möchte
        @param gadget: Gadget, das ausgewählt wurde (View: DropDown)
        @param target: Ziel, auf das das Gadget zielt (View: Clicked Position)
        Bemerkung: Zu target gehören die LOGISCHEN Koordinaten gemäß Standard!!!! (sprich: die y Koordinate muss invertiert werden!!!!)
        !!!!Parameter müssen valide sein: target muss sich im Spielfeld befinden, falls nicht: So muss hier eine extra Validierung erfolgen !!!!

        @return true, falls Spielzug valide ist, sonst false

        Diese Methode wird von der View aufgerufen, bei true sollte die Canvas geblockt werden

        Ergänzung am 08.05.2020: Abfrage nach Katze und Hausmeister Koordinaten + weitere Entscheidungen des Standards in Ausnahmefällen + Nugget Aktion + Jetpack konkretisiert
        Ergänzung am 14.05.2020: User Input, welcher sinnlos ist, wird verhindert!!
    **/
    public bool validate(DropDownOption action, Point target)
    {
        //Charakter, der am Zug ist
        Character activeCharacter = getActiveCharacter();

        //Positionen des Charakters
        Point coordinates = activeCharacter.getCoordinates();
        int coordinates_x = coordinates.x;
        int coordinates_y = coordinates.y;

        //Distanz zwischen dem active Player und dem Ziel
        int distance = calculateDistance(coordinates, target);

        /*
        //Überprüfe ob Charakter genügend APs hat, muss wahrscheinlich an einer früheren Stelle im Code geschehen !!!!
        if (activeCharacter.getAp() <= 0) return false;
        */

        bool valid = false;

        //als Hilfsvariablen
        Field field;
        Character character_target;
        Gadget gadget_target;

        //Katze und Hausmeister dürfen NIEMALS als Ziel ausgewählt werden!!
        if (ExistsJanitorOrCatOnPos(target))
        {
            gamescreen.updateLogLabel("Ungültige Aktion!");
            return false;
        }



        switch (action)
        {
            //Spielzug frühzeitig beenden
            case DropDownOption.Retire:
                //target == null, da hier kein Ziel erforderlich ist
                Operation op = new Operation(activeCharacterGuid, OperationEnum.RETIRE, null);
                sendGameOperationMessage(op);
                valid = true;
                break;

            //Pausierung
            case DropDownOption.Pause_Setting:
                valid = false;
                break;

            //bei Bewegungen werden Aktionen ignoriert
            case DropDownOption.Movement:
                valid = false;
                break;

            //Agenteninformationen anzeigen
            /*            case DropDownOption.show_Agent_Information:
                            character_target = ExistsCharacter(target);
                            if (character_target != null && IsOwnFractionMember(character_target))
                            {
                                gamescreen.fill_AgentInfo(character_target.getName(), GetGender(character_target), character_target.getChips(), mySafeCombinations,
                                                            character_target.getProperties(), character_target.GetGadgets(),
                                                            character_target.getHp(), character_target.getIp(),
                                                            character_target.getAp(), character_target.getMp());
                            }
            */
            //Bemerkung
            //Valid ist BEWUSST false, damit die Canvas im GameScreen nicht blockiert wird
            /*              valid = false;
                            break;
            */

            //Roulette spielen
            case DropDownOption.Play_Roulette:
                field = GetFieldOnPos(target);
                //Einsatz
                int commitment = 0;
                if (distance == 1 && field.getFieldStateEnum().Equals(FieldStateEnum.ROULETTE_TABLE) && !field.getIsDestroyed())
                {
                    int chipAmount = field.GetChipAmount();
                    commitment = gamescreen.getCommitmentForRoulette();
                    // Input invalide
                    if (commitment == -1) valid = false; //Fehlermeldung wird generiert!

                    // Der Spieler hat nicht so viele Spielchips
                    else if (commitment > activeCharacter.getChips())
                    {
                        gamescreen.printErrorInCommitmentForRoulette("Der Charakter hat nicht so viele Chips!");
                        valid = false;
                    }

                    //Einsatz zu hoch
                    else if (commitment > chipAmount)
                    {
                        gamescreen.printErrorInCommitmentForRoulette("Einsatz zu hoch!");
                        valid = false;
                    }

                    //Einsatz passend
                    else if (commitment > 0 && commitment <= chipAmount) valid = true;
                }
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GambleAction operation = new GambleAction(commitment, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;


            //Föhn
            //Einschränkung: Zum Föhnen muss die Zielperson die klamme Klamotten Eigenschaft haben
            case DropDownOption.HAIRDRYER:

                character_target = ExistsCharacter(target);

                //Man föhnt sich selber
                if (distance == 0 && (activeCharacter.hasProperty(PropertyEnum.CLAMMY_CLOTHES) || activeCharacter.hasProperty(PropertyEnum.CONSTANT_CLAMMY_CLOTHES))) valid = true;

                //Man föhnt Charakter auf Nachbarfeld
                else if (character_target == null) valid = false;

                else if (distance == 1 && (character_target.hasProperty(PropertyEnum.CLAMMY_CLOTHES) || character_target.hasProperty(PropertyEnum.CONSTANT_CLAMMY_CLOTHES))) valid = true;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.HAIRDRYER, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Maulwürfel
            //Weitere Einschränkungen: Zielperson darf nicht vom eigenen Team sein, da gute Eigenschaften deaktiviert werden!
            //Aber: Falls das Gadget beim Abprall einem eigenen Agenten ins Inventar hüpft, hat der Spieler Pech gehabt
            case DropDownOption.MOLEDIE:
                field = GetFieldOnPos(target);
                if (!(field.getFieldStateEnum().Equals(FieldStateEnum.WALL)) && distance <= settings.moledieRange && map.validateIsInSight(coordinates, target))
                {
                    character_target = ExistsCharacter(target);
                    if (character_target != null && !IsOwnFractionMember(character_target)) valid = true;
                    else valid = false;
                }
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.MOLEDIE, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Technicolor-Prisma
            //Bemerkung: Der Roulette Tisch muss außerdem brauchbar sein
            case DropDownOption.TECHNICOLOUR_PRISM:
                field = GetFieldOnPos(target);

                if (distance == 1 && field.getFieldStateEnum().Equals(FieldStateEnum.ROULETTE_TABLE) && !field.getIsDestroyed()) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.TECHNICOLOUR_PRISM, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Klingen Hut
            //Beachte: Ergänzung der Sichtlinie!
            //Einschränkung: Eigene Charaktere dürfen nicht mit dem Klingen Hut getroffen werden
            case DropDownOption.BOWLER_BLADE:
                //Die Sichtlinie muss für dieses Gadget angepasst werden, Felder mit Charakteren haben eine blockierende Wirkung
                //GameTile.raycast_specialMode = true;
                //GameScreen.tilemap.RefreshAllTiles();
                //ist die Sichtlinie frei

                /**

                **/
                bool raycast_successful = map.validateIsInSight(coordinates, target) && map.validateIsNotBlocked(coordinates, target, characters);
                //GameTile.raycast_specialMode = false;
                //GameScreen.tilemap.RefreshAllTiles();

                //Zielperson
                character_target = ExistsCharacter(target);

                if (distance <= settings.bowlerBladeRange && character_target != null && !IsOwnFractionMember(character_target) && raycast_successful) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.BOWLER_BLADE, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Giftpillen
            //1. Einschränkung: Bereits vergiftete Cocktails können nicht nochmal vergiftet werden!
            //2. Einschränkung: Man darf nicht den Cocktail eines eigenen Agenten vergifteten, da dies keinen Vorteil bietet!
            case DropDownOption.POISON_PILLS:
                field = GetFieldOnPos(target);
                character_target = ExistsCharacter(target);
                gadget_target = field.GetGadget();

                //Cocktail auf dem Tisch: Fall 1, Abfrage nach Tisch - Feld wird verzichtet, da redundant
                if (distance == 1 && gadget_target != null && gadget_target.gadget.Equals(GadgetEnum.COCKTAIL))
                {
                    Cocktail cocktail = (Cocktail)gadget_target;
                    if (!cocktail.isPoisoned) valid = true;
                    else valid = false;
                }

                //Cocktail in der Hand eines Agenten auf dem Nachbarfeld: Fall 2
                else if (distance == 1 && character_target != null && !IsOwnFractionMember(character_target))
                {
                    gadget_target = character_target.hasGadget(GadgetEnum.COCKTAIL);
                    if (gadget_target != null)
                    {
                        Cocktail cocktail = (Cocktail)gadget_target;
                        if (!cocktail.isPoisoned) valid = true;
                        else valid = false;
                    }
                    else valid = false;
                }
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.POISON_PILLS, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }

                break;

            //Laser
            case DropDownOption.LASER_COMPACT:
                field = GetFieldOnPos(target);
                character_target = ExistsCharacter(target);
                gadget_target = field.GetGadget();

                bool raycasting = map.validateIsInSight(coordinates, target);

                //Cocktail auf dem Tisch: Fall 1
                if (gadget_target != null && gadget_target.gadget.Equals(GadgetEnum.COCKTAIL) && raycasting) valid = true;


                //Cocktail in der Hand eines Agenten auf dem Nachbarfeld: Fall 2
                else if (raycasting && character_target != null)
                {
                    gadget_target = character_target.hasGadget(GadgetEnum.COCKTAIL);
                    if (gadget_target != null) valid = true;
                    else valid = false;
                }
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.LASER_COMPACT, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Raketenwerfer
            //Bemerkung: Aufgrund des Flächenschadens wird hier nicht die Zielperson unterschieden, Änderung ??
            case DropDownOption.ROCKET_PEN:
                valid = map.validateIsInSight(coordinates, target);

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.ROCKET_PEN, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Lippenstift
            //Einschränkung: Eigener Agent darf keinen Schaden bekommen!
            case DropDownOption.GAS_GLOSS:
                //Zielperson
                character_target = ExistsCharacter(target);

                if (distance == 1 && character_target != null && !IsOwnFractionMember(character_target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.GAS_GLOSS, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Mottenkugel
            //Bemerkung: Aufgrund des Flächenschadens keine Einschränkung
            case DropDownOption.MOTHBALL_POUCH:
                field = GetFieldOnPos(target);

                if (field.getFieldStateEnum().Equals(FieldStateEnum.FIREPLACE) && distance <= settings.mothballPouchRange && map.validateIsInSight(coordinates, target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.MOTHBALL_POUCH, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Nebeldose
            //Aufgrund des Flächenwirkung wird hier nichts eingeschränkt!
            case DropDownOption.FOG_TIN:
                field = GetFieldOnPos(target);
                if (!(field.getFieldStateEnum().Equals(FieldStateEnum.WALL)) && distance <= settings.fogTinRange && map.validateIsInSight(coordinates, target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.FOG_TIN, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Wurfhaken
            //Mögliche Einschränkung (?) : Einen vergifteten Cocktail angeln????
            case DropDownOption.GRAPPLE:
                field = GetFieldOnPos(target);
                gadget_target = field.GetGadget();
                if (distance <= settings.grappleRange && map.validateIsInSight(coordinates, target) && gadget_target != null
                && (gadget_target.gadget.Equals(GadgetEnum.COCKTAIL) || gadget_target.gadget.Equals(GadgetEnum.BOWLER_BLADE)
                || gadget_target.gadget.Equals(GadgetEnum.DIAMOND_COLLAR))) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.GRAPPLE, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Jetpack
            case DropDownOption.JETPACK:
                field = GetFieldOnPos(target);
                character_target = ExistsCharacter(target);
                bool npc_target = ExistsJanitorOrCatOnPos(target);
                //Auf dem Zielfeld darf keine andere Person stehen
                if (field.getFieldStateEnum().Equals(FieldStateEnum.FREE) && character_target == null && !npc_target) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.JETPACK, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Wanze
            //Die Wanze darf nicht einem eigenen Agenten gegeben werden !!
            //Es muss eine frühere Überprüfung geschehen, ob das Gadget aktiv ist und benutzt werden darf...
            case DropDownOption.WIRETAP_WITH_EARPLUGS:
                character_target = ExistsCharacter(target);
                if (distance == 1 && character_target != null && !IsOwnFractionMember(character_target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.WIRETAP_WITH_EARPLUGS, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Chicken Feed
            //Einschränkung: Das Chicken Feed darf nicht einer eigenen Zielperson gegeben werden, da dieses sonst verschwindet, also kein Vorteil
            case DropDownOption.CHICKEN_FEED:
                character_target = ExistsCharacter(target);
                if (distance == 1 && character_target != null && !IsOwnFractionMember(character_target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.CHICKEN_FEED, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;


            //Mirror
            //Bemerkung: Darf an einem eigenen Agenten angewandt werden
            case DropDownOption.MIRROR_OF_WILDERNESS:
                if (distance == 1 && ExistsCharacter(target) != null) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.MIRROR_OF_WILDERNESS, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Nugget
            //Einschränkung: Das Nugget darf nicht einem eigenen Agenten gegebenen werden
            case DropDownOption.NUGGET:
                character_target = ExistsCharacter(target);
                if (distance == 1 && character_target != null && !IsOwnFractionMember(character_target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    GadgetAction operation = new GadgetAction(GadgetEnum.NUGGET, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Cocktail trinken
            //Diskussion: Muss extra auf das Feld des Spielers geklickt werden ??  ---!
            //Mögliche Einschränkung (?): Darf man einen vergifteten Cocktail trinken???
            //Bermerkung: Die Option des Trinkens muss an einer früheren Stelle abgefragt werden
            case DropDownOption.Drinking_Cocktail:

                if (distance == 0 && activeCharacter.hasGadget(GadgetEnum.COCKTAIL) != null) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht 
                    GadgetAction operation = new GadgetAction(GadgetEnum.COCKTAIL, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;


            //Cocktail vom Tisch nehmen
            case DropDownOption.Collect_Cocktail:
                //Muss der Tisch ein Nachbarfeld vom Agenten sein?? - logische Schlussfolgerung: ja
                field = GetFieldOnPos(target);
                gadget_target = field.GetGadget();
                //Abfrage, ob Zielfeld ein Tisch ist, ist nicht nötig
                if (distance == 1 && gadget_target != null && gadget_target.gadget.Equals(GadgetEnum.COCKTAIL)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht 
                    GadgetAction operation = new GadgetAction(GadgetEnum.COCKTAIL, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Cocktail verschütten
            //1. Einschränkung: Der Cocktail darf nicht über einen eigenen Agenten geschüttet werden, da dieser sonst die klamme Klamotten Eigenschaft hat!
            //2. Einschränkung: Der Cocktail darf nicht über einen Agenten verschüttet werden, der bereits eine klamme Klamotten Eigenschaft hat, da dies unnötig ist!
            case DropDownOption.Spill_Cocktail:

                character_target = ExistsCharacter(target);
                if (distance == 1 && character_target != null && !IsOwnFractionMember(character_target) && !(character_target.hasProperty(PropertyEnum.CLAMMY_CLOTHES) || character_target.hasProperty(PropertyEnum.CONSTANT_CLAMMY_CLOTHES))) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht 
                    GadgetAction operation = new GadgetAction(GadgetEnum.COCKTAIL, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Leute ausspionieren
            //Einschränkung: Eigene Agenten dürfen nicht ausspioniert werden
            case DropDownOption.Spy_People:
                character_target = ExistsCharacter(target);
                if (distance == 1 && character_target != null && !IsOwnFractionMember(character_target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    Operation operation = new Operation(activeCharacterGuid, OperationEnum.SPY_ACTION, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Tresor öffnen
            //Einschränkung: Der Agent muss das Geheimnis zum Tresor kennen!
            case DropDownOption.Open_Tresor:
                field = GetFieldOnPos(target);
                //Fall 1: Nachbarfeld
                if (distance == 1 && field.getFieldStateEnum().Equals(FieldStateEnum.SAFE) && isTresorSecretKnown(field.getSafeIndex())) valid = true;
                //Fall 2: Distanz 2, aber Flaps and Seals Eigenschaft
                else if (distance == 2 && activeCharacter.hasProperty(PropertyEnum.FLAPS_AND_SEALS) && field.getFieldStateEnum().Equals(FieldStateEnum.SAFE) && isTresorSecretKnown(field.getSafeIndex())) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    Operation operation = new Operation(activeCharacterGuid, OperationEnum.SPY_ACTION, target);
                    sendGameOperationMessage(operation);
                }

                break;

            //Observation
            //Einschränkung: Darf nicht auf eigene Agenten angewandt werden, da sinnlos
            case DropDownOption.Observation:
                character_target = ExistsCharacter(target);
                if (character_target != null && !IsOwnFractionMember(character_target) && map.validateIsInSight(coordinates, target)) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    PropertyAction operation = new PropertyAction(PropertyEnum.OBSERVATION, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;

            //Bang and Burn
            //Einschränkung: Der Roulette Tisch muss davor schon brauchbar sein!
            case DropDownOption.Bang_and_Burn:
                field = GetFieldOnPos(target);
                //Der Roulette Tisch muss außerdem brauchbar sein
                if (distance == 1 && field.getFieldStateEnum().Equals(FieldStateEnum.ROULETTE_TABLE) && !field.getIsDestroyed()) valid = true;
                else valid = false;

                if (valid)
                {
                    //Sende Nachricht
                    PropertyAction operation = new PropertyAction(PropertyEnum.BANG_AND_BURN, activeCharacterGuid, target);
                    sendGameOperationMessage(operation);
                }
                break;
        }
        if (!valid) //falls Operation invalide, im GameScreen anzeigen
        {
            gamescreen.updateLogLabel("Ungültige Aktion!");
        }
        return valid;
    }


    /**
        Diese Methode frägt ab, ob an einer übergebenen Position sich die Katze oder der Hausmeister befindet
        @param target: Zielfeld

        return true, falls sich der Hausmeister oder die Katze auf dem Feld target befindet, sonst false
    **/
    public bool ExistsJanitorOrCatOnPos(Point target)
    {
        return janitorCoordinates == null ? target.EqualsPoint(catCoordinates) : (target.EqualsPoint(catCoordinates) || target.EqualsPoint(janitorCoordinates));
    }

    /**
        Diese Methode überprüft, ob die Katze auf dem angegebenen Feld existiert
    **/
    public bool ExistsJanitorOnPos(Point target)
    {
        if(janitorCoordinates != null)
        {
            return target.EqualsPoint(janitorCoordinates);
        }
        return (janitorCoordinates != null) ? target.EqualsPoint(janitorCoordinates) : false;
    }


    /**
        Diese Methode überprüft, ob die Katze auf dem angegebenen Feld existiert
    **/
    public bool ExistsCatOnPos(Point target)
    {
        return target.EqualsPoint(catCoordinates);
    }


    //Ergänzung zur WebSocket-Verbindung

    /**
        Diese Methode sendet eine GameOperationMessage über die Connection Klasse an den Server
        Und deaktiviert alle Spielzugbezogene Funktionen des GameScreens
    **/
    public void sendGameOperationMessage(Operation operation)
    {
        Debug.Log("Sending game operation message...");
        GameOperationMessage message = new GameOperationMessage(playerId, DateTime.Now, operation);
        Connection.SendWebSocketMessage(message);
        // View aktualisieren, your turn no more
        isMyTurn = false;
        gamescreen.activateDropdown(false);
        gamescreen.deactivateCommitment();
        gamescreen.hideButtons();
        gamescreen.markActiveChar(activeCharacterGuid, false);
        gamescreen.toggleTurnInfo(false);
    }

    /**
        Diese Methode wird von der Buttons Klasse aufgerufen
    **/
    public void sendMoveMessage(Point target)
    {
        Movement operation = new Movement(getActiveCharacter().getCoordinates(), activeCharacterGuid, target);
        sendGameOperationMessage(operation);
    }

}