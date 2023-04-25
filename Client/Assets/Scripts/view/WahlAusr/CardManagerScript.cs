using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @author Italgo Pellegrino
 * Diese Klasse steuert alle Karten der Wahl und Ausrüstungsphase. Sie setzt die Nachricht die vom Server kommt um,
 * indem sie sich des CardSetup Script zur Änderung der Karten und des TextChanger Script zur änderung der Charakter Karten bedient.
 * Außerdem dient diese Klasse um ein Verweis auf alle Karten der Scene zu haben, die im Inspector zugewiesen wird.
 * */
public class CardManagerScript : MonoBehaviour
{
    private GameHandler gameHandler = Connection.gameHandler;
    
    //Diese 6 Karten sind die vom Server Empfangene Karten der Wahlphase. In der Ausrüstungsphase sind diese Karte Stattdessen
    //Die Characterkarten die man zuvor in der Wahlphase gewählt hat.
    public GameObject One;
    public GameObject Two;
    public GameObject Three;
    public GameObject Four;
    public GameObject Five;
    public GameObject Six;
    
    //Diese 6 Gadgets werden nur in der Ausrüstungsphase befüllt und es sind die zuvor gewählten Gadgets.
    public GameObject Gadget1;
    public GameObject Gadget2;
    public GameObject Gadget3;
    public GameObject Gadget4;
    public GameObject Gadget5;
    public GameObject Gadget6;

    //Diese 6 Slots sind die Slots der Ausrüstungsphase und werden vom SlotData Script aufgerufen
    public GameObject Slot1;
    public GameObject Slot2;
    public GameObject Slot3;
    public GameObject Slot4;
    public GameObject Slot5;
    public GameObject Slot6;

    //Daten der 6 Karten sind in diese Arrays drin. Diese werden in der OnRequestItemChoice und On RequestEquipmentChoice
    //Methode vom GameHandler befüllt, dieser bekommt wiederum die Daten vom Server.
    public static Guid[] charactersArray;
    public static GadgetEnum[] gadgetsArray;

    //Diese Variable wird im GameHandler in der onRequestItemChoice oder onRequestEquipmentChoice Methode auf true gesetzt.
    public static bool eventComing;
    GameObject cardManager;
    // Start is called before the first frame update
    void Start()
    {
    }

    //falls eine ItemChoiceMessage oder EquipmentChoiceMessage ankommt, werden die Karten gesetzt.
    void Update()
    {
        if (eventComing)
        {
            setCards();
            eventComing = false;
        }
    }

    /**
     * Setzt alle Karten, abhängig der jeweiligen Scene
     */ 
    private void setCards()
    {
        setAllGadgets();
        setAllCharacters();
    }
    /**
     * Setzt alle Charaktere, abhängig der jeweiligen Scene
     **/
    private void setAllCharacters()
    {
        cardManager = this.gameObject;
        int switchCounter;
        int forCounter;

        //Falls man in der Wahlphase ist
        if (SceneManager.GetActiveScene().name.Equals("WahlphaseScene"))
        {
            forCounter = 6 - gadgetsArray.Length;
        }
        //Falls man in der Ausrüstungsphase ist
        else
        {
            forCounter = charactersArray.Length;
        }
        
        //Diese For-Schleife weißt den 6(falls vorhanden) GameObjects Karten den jeweiligen Character zu. 
        for (int i = 1; (forCounter) >= i; i++)
        {
            //falls man in der Wahlphase ist
            if (SceneManager.GetActiveScene().name.Equals("WahlphaseScene"))
            {
                switchCounter = i + gadgetsArray.Length;
            }
            //falls man in der Ausrüstungsphase ist
            else
            {
                switchCounter = i;
            }
            switch (switchCounter)
            {
                //erste zur Wahl stehende Karte
                case 1:
                    //Nimmt die Guid des bereits befüllten charactersArray
                    Guid cardOne = charactersArray[i - 1];
                    setCharacter(cardManager.GetComponent<CardManagerScript>().One, i);
                    break;
                //zweite Karte
                case 2:
                    Guid cardTwo = charactersArray[i - 1];
                    setCharacter(cardManager.GetComponent<CardManagerScript>().Two, i);
                    break;
                //dritte Karte
                case 3:
                    Guid cardThree = charactersArray[i - 1];
                    setCharacter(cardManager.GetComponent<CardManagerScript>().Three, i);
                    break;
                //vierte Karte
                case 4:
                    Guid cardFour = charactersArray[i - 1];
                    setCharacter(cardManager.GetComponent<CardManagerScript>().Four, i);
                    break;
                //fünfte Karte
                case 5:
                    Guid cardFive = charactersArray[i - 1];
                    setCharacter(cardManager.GetComponent<CardManagerScript>().Five, i);
                    break;
                //sechste Karte             
                case 6:
                    Guid cardSix = charactersArray[i - 1];
                    setCharacter(cardManager.GetComponent<CardManagerScript>().Six, i);
                    break;
                //sollte nicht auftreten
                default:
                    Debug.Log("Keine Charaktere mehr");
                    break;
            }
        }
    }

