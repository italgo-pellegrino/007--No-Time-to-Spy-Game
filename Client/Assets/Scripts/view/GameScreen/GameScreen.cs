using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
//EventSystem
using UnityEngine.EventSystems;
using System;
using System.Linq;
using TMPro;

public class GameScreen : MonoBehaviour
{
    /**
     * GameHandler der Szene, Bearbeitung View unabhaengiger Zeug hier
     * Triggert bestimmte Funktionen in dem View
     */
    public GameHandler gameHandler;

    /**
     * Representierung des Spielfeldes über Tilemap
     */
    public static Tilemap tilemap;

    /**
     * Camera der Szene, hat den Tag MainCamera
     * Braucht man hier nur damit man Kamera am Anfang des Spiels in die Mitte legt
     */
    public GameObject main_cam;

    //Pause Panel
    public static GameObject pause_menu;
    //Loading Panel (soll bis zum ersten GameStatusMessage da sein)
    public static GameObject loading_panel;

    //Drop Down Menu um mogliche Aktionen aufzulisten
    private Dropdown dropDown;

    public Dropdown GetDropDown()
    {
        return dropDown;
    }

    /**
     * Komponenten von Turn Indikator oben im View
     */
    private Image turnIndImage; // der Kreis
    private TextMeshProUGUI turnIndText; // der Text

    /**
        Dieses Array enthält alle GameObjects der Buttons, um diese zu de-aktivieren
    **/
    private GameObject[] buttons_gameObjects;

    /**
        Dieses GameObject repräsentiert den Pointer im Spiel
    **/
    private GameObject pointer;

    /**
        Objekt, auf dem der Pointer war
    **/
    private GameObject pointed_GameObject, pointed_dynObject, pointed_gadget;

    /**
        Ursprüngliche Farbe des pointed Objects, wichtig bei Agenten mit klamme Klamotten Eigenschaft
    **/
    private Color pointed_GameObject_Color, pointed_dynObject_Color, pointed_gadget_Color;

    /**
     * Label unterhalb des Views
     */
    private TextMeshProUGUI pointLabel; // Cursor Koordinaten
    private TextMeshProUGUI logLabel; // Log Label
    private TextMeshProUGUI roundLabel; // Rundenanzeige

    /**
        Array with Agent Information Labels
        array[0]: Name
        array[1]: Gender
        array[2]: GameChips
        array[3]: Secrets
        array[4]: Features
        array[5]: Gadgets
        array[6]: HP
        array[7]: IP
        array[8]: AP
        array[9]: MP
    **/
    private static Text[] agent_infos;
    private static Image agent_icon;



    /**
        Dies ist das InputField für den Einsatz von Spielchips, der vom Benutzer angegeben wird
    **/
    private static GameObject roulette_table_commitment;

    /**
     * Wenn man den Pointer auf einen Roulette Tisch setzt,
     * sieht man wie viele Chips am Tisch sind
     */
    private GameObject roulette_info;

    /**
     * Statistiken Canvas
     * nach Statistics Nachricht anzeigen
     */
    private GameObject statsCanvas;

    /**
     * Hashtable for Guid, Char objects
     */
    public static Dictionary<Guid, GameObject> char_table = new Dictionary<Guid, GameObject>();

    /**
     * HashTable for character coloring
     */
    public static Dictionary<int, Color> char_colors = new Dictionary<int, Color>() {
        {0, Color.white },
        {1, new Color(1, 1, 0.6f)},
        {2, new Color(1, 0.6f, 1) },
        {3, new Color(1, 0.6f, 0.6f) },
        {4, new Color(0.6f, 1, 1) },
        {5, new Color(0.6f, 1, 0.6f)},
        {6, new Color(0.6f, 0.6f, 1) },
        {7, new Color(1, 1, 0.8f) },
        {8, new Color(1, 0.8f, 1) },
        {9, new Color(1, 0.8f, 0.8f) },
        {10, new Color(0.8f, 0.8f, 1) },
        {11, new Color(0.8f, 1, 1) },
        {12, new Color(0.8f, 0.8f, 0.8f) },
        {13, new Color(0.6f, 0.8f, 0.8f) },
        {14, new Color(0.8f, 0.8f, 0.6f) },
        {15, new Color(0.8f, 0.6f, 0.8f) },
        {16, new Color(0.7f, 0.7f, 0.9f) },
        {17, new Color(0.9f, 0.7f, 1) },
        {18, new Color(0.7f, 1, 0.9f) },
        {19, new Color(1, 0.9f, 0.7f) },
        {20, new Color(0.9f, 1, 0.7f) },
    };

    //Es wird mit der Referenz des GameHandlers gearbeitet, da der GameHandler standig geupdatet wird
    //Es wird darauf geachtet, dass auf den GameHandler nur ein lesender Zugriff erfolgt!!
    public void setGameHandler(ref GameHandler gameHandlerReference)
    {
        gameHandler = gameHandlerReference;
    }



