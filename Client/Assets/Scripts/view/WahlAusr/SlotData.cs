using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Italgo Pellegrino
 * Diese Klasse kümmert sich darum den Inventar der Charaktere in den Slots anzuzeigen
 */
public class SlotData : MonoBehaviour
{
    GameObject cardManager;
    GameObject highlightedCard;
 
    public List<GadgetEnum> inventar1;
    public List<GadgetEnum> inventar2;
    public List<GadgetEnum> inventar3;
    public List<GadgetEnum> inventar4;

    void Start()
    {
        cardManager = GameObject.FindGameObjectWithTag("CardManager");

        //Initialisierung der Character Inventare
        inventar1 = new List<GadgetEnum>();
        inventar2 = new List<GadgetEnum>();
        inventar3 = new List<GadgetEnum>();
        inventar4 = new List<GadgetEnum>();   
    }

    // Update is called once per frame
    void Update()
    {
        //Selektierter Charakter
        highlightedCard = GameObject.FindGameObjectWithTag("HighlightCard");
            //Zunächst werden die Slots zurückgesetzt
            resetSlots();
        //Es wird überprüft welcher Charakter die Ausgewählte Charakterkarte ist und das jeweilige Inventar wird bei Slots angezeigt
        //Das Inventar wird im ItemSlot Skript aufgefüllt oder im ResetButton Script wieder zurückgesetzt
        if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().One.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {                  
                setSlots(inventar1);               
            }
            if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().Two.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {
                setSlots(inventar2);              
            }
            if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().Three.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {
                setSlots(inventar3);                
            }
            if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().Four.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {
                setSlots(inventar4);              
            }
    }

    /**
     * Setzt die Slots zurück
     */
    private void resetSlots()
    {
        cardManager.GetComponent<CardManagerScript>().Slot1.GetComponent<CardSetup>().gadget = GadgetEnum.None;
        cardManager.GetComponent<CardManagerScript>().Slot2.GetComponent<CardSetup>().gadget = GadgetEnum.None;        
        cardManager.GetComponent<CardManagerScript>().Slot3.GetComponent<CardSetup>().gadget = GadgetEnum.None;
        cardManager.GetComponent<CardManagerScript>().Slot4.GetComponent<CardSetup>().gadget = GadgetEnum.None;
        cardManager.GetComponent<CardManagerScript>().Slot5.GetComponent<CardSetup>().gadget = GadgetEnum.None;
        cardManager.GetComponent<CardManagerScript>().Slot6.GetComponent<CardSetup>().gadget = GadgetEnum.None;
    }

    /**
     * Die Methode bekommt eine Liste an GadgetEnums und setzt die Slots auf den jeweiligen GadgetEnum.
     */
    private void setSlots(List<GadgetEnum> inventar)
    {     
        GadgetEnum[] inventarArray = inventar.ToArray();

        //Slot 1
        if (inventarArray.Length >= 1)
        {
            cardManager.GetComponent<CardManagerScript>().Slot1.GetComponent<CardSetup>().gadget = inventarArray[0];       
        }
        //Slot 2
        if (inventarArray.Length >= 2)
        {
            cardManager.GetComponent<CardManagerScript>().Slot2.GetComponent<CardSetup>().gadget = inventarArray[1];        
        }
        //Slot 3
        if (inventarArray.Length >= 3)
        {
            cardManager.GetComponent<CardManagerScript>().Slot3.GetComponent<CardSetup>().gadget = inventarArray[2];           
        }
        //Slot 4
        if (inventarArray.Length >= 4)
        {
            cardManager.GetComponent<CardManagerScript>().Slot4.GetComponent<CardSetup>().gadget = inventarArray[3];
        }
        //Slot 5
        if (inventarArray.Length >= 5)
        {
            cardManager.GetComponent<CardManagerScript>().Slot5.GetComponent<CardSetup>().gadget = inventarArray[4];
        }
        //Slot 6
        if (inventarArray.Length >= 6)
        {
            cardManager.GetComponent<CardManagerScript>().Slot6.GetComponent<CardSetup>().gadget = inventarArray[5];
        }   
    }

}