    /**
     * setzt alle Gadgets
     */
    private void setAllGadgets()
    {
        cardManager = this.gameObject;

        //Diese For-Schleife weißt den 6(falls vorhanden) GameObjects Karten den jeweiligen GadgetEnum zu
        for (int i = 1; gadgetsArray.Length >= i; i++)
        {
            switch (i)
            {
                //erste Karte.
                case 1:                                     
                    setGadget(cardManager.GetComponent<CardManagerScript>().One, cardManager.GetComponent<CardManagerScript>().Gadget1,i);                   
                    break;
                //zweiten Karte.
                case 2:
                    setGadget(cardManager.GetComponent<CardManagerScript>().Two, cardManager.GetComponent<CardManagerScript>().Gadget2, i);
                    break;
                //dritte Karte.
                case 3:
                    setGadget(cardManager.GetComponent<CardManagerScript>().Three, cardManager.GetComponent<CardManagerScript>().Gadget3, i);
                    break;
                //vierte Karte.
                case 4:
                    setGadget(cardManager.GetComponent<CardManagerScript>().Four, cardManager.GetComponent<CardManagerScript>().Gadget4, i);
                    break;
                //fünfte Karte.
                case 5:
                    setGadget(cardManager.GetComponent<CardManagerScript>().Five, cardManager.GetComponent<CardManagerScript>().Gadget5, i);
                    break;
                //sechste Karte.
                case 6:
                    setGadget(cardManager.GetComponent<CardManagerScript>().Six, cardManager.GetComponent<CardManagerScript>().Gadget6, i);
                    break;
                //sollte nicht auftreten
                default:
                    Debug.Log("Keine Gadgets Mehr");
                    break;
            }
        }
    }

    /**
     * platziert eine Charakter Card auf Position i         
     */
    private void setCharacter(GameObject Card,int i)
    {      
        for (int j = 0; j < gameHandler.characterSettings.Length; j++)
        {
            //In characterSettings findet man alle Relevante Informationen von der Empfangene Guid des Servers.
            if (gameHandler.characterSettings[j].getCharacterId().Equals(charactersArray[i - 1]))
            {
                //Mithilfe des CardSetup Script, lässt sich die Kartenbild ändern. Während der TextChanger Script den Namen und den Text der Karte steuert.
                Card.GetComponent<CardSetup>().character = gameHandler.characterSettings[j].GetGender();
                Card.GetComponent<TextChanger>().NameString = gameHandler.characterSettings[j].name;
                CharacterDescription chars = new CharacterDescription(gameHandler.characterSettings[j].name, gameHandler.characterSettings[j].description, gameHandler.characterSettings[j].GetGender(), gameHandler.characterSettings[j].features);
                Card.GetComponent<TextChanger>().TextString = chars.getFeatureString(",");
            }
        }

        //Der GadgetEnum und characterGuid wird auf den Korrekten Wert gesetzt.
        Card.GetComponent<CardSetup>().gadget = GadgetEnum.None;
        Card.GetComponent<CardSetup>().characterGuid = charactersArray[i - 1].ToString();
    }

    /**
     * platziert eine gadget Card auf Position i
     */
    private void setGadget(GameObject Card,GameObject Gadget, int i)
    {
        //Setzen der ersten Karte auf dem jeweiligen GadgetEnum
        GadgetEnum DataEnum = gadgetsArray[i - 1];
        //falls die Scene die Wahlphase ist
        if (SceneManager.GetActiveScene().name.Equals("WahlphaseScene"))
        {
           Card.GetComponent<CardSetup>().gadget = DataEnum;
            //Die Character Infos werden auf dem defaultwert gesetzt
           Card.GetComponent<CardSetup>().character = GenderEnum.None;
           Card.GetComponent<CardSetup>().characterGuid = "";
        }
        //Falls es die Ausrüstungsphase ist
        else
        {
            //Die Gadgets werden gesetzt
            Gadget.GetComponent<CardSetup>().gadget = DataEnum;
            Gadget.GetComponent<CardSetup>().character = GenderEnum.None;
        }
    }
}