    void Start()
    {
        // Kamera Objekt durch den Tag setzen
        main_cam = GameObject.FindGameObjectWithTag("MainCamera");

        // Pause Panel deaktivieren
        pause_menu = GameObject.Find("PausePanel"); ;
        pause_menu.SetActive(false);

        // Loading Panel
        loading_panel = GameObject.Find("Loading");

        tilemap = gameObject.GetComponent<Tilemap>();

        setGameHandler(ref Connection.gameHandler);

        //GameScreen Attribut vom GameHandler gesetzt
        gameHandler.setGameScreen(this);

        //Zuweisung des Drop Downs
        dropDown = GameObject.FindGameObjectWithTag("Dropdown").GetComponent<Dropdown>();
        //Methode, die aufgerufen wird, wenn sich der Wert im Drop Down geändert wird
        dropDown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropDown);
        });
        dropDown.enabled = false;

        // turn indicator image and text
        turnIndImage = GameObject.Find("TurnColor").GetComponent<Image>();
        turnIndImage.color = Color.red;
        turnIndText = GameObject.Find("TurnText").GetComponent<TextMeshProUGUI>();

        //Zuweisung des Pointers im Spiel
        pointer = GameObject.FindGameObjectWithTag("Pointer");

        //Zuweisung Info Labels
        pointLabel = GameObject.Find("PointLabel").GetComponent<TextMeshProUGUI>();
        logLabel = GameObject.Find("LogLabel").GetComponent<TextMeshProUGUI>();
        roundLabel = GameObject.Find("RoundLabel").GetComponent<TextMeshProUGUI>();

        //Suche nach Buttons im Spiel, um sie zu aktivieren oder deaktivieren
        buttons_gameObjects = GameObject.FindGameObjectsWithTag("Button");

        //Zuweisung der Agent Info Labels
        agent_infos = new Text[]{GameObject.FindGameObjectWithTag("AgentInfo_Name").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_Gender").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_GameChips").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_Secrets").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_Features").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_Gadgets").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_HP").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_IP").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_AP").GetComponent<Text>(),
                                GameObject.FindGameObjectWithTag("AgentInfo_MP").GetComponent<Text>()
                                };
        agent_icon = GameObject.FindGameObjectWithTag("AgentInfo_Icon").GetComponent<Image>();

        //Roulette Einsatz Eingabe
        roulette_table_commitment = GameObject.FindGameObjectWithTag("commitment");
        roulette_table_commitment.SetActive(false);
        //Roulette Info (Pointer auf Roulette)
        roulette_info = GameObject.Find("roulette_info");
        roulette_info.SetActive(false);

        hideButtons();

        statsCanvas = GameObject.Find("StatisticsCanvas");
        statsCanvas.SetActive(false);

        //Das aus dem Szenario geleser FieldMap bilden/anzeigen
        generateMap(gameHandler.getLevel().getScenario());
    }

    /**
     * TileMap Aufbau
     * Die entsprechende Sprites für Tiles eingesetzt und die statische GameObjekte
     * werden auf den Tiles platziert
     */
    private void generateMap(FieldStateEnum[,] scenario)
    {
        // camera positioned at the middle of the map (regardless of maps size)
        main_cam.transform.position = 0.5f * new Vector3(scenario.GetLength(1), -1 * scenario.GetLength(0) + 1, -20);

        for (int row = 0; row < scenario.GetLength(0); row++)
        // row go in y direction
        {
            for (int col = 0; col < scenario.GetLength(1); col++)
            // columns go in x direction
            {
                Vector3Int tile_position = new Vector3Int(col, -row, 0);

                GameTile tile = ScriptableObject.CreateInstance<GameTile>();
                GameObject tileObject = null;
                // alle wichtige Vorgaenge beim Tile Erzeugung hier
                setTileInfo(scenario[row, col], row, col, ref tile, ref tileObject);
                //GetTileData in GameTile hier aufgerufen
                tilemap.SetTile(tile_position, tile);
            }
        }
    }

    void Update()
    {
        // get mouse click's position in 2d plane
        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pz.z = 0;

        // convert mouse click's position to Grid position
        GridLayout gridLayout = transform.parent.GetComponentInParent<GridLayout>();

        // absolute position in world
        Vector3Int cellPosition = gridLayout.WorldToCell(pz);
        //Debug.Log("world pos = " + cellPosition);


        //Diese Methode färbt Objekte die gepointet werden gelb
        point(cellPosition);
        //Zeigt Chip Anzahl des Roulette Tisches (falls Pointer auf einem ist)
        showRouletteInfo(cellPosition);

        // position of tile relative to tilemap
        Vector3Int relativeCellPosition = Vector3Int.Scale(cellPosition, new Vector3Int(1, -1, 1));

        //Verschieben von Pointer
        movePointer(cellPosition);
        //Zeigt die Koordinaten des Pointers an
        updatePointLabel(relativeCellPosition.x, relativeCellPosition.y);

        /*
         * Beim LClick falls Bedingungen erfüllt, validiere ausgewaehlte DropDownOption (entspricht eine Operation)
         * und falls valide schicke Operation zum Server
         * BEDINGUNGEN:
         * Spiel darf nicht pausiert sein
         * Spieler muss dran sein
         * Geclickte Tile muss im Spielfeld sein
         * Pointer darf kein UI Element anzeigen
         * Valide DropdownOption muss gewaehlt sein
         */
        if (Input.GetMouseButtonUp(0) && !gameHandler.isPaused && gameHandler.isMyTurn
            && relativeCellPosition.x >= 0 && relativeCellPosition.x < gameHandler.mapSize.y
            && relativeCellPosition.y >= 0 && relativeCellPosition.y < gameHandler.mapSize.x
            && !isPointerOverUIElement() && !getSelectedDropDownOption(dropDown).Equals(""))
        {
            //Sende Signal
            //Debug.Log("Acceptable Click");
            Point target = new Point(relativeCellPosition.x, relativeCellPosition.y);

            gameHandler.validate(DropDownTranslation.getDropDownOptionByString(getSelectedDropDownOption(dropDown)), target);
        }

    }

    /**
     * Diese Funktion prüft ob Pointer sich auf einem UI Element befindet
     * Vergleicht eigentlich einfach die Tags
     */
    private bool isPointerOverUIElement()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return EventSystem.current.currentSelectedGameObject.CompareTag("UI_Element")
                || EventSystem.current.currentSelectedGameObject.CompareTag("Button")
                || EventSystem.current.currentSelectedGameObject.CompareTag("Dropdown")
                || EventSystem.current.currentSelectedGameObject.CompareTag("commitment");
        }
        return false;
    }

    /**
     * Diese Methode setzt die Charaktere auf Tiles (als dynamicGameObjects)
     * @param prefab_name Name des Prefabs aus dem das GameObjekt instantiiert wird
     * @param coordinates Koordinaten des Tiles
     * @param agent_name Name der Agenten (GameObject wird mit dem gleichen Namen erstellt)
     * @param agent_id Damit GameObject in ID,GameObject HashTable eingefügt werden kann
     * @param char_in_fraction Wenn true wird ein grüner Marker auf dem Charakter gesetzt
     * @return hat Typ GameObject damit man Klamme Klammotten Eigenschaft von Anfang an anzeigen kann (da muss dem GameObject ein ChildObject zugewiesen werden)
     */
    public GameObject instantiate_character(string prefab_name, Vector3Int coordinates, string agent_name, Guid agent_id, bool char_in_fraction)
    {
        prefab_name = "Prefabs/" + prefab_name;
        //Instantiiere Objekt
        GameObject character = Instantiate(Resources.Load(prefab_name)) as GameObject;
        //Name des Objekts
        character.name = agent_name;
        //Farbe der Agenten aus dem HashTable lesen
        character.GetComponent<SpriteRenderer>().color = char_colors[char_table.Count];
        //falls Charakter aus eigenen Fraktion => marker drauf setzen (als ChildObject)
        if (char_in_fraction)
        {
            GameObject marker = Instantiate(Resources.Load("Prefabs/marker")) as GameObject;
            marker.GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.65f, 0);
            marker.GetComponent<SpriteRenderer>().sortingOrder = gameHandler.mapSize.max();
            marker.transform.SetParent(character.transform);
            marker.transform.localPosition = new Vector3(0, 2.55f, 0);
        }
        GameTile tile = (GameTile)tilemap.GetTile(Vector3Int.Scale(coordinates, new Vector3Int(1, -1, 1)));
        //charakter auf den Tile platzieren
        tile.setDynamicGameObject(ref character);
        //zum Hashset einfügen
        char_table.Add(agent_id, character);
        return character;
    }

    /**
     * Zur Instantiierung der Katze und des Hausmeisters
     * @param prefab_name,coordinates Analog wie oben
     * @param agent_name Wie das Objekt im Spielfeld heissen soll (Katze für katze Prefab, Hausmeister für Hausmeister prefab)
     * Ja, ja es könnte klüger gemacht werden aber es funktioniert jetzt und wir möchten nicht iwas dadurch kaputtmachen
     */
    public void instantiate_special_chars(string prefab_name, Vector3Int coordinates, string agent_name)
    {
        prefab_name = "Prefabs/" + prefab_name;
        GameObject character = Instantiate(Resources.Load(prefab_name)) as GameObject;
        character.name = agent_name;
        GameTile tile = (GameTile)tilemap.GetTile(Vector3Int.Scale(coordinates, new Vector3Int(1, -1, 1)));
        tile.setDynamicGameObject(ref character);
    }

    /**
     * Allgemeine Animationsfunktion
     * Alle Animationsoperationen müssen hier erst downcastet werden und dann werden die animationsspezifische
     * Funktionen aufgerufen (meiste sind statische Funktionen im Skript Throwing)
     * Falls Animation Effekte hat (wie z.B. falsch Ausspionieren - fügt Agenten zur Gegner-Fraktion zu)
     * Sie werden hier nach der Animation behandelt
     * @return IEnumerator damit Effekte von aufgerufene Methoden über mehrere Frames dauern
     * Wir warten i.A. bis Animationen fertig sind mit yield return
     */
    public IEnumerator animate(GameStatusMessage message)
    {
        /*
         * Animation Controller wurde so gemacht, dass man aus dem Default Animation (keine Clip, Sprite wird gezeigt)
         * alle Animationen über Triggers aufrufen kann.
         * Was mir nicht aufgefallen ist, dass man die sprite vom SpriteRenderer nicht mehr aendern kann,
         * wenn Objekt ein Animator hat. Daher sollten Stehen mit Cocktail und Sitzen allgemein als weitere Animationen
         * integriert werden. Die sind idR nur Sprite Anzeigen (1 Frame langes Anim in loop). Diese weitere Animationen
         * triggert man über bools
         * Hier geht man zurück auf das Default Animation, damit man die anderen Animationen aufrufen kann
         * Nachdem Animation durch ist (beim State update), werden flags wieder gesetzt (damit inaktive Charakter richtige Sprite hat)
         * Die Stellen sind mit DEFAULT kommentiert
         */
        Operation op;
        Animator anim;
        //variabel Downcasting
        var operations = message.operations;
        foreach (var operation in operations)
        {
            // Schreibe in Log Label welches Aktion ausgeführt ist
            updateLogLabel(operation as BaseOperation);
            switch (operation.type)
            {
                case OperationEnum.GADGET_ACTION:
                    //spezifischer Downcasten
                    GadgetAction gad_op = operation as GadgetAction;
                    if (gad_op != null)
                    {
                        //DEFAULT
                        anim = char_table[gad_op.characterId].GetComponent<Animator>();
                        anim.SetBool("stand", true);
                        anim.SetBool("cocktail", false);

                        yield return animateGadgetActions(gad_op, operation.target);
                    }
                    break;
                case OperationEnum.SPY_ACTION:
                    //spezifischer Downcasten
                    op = operation as Operation;
                    if (op != null)
                    {
                        //DEFAULT
                        anim = char_table[op.characterId].GetComponent<Animator>();
                        anim.SetBool("stand", true);
                        anim.SetBool("cocktail", false);

                        //falls Spy Action aufm Safe Tresor öffnen
                        if (gameHandler.getFieldMap().getField(operation.target).getState() == FieldStateEnum.SAFE && operation.successful)
                        {
                            Vector3Int tileLocation = Vector3Int.Scale(Point.point_to_vector3int(operation.target), new Vector3Int(1, -1, 1));
                            Throwing.OpenTresor_Animation(tilemap.GetInstantiatedObject(tileLocation));
                        }
                        else // falls auf Agenten, ausspionieren Animation
                        {
                            yield return Throwing.Spy_Animation(char_table[op.characterId], op.target.x, -op.target.y);
                            bool targetFromMyFraction = gameHandler.IsOwnFractionMember(gameHandler.ExistsCharacter(operation.target));
                            if (!isSpectatorScreen && targetFromMyFraction && !operation.successful) // ich wurde ausspioniert (nur wenn client Spieler)
                            {
                                // in EnemyFraktion einfügen und charaktere als Gegner markieren
                                gameHandler.addToEnemyFraction(op.characterId);
                                markAgent(op.characterId, true);
                                updateLogLabel(gameHandler.getCharacterById(op.characterId).name + " hat versucht dich zu spionieren. That MF!");
                            }
                        }
                    }
                    break;
                case OperationEnum.GAMBLE_ACTION:
                    //spezifischer Downcasten
                    op = operation as Operation;
                    if (op != null)
                    {
                        //DEFAULT
                        anim = char_table[op.characterId].GetComponent<Animator>();
                        anim.SetBool("stand", true);
                        anim.SetBool("cocktail", false);
                        yield return Throwing.PlayRoulette_Animation(char_table[op.characterId], op.target.x, -op.target.y);
                    }
                    break;
                case OperationEnum.PROPERTY_ACTION:
                    //spezifischer Downcasten
                    PropertyAction pro_op = operation as PropertyAction;
                    if (pro_op != null)
                    {
                        //DEFAULT
                        anim = char_table[pro_op.characterId].GetComponent<Animator>();
                        anim.SetBool("stand", true);
                        anim.SetBool("cocktail", false);
                        //allmeine Property Action Funktion
                        yield return animatePropertyActions(pro_op);
                    }
                    break;
                case OperationEnum.MOVEMENT: // DEFAULT in moveCharacter Funktion
                    if (operation.successful)
                    {
                        // NOTE TO SELF: Downcasting with as, upcasting with paranthesis
                        //spezifischer Downcasten
                        Movement mov_op = operation as Movement;
                        if (mov_op != null)
                        {
                            yield return moveCharacter(Point.point_to_vector3int(mov_op.from),
                              Point.point_to_vector3int(operation.target));
                        }
                    }
                    break;
                case OperationEnum.CAT_ACTION: // DEFAULT in moveCat Funktion
                    if (operation.successful)
                    {
                        Field targetTile = message.state.map.getMap()[operation.target.y, operation.target.x];
                        yield return moveCat(Point.point_to_vector3int(operation.target),
                            (targetTile.GetGadget() != null && targetTile.GetGadget().gadget == GadgetEnum.DIAMOND_COLLAR));
                    }
                    break;
                case OperationEnum.JANITOR_ACTION:
                    if (operation.successful)
                    {
                        yield return moveJanitor(Point.point_to_vector3int(operation.target));
                    }
                    break;
                case OperationEnum.EXFILTRATION: // DEFAULT in exfiltration Funktion
                    if (operation.successful)
                    {
                        Exfiltration ex_op = operation as Exfiltration;
                        if (ex_op != null)
                        {
                            yield return exfiltrateCharacter(Point.point_to_vector3int(ex_op.from), Point.point_to_vector3int(ex_op.target));
                        }
                    }
                    break;
                case OperationEnum.RETIRE: // mache nichts
                default:
                    break;
            }
        }
        gameHandler.updateState(message);
    }

    /**
     * Animationsfunktion spezifiziert für GadgetActions
     * Mittels Switch Case Abfrage
     */
    private IEnumerator animateGadgetActions(GadgetAction operation, Point target)
    {
        switch (operation.gadget)
        {
            case GadgetEnum.HAIRDRYER:
                yield return Throwing.Hairdryer_Animation(char_table[operation.characterId], target.x, -target.y);
                break;
            case GadgetEnum.MOLEDIE:
                yield return Throwing.Moledie_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.TECHNICOLOUR_PRISM:
                yield return Throwing.Prism_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.BOWLER_BLADE:
                if (operation.successful)
                {
                    yield return Throwing.Hat_Animation(target.x, -target.y, char_table[operation.characterId]);
                }
                break;
            case GadgetEnum.POISON_PILLS:
                yield return Throwing.PoisonPills_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.LASER_COMPACT:
                yield return Throwing.Laser_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.ROCKET_PEN:
                yield return Throwing.Rocket_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.GAS_GLOSS:
                yield return Throwing.GasGloss_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.MOTHBALL_POUCH:
                yield return Throwing.Mothball_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.FOG_TIN:
                yield return Throwing.FogTin_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.GRAPPLE:
                yield return Throwing.Grapple_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.JETPACK:
                if (operation.successful)
                {
                    yield return Jetpack_Animation(Point.point_to_vector3int(target), char_table[operation.characterId]);
                }
                else
                {
                    yield return JetpackFail_Animation(char_table[operation.characterId]);
                }
                break;
            case GadgetEnum.CHICKEN_FEED:
                yield return Throwing.ChickenFeed_Animation(target.x, -target.y, char_table[operation.characterId]);
                break;
            case GadgetEnum.NUGGET:
                yield return Throwing.Nugget_Animation(target.x, -target.y, char_table[operation.characterId]);
                //nachtraegliche Effekte vom Animation (falls operation erfolgreich)
                if (!isSpectatorScreen) // falls client spieler
                {
                    if (operation.successful && gameHandler.ExistsCharacter(target) != null && gameHandler.IsOwnFractionMember(operation.characterId))
                    {
                        //Agent in eigener Fraktion einfügen und im View als Agent markieren
                        gameHandler.addToFraction(gameHandler.ExistsCharacter(target).getGuid());
                        markAgent(gameHandler.ExistsCharacter(target).getGuid(), false);
                    }
                }
                else // falls client zuschauer
                {
                    if (operation.successful && gameHandler.ExistsCharacter(target) != null)
                    {
                        // falls Taeter aus P1 Fraktion
                        if (gameHandler.IsOwnFractionMember(operation.characterId)){
                            //Agent in p1 Fraktion einfügen und im View als p1 Agent markieren
                            gameHandler.addToFraction(gameHandler.ExistsCharacter(target).getGuid());
                            markAgent(gameHandler.ExistsCharacter(target).getGuid(), false);
                        }
                        // falls Taeter aus P2 Fraktion
                        else if (gameHandler.IsEnemyFractionMember(operation.characterId))
                        {
                            //Agent in p2 Fraktion einfügen und im View als p2 Agent markieren
                            gameHandler.addToEnemyFraction(gameHandler.ExistsCharacter(target).getGuid());
                            markAgent(gameHandler.ExistsCharacter(target).getGuid(), true);
                        }
                    }
                }
                break;
            case GadgetEnum.MIRROR_OF_WILDERNESS:
                Character target_ch = gameHandler.ExistsCharacter(target);
                //Animation davon abhaengig ob die Spiegel zerbricht
                bool mirror_break = target_ch != null && operation.successful && !gameHandler.IsOwnFractionMember(target_ch);
                yield return Throwing.Mirror_Animation(target.x, -target.y, char_table[operation.characterId], mirror_break);
                break;
            case GadgetEnum.COCKTAIL: //collecting is situational change
                Cocktail c = gameHandler.getCharacterById(operation.characterId).hasGadget(GadgetEnum.COCKTAIL) as Cocktail;
                if (c != null)
                {
                    if (target.EqualsPoint(gameHandler.getCharacterById(operation.characterId).getCoordinates())) // if drinking
                    {
                        yield return Throwing.Drink_Animation(char_table[operation.characterId], c.isPoisoned);
                    }
                    else //if spilling
                    {
                        GameTile targetTile = (GameTile)tilemap.GetTile(new Vector3Int(target.x, -target.y, 0));
                        GameObject target_go = targetTile != null ? targetTile.dynamicGameObject : null;
                        Cocktail cocktail = (Cocktail)gameHandler.getCharacterById(operation.characterId).hasGadget(GadgetEnum.COCKTAIL);
                        yield return Throwing.SpillCocktail_Animation(char_table[operation.characterId], target.x, -target.y, cocktail.isPoisoned);
                    }
                }
                break;
            // passive gadgets
            case GadgetEnum.MAGNETIC_WATCH:
            case GadgetEnum.POCKET_LITTER:
            default:
                break;
        }
    }

    /**
     * Animationsfunktion spezifiziert für PropertyActions
     * Mittels Switch Case Abfrage
     */
    private IEnumerator animatePropertyActions(PropertyAction operation)
    {
        switch (operation.usedProperty)
        {
            case PropertyEnum.BANG_AND_BURN:
                yield return Throwing.BangAndBurn_Animation(char_table[operation.characterId], operation.target.x, -operation.target.y);
                break;
            case PropertyEnum.OBSERVATION:
                yield return Throwing.Observation_Animation(char_table[operation.characterId], operation.target.x, -operation.target.y);
                // bei erfolgreicher Observation auf Gegner, Agenten zum gegnerischen Fraktion einfügen (nur wenn Spieler)
                if (!isSpectatorScreen && operation.successful && operation.isEnemy && gameHandler.IsOwnFractionMember(operation.characterId))
                {
                    gameHandler.addToEnemyFraction(gameHandler.ExistsCharacter(operation.target).getGuid());
                    markAgent(gameHandler.ExistsCharacter(operation.target).getGuid(), true);
                }
                break;
            default:
                break;
        }
    }

    /**
     * Diese Methode spielt die Bewegungsanimation des Prefabs in die berechnete Richtung
     * (Berechnung to-from - bestimmte string - e.g. walk_up)
     * Dann wird der Charakter auf dem Feld als dynamicGameObject des Tiles gesetzt
     * s. GameTile
     * 
     * in Retrospekt haette man Berechnung über aktuelle Char machen können (vllt sollte man) 
     */
    public IEnumerator moveCharacter(Vector3Int from, Vector3Int to)
    {
        // zum Tilemapkoordinaten umrechnen
        from = Vector3Int.Scale(from, new Vector3Int(1, -1, 1));
        to = Vector3Int.Scale(to, new Vector3Int(1, -1, 1));
        // GameTile Objekte
        GameTile tile_from = (GameTile)tilemap.GetTile(from);
        GameTile tile_to = (GameTile)tilemap.GetTile(to);
        // dynamicGameObjekte auf den Tiles
        GameObject character = tile_from.dynamicGameObject;
        GameObject character_switch = tile_to.dynamicGameObject;

        if (character != null) // falls Charakter aufm from Tile
        {
            // Animator state auf default setzen - DEFAULT
            Animator animator = character.GetComponent<Animator>();
            animator.SetBool("stand", true);
            animator.SetBool("cocktail", false);
            // getAnimDirection - Helferfkt (s. Funktionskommentare)
            string animation_direction = getAnimDirection(from, to);
            // automatisches Rechnen ob Animation geflipped wird
            character.transform.rotation = rotate(animation_direction);
            // triggern
            animator.SetTrigger(animation_direction);

            
            Animator counter_animator = null;
            string counter_animation_direction = "";
            //falls ein Char aufm Zielfeld ist
            if (character_switch != null)
            {
                counter_animator = character_switch.GetComponent<Animator>();
                counter_animation_direction = getAnimDirection(to, from);
                if (!character_switch.CompareTag("NPC")) // fix for wrong cat movements
                {
                    // DEFAULT für 2. Char
                    counter_animator.SetBool("stand", true);
                    counter_animator.SetBool("cocktail", false);
                    // je nach animationsrichtung flippen
                    character_switch.transform.rotation = rotate(counter_animation_direction);
                }
                //triggern
                counter_animator.SetTrigger(counter_animation_direction);
            }

            // 1 Sekunde abwarten - Animationen kürzer als 1 Sek
            yield return new WaitForSeconds(1);
            // trigger reseten
            animator.ResetTrigger(animation_direction);
            // flipping zurücksetzen
            character.transform.rotation = rotate("");

            // gleiche für andere Char (aufm Zielfeld)
            if (counter_animator != null)
            {
                counter_animator.ResetTrigger(counter_animation_direction);
                character_switch.transform.rotation = rotate("");
            }

            // tausche dynamicGameObjects der beiden Tiles - the old switcheroo as one may call it
            tile_to.setDynamicGameObject(ref character);
            tile_from.setDynamicGameObject(ref character_switch);
        }
    }

    /**
     * Diese Methode spielt die Bewegungsanimation der Katze in die berechnete Richtung
     * Berechnung analog wie oben
     * Katze wird dann als dynamicGameObject auf dem Feld gesetzt
     */
    public IEnumerator moveCat(Vector3Int to, bool collarOnTargetTile)
    {
        // finde Katze im Map
        GameObject cat = GameObject.Find("Katze");
        // Koordinaten als Vector3Int (SOLLTE idR den Tile representieren, auf der die Katze sich befindet)
        Vector3Int from = vector3_to_vector3int(cat.transform.localPosition);
        // Map Koordinaten von Zielfeld
        to = Vector3Int.Scale(to, new Vector3Int(1, -1, 1));
        // GameTiles
        GameTile tile_from = (GameTile)GameScreen.tilemap.GetTile(from);
        GameTile tile_to = (GameTile)GameScreen.tilemap.GetTile(to);
        // Charakter auf dem Zielfeld (kann null sein)
        GameObject char_switch = tile_to.dynamicGameObject;

        // Spiele Animation
        Animator animator = cat.GetComponent<Animator>();
        // getAnimDirection - helper method (see. method explanation)
        string animation_direction = getAnimDirection(from, to);
        // triggern
        animator.SetTrigger(animation_direction);

        // für Charakter auf dem Zielfeld - analog wie im moveCharacter
        Animator counter_animator = null;
        string counter_animation_direction = "";
        if (char_switch != null)
        {
            counter_animator = char_switch.GetComponent<Animator>();
            if (!char_switch.CompareTag("NPC"))
            {
                counter_animator.SetBool("stand", true);
                counter_animator.SetBool("cocktail", false);
            }
            counter_animation_direction = getAnimDirection(to, from);
            char_switch.transform.rotation = rotate(counter_animation_direction);

            counter_animator.SetTrigger(counter_animation_direction);
        }

        // 1 Sek abwarten bis Anims fertig sind (beide Animationen müssen kürzer sein als 1 Sek)
        // Das sollte meine Meinung nach standardisiert werden
        yield return new WaitForSeconds(1);
        // Trigger resetten
        animator.ResetTrigger(animation_direction);

        if (counter_animator != null)
        {
            counter_animator.ResetTrigger(counter_animation_direction);
            char_switch.transform.rotation = rotate("");
        }

        // the old switcheroo aus dem moveCharacter
        tile_to.setDynamicGameObject(ref cat);
        tile_from.setDynamicGameObject(ref char_switch);

        // Falls Katze eine herumliegende Diamanthalsband findet
        // Spiele jump animation (sieht man nie, man geht sofort in Winner Canvas)
        if (collarOnTargetTile)
        {
            animator.SetTrigger("jump");
            yield return new WaitForSeconds(0.35f);
            animator.ResetTrigger("jump");
        }
    }

    /**
     * Diese Methode spielt die Teleoport Animation des Hausmeisters und löscht den Charakter
     * auf der Stelle wo Hausmeister landet.(Destroy auf das Char GameObject)
     */
    public IEnumerator moveJanitor(Vector3Int to)
    {
        // Finde Hausmeister Objekt im Spiel
        GameObject janitor = GameObject.Find("Hausmeister");
        Vector3Int from = vector3_to_vector3int(janitor.transform.localPosition);
        to = Vector3Int.Scale(to, new Vector3Int(1, -1, 1));
        // von Tile, nach Tile
        GameTile tile_from = (GameTile)GameScreen.tilemap.GetTile(from);
        GameTile tile_to = (GameTile)GameScreen.tilemap.GetTile(to);

        // Animation Spielen
        Animator animator = janitor.GetComponent<Animator>();

        // CORRECTION MAYBE NEEDED
        animator.SetTrigger("teleport1");
        yield return new WaitForSeconds(1f);
        animator.ResetTrigger("teleport1");
        janitor.transform.localPosition = to;
        animator.SetTrigger("teleport2");
        yield return new WaitForSeconds(0.67f);
        animator.ResetTrigger("teleport2");

        /* 
         * NOT ENOUGH TIME FOR THIS SO TELEPORT TO LOCATION
         * ursprünglich war die Idee, dass sich der Hausmeister den Pfad findet und 
         * über Bewegungseinheiten hingeht
         * Das würde aber zu lange dauern.
         * Man bekommt sicherlich inzwischen weitere Nachrichten
         */
        /*
        string[] animation_directions = pathFinder(from, to);

        foreach(string s in animation_directions)
        {
            animator.Play(s, 0);
            // get animator state
            AnimatorStateInfo anim_state = animator.GetCurrentAnimatorStateInfo(0);

            if (anim_state.IsName(s))
            {
                yield return new WaitForSeconds(anim_state.length);
            }
            else // mostly this part runs
            {
                // wait 1 second, walk animations must be shorter than 1 sec.
                yield return new WaitForSeconds(0.5f);
            }
        }
        */

        // Lösche GameObject auf Tile to, setze Hausmeister auf tile to (als dynGameObj)
        Destroy(tile_to.dynamicGameObject);
        tile_to.setDynamicGameObject(ref janitor);
        GameObject NullPointer = null;
        tile_from.setDynamicGameObject(ref NullPointer);
    }

    /**
     * Diese Methode spielt die Exfiltrationsanimation der Agenten und teleportiert ihn auf den
     * Sitzplatz.
     * Dabei fehlt aber Bewegung der Agenten der sich ursprünglich am Zielfeld der Exfiltraion befunden hatte
     * Möglich dass es zu falscher Anzeige von Spielstate führt.
     */
    public IEnumerator exfiltrateCharacter(Vector3Int from, Vector3Int to)
    {
        from = Vector3Int.Scale(from, new Vector3Int(1, -1, 1));
        to = Vector3Int.Scale(to, new Vector3Int(1, -1, 1));
        GameTile tile_from = (GameTile)tilemap.GetTile(from);
        GameTile tile_to = (GameTile)tilemap.GetTile(to);
        // get objects at initial and target tiles
        GameObject character = tile_from.dynamicGameObject;
        GameObject character_switch = tile_to.dynamicGameObject; // nie benutzt Logik fehlt

        if (character != null)
        {
            // DEFAULT
            Animator animator = character.GetComponent<Animator>();
            animator.SetBool("stand", true);
            animator.SetBool("cocktail", false);

            // Animation triggern - 2 teilig
            animator.SetTrigger("exfiltration");
            yield return new WaitForSeconds(1);
            animator.ResetTrigger("exfiltration");
            character.transform.localPosition = to;
            animator.SetTrigger("exfiltration2");
            yield return new WaitForSeconds(1);
            animator.ResetTrigger("exfiltration");

            // assuming we locate to a FREE chair
            // OTHERWISE WRONG
            GameObject NullPointer = null;
            tile_to.setDynamicGameObject(ref character);
            tile_from.setDynamicGameObject(ref NullPointer);
        }
    }

    /**
    Diese Methode triggert die Jetpack Animation und befördert den Agenten zu seiner neuen Position
    Update: Funktioniert fast gleich wie Exfiltration mit unterschiedlichen Animation
    **/
    public IEnumerator Jetpack_Animation(Vector3Int to, GameObject agent)
    {
        Vector3Int from = vector3_to_vector3int(agent.transform.localPosition);
        to = Vector3Int.Scale(to, new Vector3Int(1, -1, 1));

        Animator anim = agent.GetComponent<Animator>();

        anim.SetTrigger("jetpack");
        yield return new WaitForSeconds(1);
        anim.ResetTrigger("jetpack");

        //Relocate Agenten
        agent.transform.position = to;

        //2. teil der Animation
        anim.SetTrigger("jetpack2");
        yield return new WaitForSeconds(0.67f);
        anim.ResetTrigger("jetpack2");

        // setze Charakter auf Zieltile
        GameTile fromTile = (GameTile)tilemap.GetTile(from);
        GameTile totile = (GameTile)tilemap.GetTile(to);
        GameObject Nullpointer = null;
        totile.setDynamicGameObject(ref agent);
        fromTile.setDynamicGameObject(ref Nullpointer);

    }

    /**
     * Animation, falls Jetpack Aktion fehlschlaegt. Passiert anscheinend nie
     * kein Feldwechsel -> kein Verweis
     */
    public IEnumerator JetpackFail_Animation(GameObject agent)
    {
        Animator anim = agent.GetComponent<Animator>();

        anim.SetTrigger("jetpack_fail");
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("jetpack_fail");
    }

    /**
     * Zustandsupdates nach dem alle Animationen durch sind
     * Ziel: Falls bei Animationen was schief laeuft, dass man sich hier zum state anpasst
     * Realitaet: Zum grossen Teil passts, aber manchmal erzeugt Bugs, die zur falscher Representierung des Feldes führt
     * Vielleicht könnte an einige Stellen besser implementiert sein
     */
    private bool firstUpdateDone; // Flag für die 1. Aufruf der Methode (wichtig in Maps)

    /**
     * Aktualisiert das Spielfeld (nimmt vor die Effekte, die durch Opeationen erschienen sind)
     * Ex. Wand zerstört nach Raketenfüllfeder Aktion (falls State ein freies Feld hat, wo im Map ein Wand ist
     * wird die Wand weggemacht)
     */
    public void updateMap(Field[,] map)
    {
        GameObject go;
        //iteriere die Zeilen
        for (int row = 0; row < map.GetLength(0); row++)
        // row go in y direction
        {
            //iteriere die Spalten
            for (int col = 0; col < map.GetLength(1); col++)
            // columns go in x direction
            {
                //beim ersten Update: sorting nummer der statischen Gameobjekte gesetzt
                if (!firstUpdateDone && tilemap.GetInstantiatedObject(new Vector3Int(col, -row, 0)) != null)
                {
                    tilemap.GetInstantiatedObject(new Vector3Int(col, -row, 0)).GetComponent<SpriteRenderer>().sortingOrder = row + 1;
                }
                // beim ersten Update: Tresornummern auf den Tresoren gesetzt
                if (!firstUpdateDone && map[row, col].getFieldStateEnum() == FieldStateEnum.SAFE)
                {
                    go = tilemap.GetInstantiatedObject(new Vector3Int(col, -row, 0)) as GameObject;
                    go.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/numbers")[map[row, col].getSafeIndex()];
                    go.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sortingOrder = row + 2;
                }

                // JEDES MAL
                GameTile tile = (GameTile)GameScreen.tilemap.GetTile(new Vector3Int(col, -row, 0));
                // falls Wand zu einem freien Feld wird
                if (map[row, col].getFieldStateEnum() != tile.GetStateEnum())
                {
                    // setze Tile info neu
                    setTileInfo(map[row, col].getFieldStateEnum(), row, col, ref tile, ref tile.staticGameObject);
                    tilemap.RefreshTile(new Vector3Int(col, -row, 0));
                }
                // Bar Tische, falls Cocktail drüber erscheinen aendere Tisch sprite (Cocktail auf Tische sind keine Objekte)
                // Das hatte Game Tile Logik noch komplizierter gemacht als sie ist
                if (map[row, col].getFieldStateEnum() == FieldStateEnum.BAR_TABLE)
                {
                    go = tilemap.GetInstantiatedObject(new Vector3Int(col, -row, 0));
                    if (map[row, col].GetGadget() != null && map[row, col].GetGadget().gadget == GadgetEnum.COCKTAIL)
                    {
                        Cocktail c = map[row, col].GetGadget() as Cocktail;
                        if (c != null && c.isPoisoned) // vergiftete Cocktail Sprite
                        {
                            go.GetComponentInChildren<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/tisch_cp")[0];
                        }
                        else // Cocktail Sprite
                        {
                            go.GetComponentInChildren<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/tisch_c")[0];
                        }
                    }
                    else // ohne Cocktail
                    {
                        go.GetComponentInChildren<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/tisch")[0];
                    }
                }
                // Falls Tile Nebel hat, setze Nebel
                if (map[row, col].getIsFoggy())
                {
                    tile.setFog(true);
                }
                else // sonst weg tun
                {
                    tile.setFog(false);
                }

                // falls roulette Tisch invertiert oder destroyed
                if (map[row, col].getFieldStateEnum() == FieldStateEnum.ROULETTE_TABLE)
                {
                    go = tilemap.GetInstantiatedObject(new Vector3Int(col, -row, 0));
                    if (map[row, col].getIsDestroyed()) // destroyed - bisschen dunklere Sprite
                    {
                        if (go.GetComponent<SpriteRenderer>().sprite != Resources.LoadAll<Sprite>("Sprites/roulette_short_bad")[0])
                        {
                            go.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/roulette_short_bad")[0];
                        }

                    }
                    else if (map[row, col].getIsInverted()) // inveriert - geeignete Sprite laden
                    {
                        if (go.GetComponent<SpriteRenderer>().sprite != Resources.LoadAll<Sprite>("Sprites/roulette_short_inv")[0])
                        {
                            go.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/roulette_short_inv")[0];
                        }
                    } // sonst normale Sprite laden
                    else if (go.GetComponent<SpriteRenderer>().sprite != Resources.LoadAll<Sprite>("Sprites/roulette_short")[0])
                    {
                        go.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/roulette_short")[0];
                    }
                }

                // falls Tile einen Gadget hat, Gadget Attribut setzen
                GameTile curr_tile = (GameTile)tilemap.GetTile(new Vector3Int(col, -row, 0));
                if (map[row, col].getState() == FieldStateEnum.FREE)
                {
                    if (map[row, col].GetGadget() != null) // falls Gadet
                    {
                        switch (map[row, col].GetGadget().gadget)
                        {
                            case GadgetEnum.BOWLER_BLADE: //falls Hut, instanziiere Prefab an der Stelle
                                GameObject bb = Instantiate(Resources.Load("Prefabs/Hat")) as GameObject;
                                curr_tile.setGadget(bb);
                                break;
                            case GadgetEnum.DIAMOND_COLLAR: // falls Diamanthalsband, instanziiere Prefab an der Stelle
                                GameObject dc = Instantiate(Resources.Load("Prefabs/diamond_necklace")) as GameObject;
                                curr_tile.setGadget(dc);
                                break;
                        }
                    }
                    else // falls kein Gadget aufm Feld, destroy die existierende
                    {
                        curr_tile.destroyGadget();
                    }
                }
            }
        }

        // nach dem 1. Lauf, setzt flag hoch und mach den Loading Panel weg
        if (!firstUpdateDone)
        {
            firstUpdateDone = true;
        }
        if (loading_panel.activeInHierarchy)
        {
            loading_panel.SetActive(false);
        }
    }

    /**
     * Aktualisiert die Charaktere nach dem geschickten State
     */
    public void updateChars(HashSet<Character> chars, Vector3Int cat, Vector3Int? janitor)
    {
        GameObject ch_switch;
        HashSet<Character>.Enumerator enu = chars.GetEnumerator();
        //iteriere Charakter Liste
        while (enu.MoveNext())
        {
            Character c = enu.Current;
            if (!char_table.ContainsKey(c.getGuid())) // falls Charakter nicht im ID, GameObject Hashset ist, erzeuge ihn
            {
                //immer gleiche Prefab
                GameObject char_go = instantiate_character("magent",
                    Point.point_to_vector3int(c.getCoordinates()),
                    c.getName(), c.getGuid(), gameHandler.getFraction().Contains(c.getGuid()));
                // falls Klamme Klamotten setze Tropfen auf ihn
                checkPropertyConsequences(c, char_go);
                continue;
            }
            // wenn schon instantiiert, schaue ob auf der richtigen Stelle
            GameObject ch = char_table[c.getGuid()];
            Vector3Int charFromCoordinates = vector3_to_vector3int(ch.transform.localPosition);
            Vector3Int charToCoordinates = Vector3Int.Scale(Point.point_to_vector3int(c.getCoordinates()), new Vector3Int(1, -1, 1));
            GameTile charFrom = (GameTile)GameScreen.tilemap.GetTile(charFromCoordinates);
            GameTile charTo = (GameTile)GameScreen.tilemap.GetTile(charToCoordinates);
            //falls nicht auf richtigen Stelle, setze Charakter auf die richtige Stelle (hat kleine Bugs)
            //Setzen über dynamicGameObj referenzen der Tiles
            if (!charFromCoordinates.Equals(charToCoordinates))
            {
                Debug.Log("ERROR: " + charFromCoordinates + " and " + charToCoordinates);
                ch.GetComponent<SpriteRenderer>().sortingOrder = c.getCoordinates().y + 1;
                ch_switch = charTo.dynamicGameObject;
                charTo.setDynamicGameObject(ref charFrom.dynamicGameObject);
                charFrom.setDynamicGameObject(ref ch_switch);
            }

            // unterschiedliche Animationsclips (zur Sprite anzeige) über Flags aufrufen
            // Standing w/ [psn] Cocktail, Sitting [w/, w/o] [psn] Cocktail
            // detaillierter erklaert auf Zeile 383
            Animator ch_animator = ch.GetComponent<Animator>();
            ch_animator.SetBool("stand", charTo.GetStateEnum() != FieldStateEnum.BAR_SEAT);
            Gadget cocktail_gadget = c.hasGadget(GadgetEnum.COCKTAIL);
            ch_animator.SetBool("cocktail", cocktail_gadget != null);
            if (cocktail_gadget != null)
            {
                Cocktail cocktail = c.hasGadget(GadgetEnum.COCKTAIL) as Cocktail;
                ch_animator.SetBool("poison", cocktail.isPoisoned);

            }

            // falls klamme Klamotten oder konst klamme Klamotten, setze Tropfen auf ihn an
            checkPropertyConsequences(c, ch);
            // falls Wanze und Ohrstoepsel equipped und working Kopfhörer an
            checkGadgetConsequences(c, ch);
        }

        // KATZE
        GameObject katze = GameObject.Find("Katze");
        if (katze == null) //falls nicht auf dem Feld instantiiere
        {
            instantiate_special_chars("katze", cat, "Katze");
        }
        else // sonst schaue ob auf der richtigen Position
        {
            Vector3Int catFromCoordinates = vector3_to_vector3int(katze.transform.localPosition);
            Vector3Int catToCoordinates = Vector3Int.Scale(cat, new Vector3Int(1, -1, 1));
            //wenn auf falscher Position setze sie auf richte und tausche Plaetze mit dem Agenten
            // NEU: anstatt sofort platze zu tauschen sucht man jetzt ob jemand sich am der point coordinaten
            // der Agenten befindet. Falls ja tauscht Charakter Platze mit Katze und Zustand wird falsch angezeigt
            // aber falls keiner an seinen Koordinaten ist, wird der Charakter da gestellt
            if (!catFromCoordinates.Equals(catToCoordinates))
            {
                Debug.Log("CAT ERROR: " + catFromCoordinates + " and " + catToCoordinates);
                GameTile catFrom = (GameTile)GameScreen.tilemap.GetTile(catFromCoordinates);
                GameTile catTo = (GameTile)GameScreen.tilemap.GetTile(catToCoordinates);
                ch_switch = catTo.dynamicGameObject;
                catTo.setDynamicGameObject(ref catFrom.dynamicGameObject);
                // NEU
                GameObject Nullpointer = null;
                catFrom.setDynamicGameObject(ref Nullpointer);
                if (char_table.ContainsValue(ch_switch))
                {
                    bool char_set_to_pos = false;
                    Guid ch_switch_id = Guid.Empty;
                    foreach(KeyValuePair<Guid, GameObject> pair in char_table)
                    {
                        if (pair.Value.Equals(ch_switch))
                        {
                            ch_switch_id = pair.Key;
                        }
                    }
                    if (ch_switch_id.Equals(Guid.Empty))
                    {
                        Character ch_switch_char = gameHandler.getCharacterById(ch_switch_id);
                        if(ch_switch_char != null)
                        {
                            Vector3Int ch_switch_pos = Vector3Int.Scale(Point.point_to_vector3int(ch_switch_char.getCoordinates()), new Vector3Int(1, -1, 1));
                            GameTile ch_switch_tile = (GameTile) tilemap.GetTile(ch_switch_pos);
                            if(ch_switch_tile.dynamicGameObject == null)
                            {
                                ch_switch_tile.setDynamicGameObject(ref ch_switch);
                                char_set_to_pos = true;
                            }
                        }
                    }
                    if (!char_set_to_pos) // kann fehlerhaft sein
                    {
                        catFrom.setDynamicGameObject(ref ch_switch);
                    }
                }
                // END NEU

                //catFrom.setDynamicGameObject(ref ch_switch);
            }
        }
        if (janitor != null) // falls innerhalb überlanger Spielzeit
        {
            Vector3Int janitor_val = (Vector3Int)janitor;
            GameObject hausmeister = GameObject.Find("Hausmeister");
            if (hausmeister == null) // falls kein Hausmeister -> instantiiere ihn
            {
                // lösche alle NPCs (wenn Hausmeister da ist, muss alle Agenten der beiden Fraktionen klar sein)
                // wird auch markiert
                // restlichen sind NPC und werden hier gelöscht
                foreach (KeyValuePair<Guid, GameObject> entry in char_table)
                {
                    if (gameHandler.getCharacterById(entry.Key) == null)
                    {
                        Destroy(entry.Value);
                    }
                }
                instantiate_special_chars("Hausmeister", janitor_val, "Hausmeister"); //instantiiere Hausmeister
            }
            else // falls Hausmeister schon da
            {
                Vector3Int hmFromCoordinates = vector3_to_vector3int(hausmeister.transform.localPosition);
                Vector3Int hmToCoordinates = Vector3Int.Scale(janitor_val, new Vector3Int(1, -1, 1));

                // lösche GameObjekte der Charaktere, die nicht im Char List des States waren
                foreach (KeyValuePair<Guid, GameObject> entry in char_table)
                {
                    if (gameHandler.getCharacterById(entry.Key) == null)
                    {
                        Destroy(entry.Value);
                    }
                }

                // falls hausmeister auf der falschen Stelle
                // tu ihn auf den richtigen Tile setzen und falls ein GameObject da ist, löschen
                if (!hmFromCoordinates.Equals(hmToCoordinates)) 
                {
                    Debug.Log("HAUSMEISTER ERROR: " + hmFromCoordinates + " and " + hmToCoordinates);
                    GameTile hmFrom = (GameTile)GameScreen.tilemap.GetTile(hmFromCoordinates);
                    GameTile hmTo = (GameTile)GameScreen.tilemap.GetTile(hmToCoordinates);
                    ch_switch = hmTo.dynamicGameObject;
                    Destroy(ch_switch);
                    hmTo.setDynamicGameObject(ref hmFrom.dynamicGameObject);
                    GameObject Nullpointer = null;
                    hmFrom.setDynamicGameObject(ref Nullpointer);
                }
            }
        }
    }

    /**
     * Falls klamme Klamotten oder konstant Klamme Klamotten, setze Tropfen als ChildGameObjekt
     * (was der GameObjekt des Charakters sein sollte)
     */
    private void checkPropertyConsequences(Character c, GameObject parent_obj)
    {
        GameObject water1, water2;
        if (c.hasProperty(PropertyEnum.CLAMMY_CLOTHES) && parent_obj.transform.Find("WaterTrops(Clone)") == null)
        {
            water1 = Instantiate(Resources.Load("Prefabs/WaterTrops")) as GameObject;
            water1.transform.SetParent(parent_obj.transform);
            water1.transform.localPosition = new Vector3(0, 2.3f, 0);
            water1.GetComponent<SpriteRenderer>().sortingOrder = gameHandler.mapSize.max();
            water1.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
        }
        else if (!c.hasProperty(PropertyEnum.CLAMMY_CLOTHES) && parent_obj.transform.Find("WaterTrops(Clone)") != null)
        {
            water1 = parent_obj.transform.Find("WaterTrops(Clone)").gameObject;
            water1.transform.SetParent(null);
            Destroy(water1);
        }

        if (c.hasProperty(PropertyEnum.CONSTANT_CLAMMY_CLOTHES) && parent_obj.transform.Find("WaterTrops(Clone)") == null)
        {
            water2 = Instantiate(Resources.Load("Prefabs/WaterTrops")) as GameObject;
            water2.transform.SetParent(parent_obj.transform);
            water2.transform.localPosition = new Vector3(0, 2.3f, 0);
            water2.GetComponent<SpriteRenderer>().sortingOrder = gameHandler.mapSize.max();
            water2.GetComponent<SpriteRenderer>().color = new Color(0, 0.2f, 0.5f, 0.5f);
        }
        else if (!c.hasProperty(PropertyEnum.CONSTANT_CLAMMY_CLOTHES) && parent_obj.transform.Find("WaterTrops(Clone)") != null)
        {
            water2 = parent_obj.transform.Find("WaterTrops(Clone)").gameObject;
            water2.transform.SetParent(null);
            Destroy(water2);
        }
    }

    /**
     * Falls Charakter mit Wanze und Ohrstöpsel ausgerüstet ist und es funktioniert (working=true)
     * füge Kopfhörer game object als Child GameObjekt an dem Agenten Objekts
     * Falls nicht mehr working, destroy das Child Game Objekt
     */
    private void checkGadgetConsequences(Character c, GameObject parent_obj)
    {
        GameObject earplugs;
        if (c.hasGadget(GadgetEnum.WIRETAP_WITH_EARPLUGS) != null) //earplugs
        {
            WiretapWithEarplugs plugs = c.hasGadget(GadgetEnum.WIRETAP_WITH_EARPLUGS) as WiretapWithEarplugs;
            if (plugs.working && parent_obj.transform.Find("Ohrstopsel(Clone)") == null)
            {
                earplugs = Instantiate(Resources.Load("Prefabs/Ohrstopsel")) as GameObject;
                earplugs.transform.SetParent(parent_obj.transform);
                earplugs.transform.localPosition = new Vector3(0, 2.15f, 0);
                earplugs.GetComponent<SpriteRenderer>().sortingOrder = gameHandler.mapSize.max();
            }
            else if (!plugs.working && parent_obj.transform.Find("Ohrstopsel(Clone)") != null)
            {
                earplugs = parent_obj.transform.Find("Ohrstopsel(Clone)").gameObject;
                earplugs.transform.SetParent(null);
                Destroy(earplugs);
            }

        }
        else
        {
            if (parent_obj.transform.Find("Ohrstopsel(Clone)") != null)
            {
                earplugs = parent_obj.transform.Find("Ohrstopsel(Clone)").gameObject;
                earplugs.transform.SetParent(null);
                Destroy(earplugs);
            }
        }
    }

    /**
     * der Marker des aktiven Charakters gelb (aktiv) oder grün (inaktiv) faerben
     * wird von requestgameOperation Nachricht auf aktiv gesetzt und von sendGameOperation
     * oder Strike zurück
     */
    public void markActiveChar(Guid activeGuid, bool activating)
    {
        GameObject activeGameObject = char_table[activeGuid];
        GameObject marker = activeGameObject.transform.Find("marker(Clone)").gameObject;
        if (marker != null)
        {
            if (activating)
            {
                marker.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else
            {
                marker.GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.65f, 0);
            }
        }
    }

    /**
     * Auf dem GameObjekt entsprechend des IDs (in char_table HashSet)
     * wird einen Marker gesetzt (kann aus eigenen oder gegnerische Fraktion sein)
     * @param isEnemy gibt an ob Agenten ein Gegner Agenten ist (wenn ja setzt rote marker)
     * sonst setze grün marker
     */
    public void markAgent(Guid agentId, bool isEnemy)
    {
        if (char_table.ContainsKey(agentId))
        {
            GameObject enemyAgent = char_table[agentId];
            GameObject marker = Instantiate(Resources.Load("Prefabs/marker")) as GameObject;
            marker.GetComponent<SpriteRenderer>().color = isEnemy ? Color.red : new Color(0.1f, 0.65f, 0);
            marker.GetComponent<SpriteRenderer>().sortingOrder = gameHandler.mapSize.max();
            marker.transform.SetParent(enemyAgent.transform);
            marker.transform.localPosition = new Vector3(0, 2.55f, 0);
        }
    }

    /**
     * Transparenz im Map
     * i.d.R. falls ein Objekt die Chance hat ein hinten stehender Objekt auszublenden
     * wird er durchsichtig gemacht (damit kleine Charaktere auch sehbar sind z.B. Katze hinter Kamin,
     * oder Cocktail auf einem Tisch hinter einem Charakter)
     * Wird nach jedem Map update aufgerufen
     * Bugs:
     * 1) falls ein Charakter durchsichtig wird wenn Pointer auf ihm ist, bleibt er solange gelb  bis er nicht mehr transparent ist
     * 2) Wenn ein zerstörter Roulette Tisch einmal durchsichtig wird, bekommt sie spaeter Farbe der aktiven Roulette Tische (Tisch bleibt aber weiterhin inaktiv)
     * 
     */
    public void setTransparency(int trow, int tcol)
    {
        for (int row = 1; row < trow; row++)
        {
            for (int col = 0; col < tcol; col++)
            {
                GameObject go1 = tilemap.GetInstantiatedObject(new Vector3Int(col, -row, 0));
                GameTile currTile = (GameTile)tilemap.GetTile(new Vector3Int(col, -row, 0));
                GameObject go2 = currTile.dynamicGameObject;
                GameTile upperTile = (GameTile)tilemap.GetTile(new Vector3Int(col, -(row - 1), 0));
                GameObject goa = upperTile.dynamicGameObject;
                if (go1 != null) // statische Game Objekte - falls dahinter ein Spieler -> durchsichtig
                {
                    if (goa != null && (goa.CompareTag("Player") || goa.CompareTag("NPC")))
                    {
                        go1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
                    }
                    else //sonst nicht durchsichtig (Bug 2)
                    {
                        go1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    }
                }
                if (go2 != null && go2.CompareTag("Player")) // Agentenobjekte - falls dahinter Charakter oder Safe oder Bartisch
                {
                    if ((goa != null && (goa.CompareTag("Player") || goa.CompareTag("NPC")))
                        || (upperTile.GetStateEnum() == FieldStateEnum.BAR_TABLE || upperTile.GetStateEnum() == FieldStateEnum.SAFE))
                    {
                        // Bug 1 - wenn Pointer drauf aktive Farbe gelb
                        go2.GetComponent<SpriteRenderer>().color = new Color(go2.GetComponent<SpriteRenderer>().color.r,
                            go2.GetComponent<SpriteRenderer>().color.g,
                            go2.GetComponent<SpriteRenderer>().color.b, 0.7f);
                    }
                    else // nicht durchsichtig (ursprüngliche Farbe aus dem Color Dictionary zurücksetzen)
                    {
                        int go2_org_index = GetDictIndex(go2);
                        if (go2_org_index >= 0)
                        {
                            go2.GetComponent<SpriteRenderer>().color = char_colors.ElementAt(go2_org_index).Value;
                        }
                        else // indicates: THIS HERE IS AN ERROR - SHOULD NEVER HAPPEN
                        {
                            go1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                        }
                    }
                }
            }
        }
    }

    /**
     * Helferfunktion um ursprüngliche Farben der Agenten rauszulesen
     * Findet in char_table das Objekt -> gleiche Index muss auch im Color Dictionary sein
     */
    private static int GetDictIndex(GameObject g)
    {
        for (int i = 0; i < char_table.Count; i++)
        {
            if (char_table.ElementAt(i).Value.Equals(g))
            {
                return i;
            }
        }
        return -1;
    }

    /**
     * Diese Methode gibt einen String zurück, der die Bewegungsrichtung angibt
     * Die Trigger in Animation Controller heissen auch gleich wie diesen Strings
     */
    private static string getAnimDirection(Vector3Int from, Vector3Int to)
    {
        if ((to - from).Equals(new Vector3Int(1, 0, 0))) // right
        {
            return "walk_right";
        }
        else if ((to - from).Equals(new Vector3Int(1, 1, 0))) // up right
        {
            return "walk_up_right";
        }
        else if ((to - from).Equals(new Vector3Int(0, 1, 0))) // up
        {
            return "walk_up";
        }
        else if ((to - from).Equals(new Vector3Int(-1, 1, 0))) // up left
        {
            return "walk_up_left";
        }
        else if ((to - from).Equals(new Vector3Int(-1, 0, 0))) // left
        {
            return "walk_left";
        }
        else if ((to - from).Equals(new Vector3Int(-1, -1, 0))) //down left
        {
            return "walk_down_left";
        }
        else if ((to - from).Equals(new Vector3Int(0, -1, 0))) // down
        {
            return "walk_down";
        }
        else if ((to - from).Equals(new Vector3Int(1, -1, 0))) // down right
        {
            return "walk_down_right";
        }
        return "";
    }

    /**
     * Da viele Animationen die gleichen Sprites als Key Frames benutzen, wurden bei
     * einigen Animation einfach geflipped (rotation um 180 um y Achse
     * Diese Methode entscheidet je nach String was für ein Rotation (flip x oder keine)
     * stattfinden sollte
     */
    private static Quaternion rotate(string direction)
    {
        if (direction.Contains("walk_left")
            || direction.Contains("walk_down_left")
            || direction.Contains("walk_up_right"))
        {
            return Quaternion.Euler(0, 180, 0);
        }
        return Quaternion.Euler(0, 0, 0);
    }

    /**
     * Pfad finder für Hausmeister, nicht implementiert, da Animation (einheitliche Laufanimationen
     * zu lange dauern könnte)
     */
    private static string[] pathFinder(Vector3Int from, Vector3Int to)
    {
        List<string> directions = new List<string>();
        while (!from.Equals(to))
        {
            Vector3Int diff = to - from;
            // TODO: implement the logic
            if (diff.x > 0)
            {

            }
            else if (diff.x == 0)
            {

            }
            else
            {

            }
        }
        return directions.ToArray();
    }

    /**
     * Zeigt Pause Panel an und setzt den Richtigen Sprite für Pause Button je nach
     * pausiert oder nicht
     */
    public void togglePause(bool isPaused)
    {
        if (isPaused)
        {
            GameObject.Find("PauseButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/pause_button2");
            pause_menu.SetActive(true);
        }
        else
        {
            GameObject.Find("PauseButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/pause_button");
            pause_menu.SetActive(false);
        }
    }

    /**
     * Falls Server eine Pause enforct, muss man kein Request schicken können,
     * daher wird pause button deaktiviert
     */
    public void pausebutton_active(bool active)
    {
        GameObject.Find("PauseButton").GetComponent<Button>().interactable = active;
    }

    /**
     * DROP DOWN - wie Spielzüge ausgeführt sind
     */
    /**
     * Dropdown Menu aktivieren (onRequestGameOperation)
     * oder Deaktivieren (sendGameOperation, Strike, Zuschauer)
     */
    public void activateDropdown(bool active)
    {
        dropDown.enabled = active;
    }
    /**
     * Dropdown mit validen Optionen auffüllen
     * Die eigentliche Strings im DropdownTranslation definiert
     */
    public void fillDropdown(Character active_char)
    {
        // Default Aktionen, solange MPs und APs vorliegen, kann jeder Charakter die waehlen
        // validierung folgt im GameHandler
        List<string> to_keep = new List<string>()
        {
            "",
            DropDownTranslation.translate(DropDownOption.Spy_People),
            DropDownTranslation.translate(DropDownOption.Movement),
            DropDownTranslation.translate(DropDownOption.Open_Tresor),
            DropDownTranslation.translate(DropDownOption.Play_Roulette),
            DropDownTranslation.translate(DropDownOption.Collect_Cocktail),
            DropDownTranslation.translate(DropDownOption.Retire)
        };

        //Wenn Aktionspunkte vorhanden
        if (active_char.getAp() > 0)
        {
            if(active_char.getChips() <= 0) // wenn keine Chips -> keine Roulette Aktion
            {
                to_keep.Remove(DropDownTranslation.translate(DropDownOption.Play_Roulette));
            }
            // extra Optionen einfügen
            if (active_char.hasProperty(PropertyEnum.BANG_AND_BURN))
            {
                to_keep.Add(DropDownTranslation.translate(DropDownOption.Bang_and_Burn));
            }
            //Observation nur wenn kein Maulwürfel im Inventar
            if (active_char.hasProperty(PropertyEnum.OBSERVATION) && active_char.hasGadget(GadgetEnum.MOLEDIE) == null)
            {
                to_keep.Add(DropDownTranslation.translate(DropDownOption.Observation));
            }
            // Gadget Aktionen
            HashSet<Gadget>.Enumerator enu = active_char.GetGadgets().GetEnumerator();
            while (enu.MoveNext())
            {
                GadgetEnum g = enu.Current.gadget;
                // wenn gadget aktionsfaehig ist
                if (!(g.Equals(GadgetEnum.MAGNETIC_WATCH) || g.Equals(GadgetEnum.POCKET_LITTER)
                    || g.Equals(GadgetEnum.ANTI_PLAGUE_MASK) || g.Equals(GadgetEnum.DIAMOND_COLLAR))) // Kein-Aktion-Gadgets
                {
                    if (g.Equals(GadgetEnum.COCKTAIL))
                    {
                        to_keep.Add(DropDownTranslation.translate(DropDownOption.Drinking_Cocktail));
                        to_keep.Add(DropDownTranslation.translate(DropDownOption.Spill_Cocktail));
                        to_keep.Remove(DropDownTranslation.translate(DropDownOption.Collect_Cocktail)); //wenn Cocktail im Hand, kein Collect Cocktail möglich
                        continue;
                    }

                    Gadget gadget = enu.Current;
                    if (gadget.usages > 0) // Gadget muss usage > 0 haben
                    {
                        string s = g.ToString();
                        to_keep.Add(DropDownTranslation.translate((DropDownOption)Enum.Parse(typeof(DropDownOption), s)));
                    }
                }
            }
        }
        else // wenn kein AP, nur Bewegen und Beenden möglich
        {
            to_keep.Remove(DropDownTranslation.translate(DropDownOption.Spy_People));
            to_keep.Remove(DropDownTranslation.translate(DropDownOption.Open_Tresor));
            to_keep.Remove(DropDownTranslation.translate(DropDownOption.Play_Roulette));
            to_keep.Remove(DropDownTranslation.translate(DropDownOption.Collect_Cocktail));
        }

        // wenn keine MP keine Bewegung möglich
        if (active_char.getMp() <= 0)
        {
            to_keep.Remove(DropDownTranslation.translate(DropDownOption.Movement));
        }

        dropDown.ClearOptions();
        dropDown.AddOptions(to_keep);
    }

    /**
        Diese Methode gibt den string zurück, welcher im DropDown gewählt wurde
    **/
    public string getSelectedDropDownOption(Dropdown dropDown)
    {
        return dropDown.captionText.text;
    }

    /**
    Diese Methode wird aufgerufen, wenn sich der Wert im Drop Down Menü ändert
    **/
    void DropdownValueChanged(Dropdown change)
    {
        //Buttons Deaktivierung/Aktivierung, je nachdem ob die Option "Bewegung" aktiviert wurde
        if (getSelectedDropDownOption(change).Equals(DropDownTranslation.translate(DropDownOption.Movement)))
        {
            showButtons();
        }
        else
        {
            hideButtons();
        }

        //Aktivierung/Deaktivierung des Einsatz Inputfeldes
        if (getSelectedDropDownOption(change).Equals(DropDownTranslation.translate(DropDownOption.Play_Roulette)))
        {
            roulette_table_commitment.SetActive(true);
        }
        else roulette_table_commitment.SetActive(false);
    }

    //DropdownOption auf leeren String zurückgesetzt, wenn DropdownOption leere String
    //validate nie aufgerufen -> unmöglich Aktion zu senden
    public void resetDropdownChoice()
    {
        dropDown.value = 0;
        dropDown.RefreshShownValue();
    }

    /**
     * Diese Methode aktiviert alle Buttons in der Canvas, und setzt den Flag in Buttons Klasse auf damit
     * selektiv aktivieren möglich ist
     */
    private void showButtons()
    {
        foreach (GameObject go in buttons_gameObjects)
        {
            go.SetActive(true);
        }
        Buttons.good_to_go = true;
    }

    /**
        Diese Methode setzt den Flag in Buttons Skript zurück, damitalle Buttons in der Canvas sich deaktieveren.
    **/
    public void hideButtons()
    {
        Buttons.good_to_go = false;
    }

    /**
     * Text und Kreis in HUD, der angibt ob Spieler dran ist
     * Die Werte davon setzen
     */
    public void toggleTurnInfo(bool myTurn)
    {
        if (myTurn)
        {
            turnIndImage.color = Color.green;
            turnIndText.text = "YOUR TURN";
        }
        else
        {
            turnIndImage.color = Color.red;
            turnIndText.text = "NOT YOUR TURN";
        }
    }

    //Ergänzungen zum Pointer
    /**
        Diese Methode verschiebt den Pointer im Spielfeld
    **/
    private void movePointer(Vector3 cellPosition)
    {
        //wegen Verschiebung der GameTiles
        Vector3 vector = new Vector3(cellPosition.x + 0.5f, cellPosition.y + 0.5f, 0);
        pointer.transform.position = vector;
    }

    /**
        Diese Methode färbt das staticGameObject, dynamicGameObject und gadget auf dem jeweiligen Feld gelb
        und zeigt char info falls ein char darauf liegt
        @param cellPosition: Postion des Feldes
    **/
    public void point(Vector3Int cellPosition)
    {
        //lokal gepointete GameObject
        GameObject pointed_GameObject_local = tilemap.GetInstantiatedObject(cellPosition);
        //falls sich das lokal gepointete GameObject vom vorherigen unterscheidet
        if (pointed_GameObject != null && !pointed_GameObject.Equals(pointed_GameObject_local))
        {
            //das GameObject bekommt seine ursprüngliche Farbe
            SpriteRenderer sp = pointed_GameObject.GetComponent<SpriteRenderer>();
            sp.color = pointed_GameObject_Color;
            if (!gameHandler.activeCharacterGuid.Equals(Guid.Empty) && char_table.ContainsKey(gameHandler.activeCharacterGuid))
            {
                fAI(gameHandler.getActiveCharacter(), true);
            }
        }

        //falls das lokal gepointete GameObject nicht null ist ... 
        if (pointed_GameObject_local != null)
        {
            SpriteRenderer sp = pointed_GameObject_local.GetComponent<SpriteRenderer>();
            //Die Farbe des GameObjects wird gespeichert, bevor sie schon gelb gefärbt wurde
            if (!sp.color.Equals(Color.yellow))
            {
                //ursprüngliche Farbe des GameObject für das Umfärben
                pointed_GameObject_Color = sp.color;
            }
            //das gepointete GameObject wird gespeichert
            pointed_GameObject = pointed_GameObject_local;
            //das gepointete GameObject wird gelb gefärbt
            sp.color = Color.yellow;

        }

        // GLEICHE FUER DYNAMISCHE OBJEKTE
        //lokal gepointete GameObject
        GameTile pointedTile = (GameTile)tilemap.GetTile(cellPosition);
        GameObject pointed_dynObject_local = null;
        if (pointedTile != null)
        {
            pointed_dynObject_local = pointedTile.dynamicGameObject;
        }
        //falls sich das lokal gepointete GameObject vom vorherigen unterscheidet
        if (pointed_dynObject != null && !pointed_dynObject.Equals(pointed_dynObject_local))
        {
            //das GameObject bekommt seine ursprüngliche Farbe
            SpriteRenderer sp = pointed_dynObject.GetComponent<SpriteRenderer>();
            sp.color = pointed_dynObject_Color;
            if (!gameHandler.activeCharacterGuid.Equals(Guid.Empty) && char_table.ContainsKey(gameHandler.activeCharacterGuid))
            {
                fAI(gameHandler.getActiveCharacter(), true);
            }
        }

        //falls das lokal gepointete GameObject nicht null ist ... 
        if (pointed_dynObject_local != null)
        {
            SpriteRenderer sp = pointed_dynObject_local.GetComponent<SpriteRenderer>();
            //Die Farbe des GameObjects wird gespeichert, bevor sie schon gelb gefärbt wurde
            if (!sp.color.Equals(Color.yellow))
            {
                //ursprüngliche Farbe des GameObject für das Umfärben
                pointed_dynObject_Color = sp.color;
            }
            //das gepointete GameObject wird gespeichert
            pointed_dynObject = pointed_dynObject_local;
            //das gepointete GameObject wird gelb gefärbt
            sp.color = Color.yellow;

            // show player info on hover
            if (pointed_dynObject_local.CompareTag("Player"))
            {
                Character c = gameHandler.getCharacterByName(pointed_dynObject_local.name);
                /*
                // TEMP
                if (!gameHandler.getActiveCharacter().Equals(c))
                {
                    fAI(c, false); // if not dont display any info
                }
                */

                // CORRECT
                //check if pointed char is in my own fraction (which should be set at wahlphase)
                if (!gameHandler.getFraction().Contains(c.getGuid()))
                {
                    fAI(c, false); // if not dont display any info
                }
                else
                {
                    fAI(c, true); // display full info
                }
            }
        }

        // GLEICHE FUER GADGETS
        //lokal gepointete Gadget
        GameObject pointed_gadget_local = null;
        if (pointedTile != null)
        {
            pointed_gadget_local = pointedTile.gadget;
        }
        //falls sich das lokal gepointete GameObject vom vorherigen unterscheidet
        if (pointed_gadget != null && !pointed_gadget.Equals(pointed_gadget_local))
        {
            //das GameObject bekommt seine ursprüngliche Farbe
            SpriteRenderer sp = pointed_gadget.GetComponent<SpriteRenderer>();
            sp.color = pointed_gadget_Color;
        }

        //falls das lokal gepointete GameObject nicht null ist ... 
        if (pointed_gadget_local != null)
        {
            SpriteRenderer sp = pointed_gadget_local.GetComponent<SpriteRenderer>();
            //Die Farbe des GameObjects wird gespeichert, bevor sie schon gelb gefärbt wurde
            if (!sp.color.Equals(Color.yellow))
            {
                //ursprüngliche Farbe des GameObject für das Umfärben
                pointed_gadget_Color = sp.color;
            }
            //das gepointete GameObject wird gespeichert
            pointed_gadget = pointed_gadget_local;
            //das gepointete GameObject wird gelb gefärbt
            sp.color = Color.yellow;
        }
    }

    /**
     * Label gibt an auf welchen logischen Koordinaten Pointer sich befindet
     * (Aufruf im Update)
     */
    public void updatePointLabel(int x, int y)
    {
        if (x >= 0 && x < gameHandler.mapSize.y && y >= 0 && y < gameHandler.mapSize.x)
        {
            pointLabel.text = "(" + x + "," + y + ")";
        }
    }

    /**
     * Ausgabe der letzt gemachte Aktion als Text im Label
     */
    public void updateLogLabel(BaseOperation operation)
    {
        string op_type = "";
        switch (operation.type)
        {
            case OperationEnum.CAT_ACTION:
                op_type = "Katze geht";
                break;
            case OperationEnum.EXFILTRATION:
                op_type = "Exfiltration";
                Exfiltration ex_op = operation as Exfiltration;
                if (ex_op != null)
                {
                    op_type += " von (" + ex_op.from.x + "," + ex_op.from.y + ")";
                }
                break;
            case OperationEnum.GADGET_ACTION:
                op_type = "Gadget Aktion";
                GadgetAction gad_op = operation as GadgetAction;
                if (gad_op != null)
                {
                    op_type = translateGadget(gad_op.gadget) + " Aktion";
                }
                break;
            case OperationEnum.GAMBLE_ACTION:
                op_type = "Roulette Aktion";
                break;
            case OperationEnum.JANITOR_ACTION:
                op_type = "Hausmeister geht";
                break;
            case OperationEnum.MOVEMENT:
                op_type = "Bewegung";
                Movement mov_op = operation as Movement;
                if (mov_op != null)
                {
                    op_type += " von (" + mov_op.from.x + "," + mov_op.from.y + ")";
                }
                break;
            case OperationEnum.PROPERTY_ACTION:
                op_type = "Property Aktion";
                PropertyAction pro_op = operation as PropertyAction;
                if (pro_op != null)
                {
                    op_type = translateProperty(pro_op.usedProperty) + " Aktion";
                }
                break;
            case OperationEnum.SPY_ACTION:
                op_type = "Ausspionieren";
                break;
            case OperationEnum.RETIRE:
                return;
        }
        logLabel.text = op_type + " auf (" + operation.target.x + "," + operation.target.y + ").";
    }

    /**
     * beliebiges update vom Log Label
     * z.B. für informative Sachen
     */
    public void updateLogLabel(string s)
    {
        logLabel.text = s;
    }

    /**
     * Label gibt die Runde an, wird bei jedem GameStatusMessage aktualisiert
     */
    public void updateRoundLabel(int round_curr)
    {
        roundLabel.text = "Runde: " + round_curr;
    }

    /**
     * Wenn Pointer auf einem AKTIVEN Roulette Feld ist, wird angegeben, wie viel Chips
     * am Tisch sind. Angabe solange Pointer auf dem Roulette Tisch bleibt
     */
    public void showRouletteInfo(Vector3Int coordinates)
    {
        GameTile tile = tilemap.GetTile(coordinates) as GameTile;
        FieldMap map = gameHandler.getFieldMap();
        if (tile != null && tile.GetStateEnum() == FieldStateEnum.ROULETTE_TABLE && map != null
            && gameHandler.GetFieldOnPos(new Point(coordinates.x, -coordinates.y)).isState(FieldStateEnum.ROULETTE_TABLE)
            && !gameHandler.GetFieldOnPos(new Point(coordinates.x, -coordinates.y)).getIsDestroyed())
        {
            roulette_info.SetActive(true);
            roulette_info.transform.localPosition = coordinates + new Vector3Int(0, 1, 0);
            roulette_info.GetComponent<TextMeshPro>().sortingOrder = -coordinates.y + 3;
            int chips = gameHandler.GetFieldOnPos(new Point(coordinates.x, -coordinates.y)).GetChipAmount();
            if (chips > 1)
            {
                roulette_info.GetComponent<TextMeshPro>().text = chips + Environment.NewLine + "Chips";
            }
            else if (chips == 1)
            {
                roulette_info.GetComponent<TextMeshPro>().text = chips + Environment.NewLine + "Single" + Environment.NewLine + "Chip";
            }
            else
            {
                roulette_info.GetComponent<TextMeshPro>().text = "No Chips";
            }
        }
        else
        {
            roulette_info.SetActive(false);
        }
    }

    /**
        Diese Methode gibt den Einsatz von Spielchips zum Spielen am Roulette Tisch zurück

        return eine Zahl, falls Einsatz valide, sonst -1
    **/
    public int getCommitmentForRoulette()
    {
        InputField inputField = roulette_table_commitment.GetComponent<InputField>();
        string input = inputField.text;
        if (input.Equals(""))
        {
            inputField.text = "Einsatz gefordert!";
            return -1;
        }

        //-1 ist ein invalider Input
        int commitment = -1;
        try
        {
            commitment = Int32.Parse(input);
        }
        catch (FormatException)
        {
            inputField.text = "Einsatz invalide!";
        }

        //Input invalide
        if (commitment <= 0) return -1;
        else return commitment;
    }

    public void printErrorInCommitmentForRoulette(string errorMessage)
    {
        InputField inputField = roulette_table_commitment.GetComponent<InputField>();
        inputField.text = errorMessage;
    }

    /**
     * Manuelles Ausschalten vom Roulette Eingabe (wenn GameOperation geschickt oder
     * Strike bekommen)
     */
    public void deactivateCommitment()
    {
        roulette_table_commitment.SetActive(false);
    }



    //AGENTEN INFO BOx
    /**
     * simplere Funktion um InfoBox zu aktualisieren
     * benutzt das untere
     * @param complete ursprünglich so gedacht, dass ich nur bei eigenen Agenten detallierte Infos
     * anzeige (wie z.B. HP, IP, Chips) aber dann einfach gesagt "eeh fuck it"
     */
    public void fAI(Character c, bool complete)
    {
        if (char_table.ContainsKey(c.getGuid()))
        {
            if (complete)
            {
                fill_AgentInfo(c.getName(), gameHandler.GetGender(c), c.getChips(),
                    gameHandler.mySafeCombinations, c.getProperties(), c.GetGadgets(), c.getHp(),
                    c.getIp(), c.getAp(), c.getMp());
            }
            else
            {
                fill_AgentInfo(c.getName(), gameHandler.GetGender(c), c.getChips(),
                    new HashSet<int>() { }, c.getProperties(), c.GetGadgets(), c.getHp(),
                    c.getIp(), c.getAp(), c.getMp());
            }
        }
    }


    /**
        Mithilfe dieser Methode werden die Labels in der Info Box gefüllt, param Bezeichnungen sind selbsterklärend
        -- Diese Methode wird vom GameHandler aufgerufen!
        Parameter richten sich an Typ im Standard!
    **/
    public void fill_AgentInfo(string name, GenderEnum gender, int game_Chips,
    HashSet<int> secrets, HashSet<PropertyEnum> featuers, HashSet<Gadget> gadgets,
    int hp, int ip, int ap, int mp)
    {
        //Name des Agenten
        agent_infos[0].text = name;

        //Geschlecht des Agenten
        if (gender.Equals(GenderEnum.MALE))
        {
            agent_infos[1].text = "Männlich";
            agent_icon.sprite = Resources.Load<Sprite>("AgentIcons/AgentMale");
        }
        if (gender.Equals(GenderEnum.FEMALE))
        {
            agent_infos[1].text = "Weiblich";
            agent_icon.sprite = Resources.Load<Sprite>("AgentIcons/AgentFemale");
        }
        if (gender.Equals(GenderEnum.DIVERSE))
        {
            agent_infos[1].text = "Divers";
            agent_icon.sprite = Resources.Load<Sprite>("AgentIcons/AgentFemale");
        }

        //Spielchips
        agent_infos[2].text = "" + game_Chips;

        //Hilfsvariablen
        string output = "";
        //Länge der HashSet (Datenstruktur)
        int size;
        //Zähler
        int i = 0;

        //Secrets
        if (secrets != null)
        {
            HashSet<int>.Enumerator enu_Int = secrets.GetEnumerator();
            size = secrets.Count;
            while (enu_Int.MoveNext())
            {
                int secret = enu_Int.Current;
                output += secret;
                i++;
                if (i < size)
                {
                    output += "," + Environment.NewLine;
                }
            }

        }
        agent_infos[3].text = output;


        //Liste an Eigenschaften
        HashSet<PropertyEnum>.Enumerator enu_PropertyEnum = featuers.GetEnumerator();
        i = 0;
        size = featuers.Count;
        output = "";

        while (enu_PropertyEnum.MoveNext())
        {
            PropertyEnum feature = enu_PropertyEnum.Current;
            output += translateProperty(feature);
            i++;
            if (i < size)
            {
                output += ", " + Environment.NewLine;
            }
        }

        agent_infos[4].text = output;


        //Liste an Gadgets
        HashSet<Gadget>.Enumerator enu_Gadget = gadgets.GetEnumerator();
        i = 0;
        size = gadgets.Count;
        output = "";

        while (enu_Gadget.MoveNext())
        {
            Gadget gadget = enu_Gadget.Current;
            output += translateGadget(gadget);
            i++;
            if (i < size)
            {
                output += ", " + Environment.NewLine;
            }
        }
        agent_infos[5].text = output;

        //HP
        agent_infos[6].text = "" + hp;

        //IP
        agent_infos[7].text = "" + ip;

        //AP
        agent_infos[8].text = "" + ap;

        //MP
        agent_infos[9].text = "" + mp;
    }

    /**
     * Zeige Statistik Canvas
     * Unterschiedliche Texte beim Zuschauerscreen und SpielerScreen
     */
    public void showStats(bool winner, VictoryEnum reason, StatisticsEntry[] stats, bool userIsPlayerOne, bool userIsPlayerTwo, bool spectatorP1_won)
    {
        statsCanvas.SetActive(true);
        //winner label
        TextMeshProUGUI label = GameObject.Find("WinnerLabel").GetComponent<TextMeshProUGUI>();
        if (!userIsPlayerOne && !userIsPlayerTwo)
        {
            label.text = spectatorP1_won ? "P1 WON" : "P2 WON";
            GameObject.Find("P1").GetComponent<TextMeshProUGUI>().text = "P1";
            GameObject.Find("P2").GetComponent<TextMeshProUGUI>().text = "P2";
        }
        else if (winner)
        {
            label.text = "YOU WIN";
            label.color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P1").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P1_IP").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P1_CocktailDrank").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P1_CocktailSpilled").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P1_Collar").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P1_Damage").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P2").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P2_IP").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P2_CocktailDrank").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P2_CocktailSpilled").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P2_Collar").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P2_Damage").GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            label.text = "YOU LOST";
            label.color = Color.red;
            GameObject.Find("P1").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P1_IP").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P1_CocktailDrank").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P1_CocktailSpilled").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P1_Collar").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P1_Damage").GetComponent<TextMeshProUGUI>().color = Color.red;
            GameObject.Find("P2").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P2_IP").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P2_CocktailDrank").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P2_CocktailSpilled").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P2_Collar").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
            GameObject.Find("P2_Damage").GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.65f, 0);
        }
        // reason label
        label = GameObject.Find("WinReasonLabel").GetComponent<TextMeshProUGUI>();
        label.text = writeCorrectly(reason.ToString());
        foreach (StatisticsEntry stat in stats)
        {
            label = null;
            TextMeshProUGUI label2;
            switch (stat.title)
            {
                case "Intelligence Points":
                    label = GameObject.Find("P1_IP").GetComponent<TextMeshProUGUI>();
                    label2 = GameObject.Find("P2_IP").GetComponent<TextMeshProUGUI>();
                    break;
                case "Drank Cocktails":
                    label = GameObject.Find("P1_CocktailDrank").GetComponent<TextMeshProUGUI>();
                    label2 = GameObject.Find("P2_CocktailDrank").GetComponent<TextMeshProUGUI>();
                    break;
                case "Spilled Cocktails":
                    label = GameObject.Find("P1_CocktailSpilled").GetComponent<TextMeshProUGUI>();
                    label2 = GameObject.Find("P2_CocktailSpilled").GetComponent<TextMeshProUGUI>();
                    break;
                case "Returned collar":
                    label = GameObject.Find("P1_Collar").GetComponent<TextMeshProUGUI>();
                    label2 = GameObject.Find("P2_Collar").GetComponent<TextMeshProUGUI>();
                    break;
                case "Damage taken":
                    label = GameObject.Find("P1_Damage").GetComponent<TextMeshProUGUI>();
                    label2 = GameObject.Find("P2_Damage").GetComponent<TextMeshProUGUI>();
                    break;
                default:
                    continue;
            }
            label.text = (userIsPlayerOne || userIsPlayerTwo) ? (userIsPlayerOne ? stat.valuePlayer1 : stat.valuePlayer2) : stat.valuePlayer1;
            label2.text = (userIsPlayerOne || userIsPlayerTwo) ? (userIsPlayerOne ? stat.valuePlayer2 : stat.valuePlayer1) : stat.valuePlayer2;
        }
    }

    /**
     * Helfer Methode um Strings First Upper Case darzustellen
     */
    private static string writeCorrectly(string s)
    {
        string[] s_arr = s.ToLower().Split('_');
        string result = "";
        foreach (string str in s_arr)
        {
            result += char.ToUpper(str[0]) + str.Substring(1) + " ";
        }
        return result.Substring(0, result.Length - 1);
    }

    /**
        Diese Hilfsmethode übersetzt das Feature in die deutsche Sprache
        @param p: Feauture als Enum

        return deutscher Text als String
    **/
    public static string translateProperty(PropertyEnum p)
    {
        switch (p)
        {
            case PropertyEnum.AGILITY:
                return "Agilität";
            case PropertyEnum.BABYSITTER:
                return "Babysitter";
            case PropertyEnum.BANG_AND_BURN:
                return "Bang and Burn";
            case PropertyEnum.CLAMMY_CLOTHES:
                return "Klamme Klamotten";
            case PropertyEnum.CONSTANT_CLAMMY_CLOTHES:
                return "Konstante Klamme Klamotten";
            case PropertyEnum.FLAPS_AND_SEALS:
                return "Flaps and Seals";
            case PropertyEnum.HONEY_TRAP:
                return "Honey Trap";
            case PropertyEnum.JINX:
                return "Pechvogel";
            case PropertyEnum.LUCKY_DEVIL:
                return "Glückspilz";
            case PropertyEnum.NIMBLENESS:
                return "Behändigkeit";
            case PropertyEnum.OBSERVATION:
                return "Observation";
            case PropertyEnum.PONDEROUSNESS:
                return "Behäbigkeit";
            case PropertyEnum.ROBUST_STOMACH:
                return "Robuster Magen";
            case PropertyEnum.SLUGGISHNESS:
                return "Schwerfälligkeit";
            case PropertyEnum.SPRYNESS:
                return "Behändigkeit";
            case PropertyEnum.TOUGHNESS:
                return "Zähigkeit";
            case PropertyEnum.TRADECRAFT:
                return "Tradecraft";

        }
        //Bei Fehler
        return "unbekannt";
    }


    /**
        Diese Methode übersetzt das Gadget ins Deutsche
        @param Gadget: Gadget

        return deutscher Text als String
    **/
    public static string translateGadget(Gadget gadget)
    {
        GadgetEnum gadgetEnum = gadget.gadget;
        switch (gadgetEnum)
        {
            case GadgetEnum.HAIRDRYER:
                return "Akku-Föhn";
            case GadgetEnum.MOLEDIE:
                return "Maulwürfel";
            case GadgetEnum.TECHNICOLOUR_PRISM:
                return "Technicolor-Prisma";
            case GadgetEnum.BOWLER_BLADE:
                return "Klingen-Hut";
            //Dieses Gadget hat keine aktive Wirkung !!!
            case GadgetEnum.MAGNETIC_WATCH:
                return "Magnetfeld-Armbanduhr";
            case GadgetEnum.POISON_PILLS:
                return "Giftpillen-Flasche";
            case GadgetEnum.LASER_COMPACT:
                return "Laser-Puderdose";
            case GadgetEnum.ROCKET_PEN:
                return "Raketenwerfer-Füllfederhalter";
            case GadgetEnum.GAS_GLOSS:
                return "Gaspatronen-Lippenstift";
            case GadgetEnum.MOTHBALL_POUCH:
                return "Mottenkugel-Beutel";
            case GadgetEnum.FOG_TIN:
                return "Nebeldose";
            case GadgetEnum.GRAPPLE:
                return "Wurfhaken";
            case GadgetEnum.WIRETAP_WITH_EARPLUGS:
                return "Wanze und Ohrstöpsel";
            //Keine aktive Wirkung !!!
            case GadgetEnum.DIAMOND_COLLAR:
                return "Diamanthalsband";
            case GadgetEnum.JETPACK:
                return "Jetpack";
            case GadgetEnum.CHICKEN_FEED:
                return "Chicken Feed";
            case GadgetEnum.NUGGET:
                return "Nugget";
            case GadgetEnum.MIRROR_OF_WILDERNESS:
                return "Mirror of Wilderness";
            //Keine aktive Wirkung!!!
            case GadgetEnum.POCKET_LITTER:
                return "Pocket Litter";
            case GadgetEnum.ANTI_PLAGUE_MASK:
                return "Anti-Seuchen-Schnabelmaske";
            case GadgetEnum.COCKTAIL:
                return "Cocktail";
        }

        //Bei Fehler
        return "unbekannt";
    }

    /**
     * Obere Funktion überladen, damit es auch bei GadgetEnum Übergabe funktioniert
     */
    public static string translateGadget(GadgetEnum gadgetEnum)
    {
        switch (gadgetEnum)
        {
            case GadgetEnum.HAIRDRYER:
                return "Akku-Föhn";
            case GadgetEnum.MOLEDIE:
                return "Maulwürfel";
            case GadgetEnum.TECHNICOLOUR_PRISM:
                return "Technicolor-Prisma";
            case GadgetEnum.BOWLER_BLADE:
                return "Klingen-Hut";
            //Dieses Gadget hat keine aktive Wirkung !!!
            case GadgetEnum.MAGNETIC_WATCH:
                return "Magnetfeld-Armbanduhr";
            case GadgetEnum.POISON_PILLS:
                return "Giftpillen-Flasche";
            case GadgetEnum.LASER_COMPACT:
                return "Laser-Puderdose";
            case GadgetEnum.ROCKET_PEN:
                return "Raketenwerfer-Füllfederhalter";
            case GadgetEnum.GAS_GLOSS:
                return "Gaspatronen-Lippenstift";
            case GadgetEnum.MOTHBALL_POUCH:
                return "Mottenkugel-Beutel";
            case GadgetEnum.FOG_TIN:
                return "Nebeldose";
            case GadgetEnum.GRAPPLE:
                return "Wurfhaken";
            case GadgetEnum.WIRETAP_WITH_EARPLUGS:
                return "Wanze und Ohrstöpsel";
            //Keine aktive Wirkung !!!
            case GadgetEnum.DIAMOND_COLLAR:
                return "Diamanthalsband";
            case GadgetEnum.JETPACK:
                return "Jetpack";
            case GadgetEnum.CHICKEN_FEED:
                return "Chicken Feed";
            case GadgetEnum.NUGGET:
                return "Nugget";
            case GadgetEnum.MIRROR_OF_WILDERNESS:
                return "Mirror of Wilderness";
            //Keine aktive Wirkung!!!
            case GadgetEnum.POCKET_LITTER:
                return "Pocket Litter";
            case GadgetEnum.ANTI_PLAGUE_MASK:
                return "Anti-Seuchen-Schnabelmaske";
            case GadgetEnum.COCKTAIL:
                return "Cocktail";
        }
        return "Unbekannt";
    }

    /**
     * Tile Sprites und GameObjekte auf den Tiles setzen (je nach StateEnum)
     * Wird jedes Mal aufgerufen, wenn sich Aufbau des Feldes abaendert (beim Aufbau vom Tilemap,
     * wenn ein Wand zerstört wird) - sonstige Aenderungen sind nur am GameObjects auf dem Tiles,
     * aendern den Aufbau des Tiles nicht
     */
    public static void setTileInfo(FieldStateEnum fs, int pos_row, int pos_col, ref GameTile tile, ref GameObject tileObject)
    {
        GameObject NullPointer = null;
        switch (fs)
        {
            // tile sprites are pivotted at bottom center and tile anchors are (0.5, 0, 0)
            case FieldStateEnum.SAFE:
                tile.sprite = Resources.Load<Sprite>("Sprites/floor");
                // gameobject put on sprite because of animations
                tileObject = (GameObject)Resources.Load("Prefabs/safe");

                tile.setStaticGameObject(ref tileObject);
                break;
            case FieldStateEnum.WALL: // 
                if (pos_row == 0)
                {
                    tile.sprite = Resources.LoadAll<Sprite>("Sprites/wall")[2]; // long wall 32x64
                }
                else if (pos_row % 2 == 0)
                {
                    tile.sprite = Resources.LoadAll<Sprite>("Sprites/wall")[1];
                }
                else
                {
                    tile.sprite = Resources.LoadAll<Sprite>("Sprites/wall")[0];
                }
                break;
            case FieldStateEnum.BAR_SEAT:
                tile.sprite = Resources.Load<Sprite>("Sprites/floor");
                tileObject = (GameObject)Resources.Load("Prefabs/chair");
                tile.setStaticGameObject(ref tileObject);
                break;
            case FieldStateEnum.BAR_TABLE:
                tile.sprite = Resources.Load<Sprite>("Sprites/floor");
                tileObject = (GameObject)Resources.Load("Prefabs/table");
                tile.setStaticGameObject(ref tileObject);
                break;
            case FieldStateEnum.FIREPLACE:
                tile.sprite = Resources.Load<Sprite>("Sprites/floor");
                // gameobject put on sprite because of animations
                tileObject = (GameObject)Resources.Load("Prefabs/kamin");
                tile.setStaticGameObject(ref tileObject);
                break;
            case FieldStateEnum.ROULETTE_TABLE:
                tile.sprite = Resources.Load<Sprite>("Sprites/floor");
                tileObject = (GameObject)Resources.Load("Prefabs/roulette");
                tile.setStaticGameObject(ref tileObject);
                break;
            case FieldStateEnum.FREE:
            default:
                tile.sprite = Resources.Load<Sprite>("Sprites/floor");
                tile.setStaticGameObject(ref NullPointer);
                break;
        }
        tile.setStateEnum(fs);
    }


    /**
     * Unity möchte Vector3 nicht intern zu Vector3Ints konvertieren
     * then i took my faith in my own hands :D
     */
    public static Vector3Int vector3_to_vector3int(Vector3 vector)
    {
        return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
    }

    /**
     * Flag der besagt, obs Zuschauerscreen ist, wird erst hochgestellt nachdem das Screen zu einem
     * Zuschauerscreen umgestellt wurde
     */
    public bool isSpectatorScreen;

    /**
     * Umstellung auf Zuschauerscreen
     * Einfach alles Deaktivieren, die mir irgendwelche Eingaben ermöglichen
     */
    public void spectatorScreen()
    {
        isSpectatorScreen = true;
        dropDown.gameObject.SetActive(false);
        GameObject.Find("PauseButton").SetActive(false);
        GameObject.Find("TurnIndicator").SetActive(false);
    }
}