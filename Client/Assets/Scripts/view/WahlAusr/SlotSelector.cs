using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * @author Italgo Pellegrino
 * Diese Klasse Enthält die Logik um die Slots der Wahlphase zu befüllen, die ItemChoice Nachrichten werden hier versendet und
 * die Ausrüstungsphase wird hier geladen.
**/
public class SlotSelector : MonoBehaviour
{
    public Button yourButton;
    

    //Zugewiesene Kartenslots
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    public GameObject slot4;
    public GameObject slot5;
    public GameObject slot6;
    public GameObject slot7;
    public GameObject slot8;

    //Diese Karten werden Instaziert und werden statt den alten Slots angezeigt
    private GameObject cardSlot1;
    private GameObject cardSlot2;
    private GameObject cardSlot3;
    private GameObject cardSlot4;
    private GameObject cardSlot5;
    private GameObject cardSlot6;
    private GameObject cardSlot7;
    private GameObject cardSlot8;

    //Diese Liste wird benötigt um keine Doppelte Gadgets oder Charaktere auszuwählen
    private List<GadgetEnum> selectedGadgets;
    private List<string> selectedCharacters;

    GameHandler gameHandler = Connection.gameHandler;
    GameObject selCard;

    //Counter um die verschiedene Slots zu befüllen
    private int counter;

    void Start()
    {      
        counter = 1;
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

        selectedGadgets = new List<GadgetEnum>();
        selectedCharacters = new List<string>();
    }
 
    /**
     *Selektiert Slots je nach Zustand des Counter
     */
    void TaskOnClick()
    {
        //Ausgewählte Karte
        selCard = GameObject.FindGameObjectWithTag("HighlightCard");
        
        //Falls ausgewählte Karte angezeigt wird
        if (selCard.GetComponent<Canvas>().enabled == true)
        {
            switch (counter)
            {
                //Slot 1
                case 1:
                    selectSlot(cardSlot1, slot1);
                    break;

                //Slot 2
                case 2:
                    selectSlot(cardSlot2, slot2);
                    break;

                //Slot 3
                case 3:
                    selectSlot(cardSlot3, slot3);
                    break;

                //Slot 4
                case 4:
                    selectSlot(cardSlot4, slot4);
                    break;

                //Slot 5
                case 5:
                    selectSlot(cardSlot5, slot5);
                    break;

                //Slot 6
                case 6:
                    selectSlot(cardSlot6, slot6);   
                    break;

                //Slot 7
                case 7:
                    selectSlot(cardSlot7, slot7);
                    break;

                //Slot 8
                case 8:
                    selectSlot(cardSlot8, slot8);
                    //Direkt nach dem selektieren der letzten Karte wird die AusrüstungsScene geladen
                    SceneManager.LoadScene("Scenes/AusrüstungphaseScene");
                    break;
                default:
                    Debug.Log("Wahlphase zu ende");
                    break;
            }
           
        }
        
    }

    /**
     * Selektiert ein bestimmtes Slot
     */
    private void selectSlot(GameObject cardSlot, GameObject slot)
    {
        //Instaziert eine Kopie der ausgewählten Karten an der Position vom Slot
        cardSlot = Instantiate(selCard, slot.transform.position, Quaternion.identity);
        cardSlot.name = "Slot "+counter;
        cardSlot.tag = "slot";
        cardSlot.transform.localScale = (new Vector3(0.4f, 0.4f, 1));

        //Deaktiviert den Slot
        slot.SetActive(false);

        //Falls ausgewählte Karte ein Gadget ist
        if (selCard.GetComponent<CardSetup>().characterGuid.Equals(""))
        {
            //ItemChoiceMessage wird gesendet
            gameHandler.sendItemChoiceMessage(new ItemChoiceMessage(gameHandler.playerId, DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")), null, selCard.GetComponent<CardSetup>().gadgetCopy));
            //Diese Gadget wird in der Liste der bereits gewählten Gadgets ausgewählt. Lassen sich nicht mehr selektieren
            selectedGadgets.Add(selCard.GetComponent<CardSetup>().gadgetCopy);
            counter++;
        }
        //Falls ausgewählte Karte ein Charakter ist, funktioniert wie der obere Fall, nur mit Charaktere
        else
        {
            gameHandler.sendItemChoiceMessage(new ItemChoiceMessage(gameHandler.playerId, DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")), Guid.Parse(selCard.GetComponent<CardSetup>().characterGuid), null));
            selectedCharacters.Add(selCard.GetComponent<CardSetup>().characterGuid);
            counter++;
        }
    }
}
