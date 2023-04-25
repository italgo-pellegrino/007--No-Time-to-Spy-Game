using JetBrains.Annotations;
//IndexOutOfBoundsException
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/**
    Vlt muss diese Klasse ins BuildMap ausgelagert werden
**/
public class Buttons : MonoBehaviour
{
    public Button button;

    //static, weil dieses Skript an alle Buttons zugeordnet wird
    //muss es static sein???
    private GameHandler gameHandler;

    public static bool good_to_go; //flag telling us buttons should be activated
    private bool gone; //flag that tells us buttons are activated (so that we wont activate them every frame)

    //Es wird mit der Referenz des GameHandlers gearbeitet, da der GameHandler standig geupdatet wird
    //Es wird darauf geachtet, dass auf den GameHandler nur ein lesender Zugriff erfolgt!!
    public void setGameHandler(ref GameHandler gameHandlerReference)
    {
        gameHandler = gameHandlerReference;
    }

    //Am Anfang sollten die Buttons deaktiviert sein!!
    //Es muss auf die Parallel laufende Start-Methode aufgepasst werden!
    void Start()
    {
        setGameHandler(ref Connection.gameHandler);
        button = gameObject.GetComponent<Button>();
        //button.onClick.AddListener(button_debug);
        button.onClick.AddListener(button_action);
    }

    public void button_action()
    {
        //Der ActiveCharacter im GameHandler gibt die logischen Koordinaten des Charakters an
        //Mögliche Darstellung: ActiveCharacter, also der Agent wird gelb umrandet, Kamera zeigt in Richtung des Agenten (optinal), Buttons werden aktiviert für Spieler des Agenten (selektiv oder alle??)
        //Uberprufung auf Bewegungpunkte **TODO**

        //hier: Deaktivierung der Button - Leiste, damit nicht 2 Signale "gleichzeitig" gesendet werden
        //deactivateButton();

        //Auswertung des Signals ... 

        //GameHandler schickt Move-Message
        //Implementierung erfolgt bei Vorlage der WebSocket Schnittstelle
        bool valid = true;
        Point curr_pos = gameHandler.getActiveCharacter().getCoordinates();
        switch (button.name)
        {
            case "r":
                curr_pos += new Point(1, 0);
                break;
            case "ur":
                curr_pos += new Point(1, -1);
                break;
            case "u":
                curr_pos += new Point(0, -1);
                break;
            case "ul":
                curr_pos += new Point(-1, -1);
                break;
            case "l":
                curr_pos += new Point(-1, 0);
                break;
            case "dl":
                curr_pos += new Point(-1, 1);
                break;
            case "d":
                curr_pos += new Point(0, 1);
                break;
            case "dr":
                curr_pos += new Point(1, 1);
                break;
            default:
                Debug.Log("huh??");
                valid = false;
                break;
        }
        if (valid)
        {
            gameHandler.sendMoveMessage(curr_pos);
        }
    }

    /**
     * Falls Buttons aktiviert werden können aber noch nicht aktiviert sind 
     * aktiviere sie. Mache nicht solange sie aktiv sind. Wenn sie nicht
     * mehr aktiv sein sollen werden sie deaktiviert
     */
    void Update()
    {
        if (good_to_go && !gone) // should be activated but not activated
        {
            //Debug.Log("ACTIVATION");
            deactivateButton();
            activateSelectivelyButtons();
            gone = true; // are activated
        }
        else if (!good_to_go) // should be deactivated
        {
            //Debug.Log("RESET");
            gone = false; //are not activated
            this.gameObject.SetActive(false);
        }
    }
    /**
     * Diese Methode deaktiviert alle (bzw. nicht deaktivierten) Buttons in der Button - Leiste
     */
    public void deactivateButton()
    {
        //Debug.Log("deactivation");
        button.interactable = false;
    }

    /**
     * Diese Methode uberpruft, ob das Zielfeld betretbar ist und leitet weitere Schritte ein
    */
    private bool isFieldEnterable(int x_pos, int y_pos, Field[,] map)
    {
        Field field;

        //Durch das try catch wird uberpruft, ob die angegebenen Indixes über die Array Dimensionen hinausgeht
        try
        {
            field = map[y_pos, x_pos];
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log(e.StackTrace);
            return false;
        }

        FieldStateEnum fieldState = field.getFieldStateEnum();
        //Ein Charakter kann nur sich auf einen Stuhl oder auf ein freies Feld sich bewegen
        if (fieldState.Equals(FieldStateEnum.BAR_SEAT) || fieldState.Equals(FieldStateEnum.FREE))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /**
        Diese Methode aktiviert selektiv Buttons in der Button - Leiste
    **/
    public void activateSelectivelyButtons()
    {
        //Debug.Log("activation");
        //Koordinaten des activePlayer
        int x, y;
        Character activeCharacter = gameHandler.getActiveCharacter();
        Point coordinates = activeCharacter.getCoordinates();
        x = coordinates.x;
        y = coordinates.y;

        //Map
        FieldMap fieldMap = gameHandler.getFieldMap();
        Field[,] map = fieldMap.getMap();

        //Uberprufung auf Bewegungspunkte
        //Falls keine Bewegungspunkte vorhanden sind, dann bleiben alle Buttons deaktiviert
        if (activeCharacter.getMp() <= 0) return;

        //Hat der Charakter das Halsband ??
        bool hasDiamondCollor = activeCharacter.hasGadget(GadgetEnum.DIAMOND_COLLAR) != null;

        //Selektive Aktivierung der Buttons je nach dem, ob ein Feld betretbar ist oder nicht
        switch (button.name)
        {
            case "r":
                if (isFieldEnterable(x + 1, y, map) && !gameHandler.ExistsJanitorOnPos(new Point(x + 1, y))) button.interactable = true;
                break;
            case "ur":
                if (isFieldEnterable(x + 1, y - 1, map) && !gameHandler.ExistsJanitorOnPos(new Point(x + 1, y - 1))) button.interactable = true;
                break;
            case "u":
                if (isFieldEnterable(x, y - 1, map) && !gameHandler.ExistsJanitorOnPos(new Point(x, y - 1))) button.interactable = true;
                break;
            case "ul":
                if (isFieldEnterable(x - 1, y - 1, map) && !gameHandler.ExistsJanitorOnPos(new Point(x - 1, y - 1))) button.interactable = true;
                break;
            case "l":
                if (isFieldEnterable(x - 1, y, map) && !gameHandler.ExistsJanitorOnPos(new Point(x - 1, y))) button.interactable = true;
                break;
            case "dl":
                if (isFieldEnterable(x - 1, y + 1, map) && !gameHandler.ExistsJanitorOnPos(new Point(x - 1, y + 1))) button.interactable = true;
                break;
            case "d":
                if (isFieldEnterable(x, y + 1, map) && !gameHandler.ExistsJanitorOnPos(new Point(x, y + 1))) button.interactable = true;
                break;
            case "dr":
                if (isFieldEnterable(x + 1, y + 1, map) && !gameHandler.ExistsJanitorOnPos(new Point(x + 1, y + 1))) button.interactable = true;
                break;
            default:
                Debug.Log("huh??");
                break;
        }
    }
}