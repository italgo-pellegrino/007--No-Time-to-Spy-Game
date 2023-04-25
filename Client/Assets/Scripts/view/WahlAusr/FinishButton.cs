using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

/**
 *@author Italgo Pellegrino
 * Diese Klasse sammelt die Informationen aus SlotData(Das Inventar der Charaktere), bildet daraus ein Hashmap, zwischen den
 * Charakteren und ausgerüstete Gadgets und sendet die Nachricht zum Server.
 * +
 */
public class FinishButton : MonoBehaviour
{

    public Button yourButton;
    GameObject cardManager;
    GameHandler gameHandler;   
    Guid char1;
    Guid char2;
    Guid char3;
    Guid char4;

    // Start is called before the first frame update
    void Start()
    {
        cardManager = GameObject.FindGameObjectWithTag("CardManager");
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        gameHandler = Connection.gameHandler;
    }

    /**
     * sammelt die Informationen der Ausrüstungsphase und sendet die Nachricht zum Server
     */
    void TaskOnClick()
    {
        //Überprüft ob ein Character vorhanden ist und falls ja wird die jeweiligen Char Variable zugewiesen
        if (cardManager.GetComponent<CardManagerScript>().One.GetComponent<CardSetup>().characterGuid != "")
        {
            char1 = Guid.Parse(cardManager.GetComponent<CardManagerScript>().One.GetComponent<CardSetup>().characterGuid);
        }
        if (cardManager.GetComponent<CardManagerScript>().Two.GetComponent<CardSetup>().characterGuid != "")
        {
            char2 = Guid.Parse(cardManager.GetComponent<CardManagerScript>().Two.GetComponent<CardSetup>().characterGuid);
        }
        if (cardManager.GetComponent<CardManagerScript>().Three.GetComponent<CardSetup>().characterGuid != "")
        {
            char3 = Guid.Parse(cardManager.GetComponent<CardManagerScript>().Three.GetComponent<CardSetup>().characterGuid);
        }
        if (cardManager.GetComponent<CardManagerScript>().Four.GetComponent<CardSetup>().characterGuid != "")
        {
            char4 = Guid.Parse(cardManager.GetComponent<CardManagerScript>().Four.GetComponent<CardSetup>().characterGuid);
        }

        //Jedem Character wird ein eigener Inventar die mit zuvor zugeordneten Gadgets zugewiesen.
        GadgetEnum[] character1Array = cardManager.GetComponent<SlotData>().inventar1.ToArray();
        GadgetEnum[] character2Array = cardManager.GetComponent<SlotData>().inventar2.ToArray();
        GadgetEnum[] character3Array = cardManager.GetComponent<SlotData>().inventar3.ToArray();
        GadgetEnum[] character4Array = cardManager.GetComponent<SlotData>().inventar4.ToArray();

        //Wird benötigt um jeden Character eine Liste an Gadgets zu mappen
        HashSet<GadgetEnum> gadgetHash1 = new HashSet<GadgetEnum>();
        HashSet<GadgetEnum> gadgetHash2 = new HashSet<GadgetEnum>();
        HashSet<GadgetEnum> gadgetHash3 = new HashSet<GadgetEnum>();
        HashSet<GadgetEnum> gadgetHash4 = new HashSet<GadgetEnum>();
        Dictionary<Guid, HashSet<GadgetEnum>> equipmentMap = new Dictionary<Guid, HashSet<GadgetEnum>>();

        //Das Hash wird mit den Gadgetsinfos gefüllt
        for (int i=0; character1Array.Length > i; i++)
        {
            
            if (character1Array[i] != GadgetEnum.None && char1 != Guid.Empty) 
            {
                gadgetHash1.Add(character1Array[i]);
            }

        }
        for (int i = 0; character2Array.Length > i; i++)
        {
            if (character2Array[i] != GadgetEnum.None && char2 != Guid.Empty)
            {
                gadgetHash2.Add(character2Array[i]);
            }
        }
        for (int i = 0; character3Array.Length > i; i++)
        {
            if (character3Array[i] != GadgetEnum.None && char3 != Guid.Empty)
            {
                gadgetHash3.Add(character3Array[i]);
            }
        }
        for (int i = 0; character4Array.Length > i; i++)
        {
            if (character4Array[i] != GadgetEnum.None && char4 != Guid.Empty)
            {
                gadgetHash4.Add(character4Array[i]);
            }
        }

        //Falls Chararkter vorhanden wird Schlussendlich den Gadgethash den jeweiligen Character zugeordnet
        if(char1 != Guid.Empty)
        {
            equipmentMap.Add(char1, gadgetHash1);
        }
        if (char2 != Guid.Empty)
        {
            equipmentMap.Add(char2, gadgetHash2);
        }
        if (char3 != Guid.Empty)
        {
            equipmentMap.Add(char3, gadgetHash3);
        }
        if (char4 != Guid.Empty)
        {
            equipmentMap.Add(char4, gadgetHash4);
        }

        //Die EquipmentChoice wird gesendet und die GameScene wird geladen
        if (cardManager.GetComponent<CardManagerScript>().One.GetComponent<CardSetup>().characterGuid != "")
        {
            gameHandler.sendEquipmentChoiceMessage(new EquipmentChoiceMessage(gameHandler.playerId, DateTime.Parse(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")), equipmentMap));
            SceneManager.LoadScene("Scenes/GameScene");
        }
    }
}
