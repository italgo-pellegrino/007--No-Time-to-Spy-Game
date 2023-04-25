using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/**
 * @author Italgo Pellegrino
 * Diese Klasse Implementiert die Steuerung der Karten. Anhand vom GadgetEnum oder der Guid der Charaktere lassen sich diese
 * Karten steuern.
 * */
public class CardSetup : MonoBehaviour
{
    //Steuerung der Karten
    public GadgetEnum gadget;
    public GenderEnum character;
    [HideInInspector]public GadgetEnum gadgetCopy;
    public string characterGuid;

    //Extra Funktionen wie Skalierung ändern oder die Ausrüstungsphasen Drag & Drop Funktionen
    public float scale;
    public bool changeScale;
    public bool AusrüstungsPhase;

    //Dienen dazu neue Karten zu setzen
    private GenderEnum oldCharacter;
    private string oldCharacterGuid;
    private GadgetEnum oldGadget;
    private GameObject newCard;
    [HideInInspector] public List<GadgetEnum> inventar;

    /**
     * Eine Karte wird auf der Stelle und in der Größe der Musterkarte erzeugt. Benötigt den "Replacer",
     * "EquipslotManager und "TextChanger" Skript um Korrekt zu funktionieren.
     **/
    private void setCard(string name, string path)
    {
        //gadgetCopy wird benötigt um den neuen Gadgetkarten zu wechseln, da es ansonsten zu Bugs kommt wenn man direkt
        //mit gadget arbeiten würde
        gadgetCopy = gadget;
        string gadgetStr = gadget.ToString();

        //Kümmert sich um die Gadgets, dafür muss die characterGuid leer sein.  
        if (gadgetStr == name && characterGuid == "")
        {
            //falls eine newCard bereits existiert wird diese zerstört um zu verhindern das es immer mehr GameObjects
            //instaziert werden
            if (newCard != null)
            {
                Destroy(newCard);
            }

            if (name != "None")
            {
                //Eine Neue Karte wird aus dem Prefab was sich im "path" befindet instaziert. Diese Karte hat alle Werte
                //auf NUll, desahalb muss die characterGuid und gadgetCopy kopiert werden
                newCard = (GameObject)Instantiate(Resources.Load(path), gameObject.transform.position, Quaternion.identity);
                newCard.GetComponent<CardSetup>().characterGuid = "";
                newCard.GetComponent<CardSetup>().gadgetCopy = gameObject.GetComponent<CardSetup>().gadgetCopy;

                //Falls erwünscht kann man die Größe der Karte ändern indem man changeScale im Inspektor aktiviert
                if (changeScale)
                {
                    newCard.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                }
                //Für die Ausrüstungsphase werden einige andere Funktionen gebraucht, dies lässt sich im Inspektor aktivieren.
                //Diese Funktionen dienen hauptsächlich um den Drag & Drop Teil zu implementieren.
                if (SceneManager.GetActiveScene().name.Equals("AusrüstungphaseScene"))
                {
                    if (gameObject.tag == "slot")
                    {
                        newCard.tag = "newSlot";
                    }
                    newCard.GetComponent<Canvas>().sortingOrder = 1;
                    newCard.GetComponent<Replacer>().enabled = false;
                    if (newCard.tag != "newSlot")
                    {
                        newCard.AddComponent<CanvasGroup>();
                        newCard.AddComponent<EquipSlotManager>();
                        newCard.GetComponent<CanvasGroup>().interactable = true;                       
                        newCard.GetComponent<EquipSlotManager>().canvas = newCard.GetComponent<Canvas>().rootCanvas;
                    }
                }
            }
            //Um auf neue Messages zu reagieren, muss man zwischen alte Gadget und neue Unterscheiden können
            oldGadget = gadget;
        }

        //Kümmert sich um die Charactere, dafür muss das Gadget auf "None" gesetzt sein.  
        if (gadgetStr == "None" && character.ToString() == name)
        {
            //falls eine newCard bereits existiert wird diese zerstört um zu verhindern das es immer mehr GameObjects
            //instaziert werden
            if (newCard != null)
            {
                Destroy(newCard);
            }
            //Instaziert die Characterkarte die zunächst nur den richtigen Geschlecht hat. Hier werden die restliche Infos
            //kopiert, falls null kann keine Karte Instaziert werden und dieser Schritt wird übersprungen.
            if (Resources.Load(path) !=null)
            {
                newCard = (GameObject)Instantiate(Resources.Load(path), gameObject.transform.position, Quaternion.identity);
                newCard.GetComponent<CardSetup>().characterGuid = gameObject.GetComponent<CardSetup>().characterGuid;
                newCard.GetComponent<CardSetup>().gadgetCopy = gameObject.GetComponent<CardSetup>().gadgetCopy;
                newCard.GetComponent<TextChanger>().NameString = gameObject.GetComponent<TextChanger>().NameString;
                newCard.GetComponent<TextChanger>().TextString = gameObject.GetComponent<TextChanger>().TextString;
                if (changeScale)
                {
                    newCard.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                }

            }
            
           
            oldCharacterGuid = characterGuid;
            oldCharacter = character;
        }
    }

    /**
      * Diese Methode is dafür zuständig alle Gadgetkarten zu setzen
      * 
      **/
    private void setGadget()
    {
        if (SceneManager.GetActiveScene().name.Equals("AusrüstungphaseScene"))
        {
            setCard("None", "");
        }

        setCard("HAIRDRYER", "Prefab/Gadgets/Akku-Föhn");
        setCard("MOLEDIE", "Prefab/Gadgets/Maulwürfel");
        setCard("TECHNICOLOUR_PRISM", "Prefab/Gadgets/Technicolor_Prisma");
        setCard("BOWLER_BLADE", "Prefab/Gadgets/Klingenhut");
        setCard("MAGNETIC_WATCH", "Prefab/Gadgets/Magnetfeld-Armbanduhr");
        setCard("POISON_PILLS", "Prefab/Gadgets/Giftpillen-Flasche");
        setCard("LASER_COMPACT", "Prefab/Gadgets/Laser-Puderdose");
        setCard("ROCKET_PEN", "Prefab/Gadgets/Raketenwerfer-Füllfederhalter");
        setCard("GAS_GLOSS", "Prefab/Gadgets/Gaspatronen-Lippenstift");
        setCard("MOTHBALL_POUCH", "Prefab/Gadgets/Mottenkugel-Beutel");
        setCard("FOG_TIN", "Prefab/Gadgets/Nebeldose");
        setCard("GRAPPLE", "Prefab/Gadgets/Wurfhacken");
        setCard("WIRETAP_WITH_EARPLUGS", "Prefab/Gadgets/WanzeHörstöpsel");
        setCard("JETPACK", "Prefab/Gadgets/JetPack");
        setCard("CHICKEN_FEED", "Prefab/Gadgets/Chicken_Feed");
        setCard("NUGGET", "Prefab/Gadgets/Nugget");
        setCard("MIRROR_OF_WILDERNESS", "Prefab/Gadgets/Mirror");
        setCard("POCKET_LITTER", "Prefab/Gadgets/Pocket_Litter");
        setCard("Cocktail", "Prefab/Card1");
        setCard("ANTI_PLAGUE_MASK", "Prefab/Gadgets/Anti-Seuchen-Maske");
    }

    /**
      * Diese Methode is dafür zuständig alle Characterkarten zu setzen
      **/
    private void setCharacter()
    {       
        setCard("MALE", "Prefab/Character/Male");
        setCard("FEMALE", "Prefab/Character/Female");
        setCard("DIVERSE", "Prefab/Character/Diverse");
    }

    void Start()
    {
        //setzt die erste Iteration an Karten
        setGadget();
        setCharacter();
    }
    private void Update()
    {
        //stellt sicher das sich die Gadgetkarten ändern, sobald eine neue Nachricht kommt. 
        if (oldGadget != gadget)
        {
            setGadget();
        }     
        
        if (gameObject.GetComponent<CardSetup>().gadget.Equals(GadgetEnum.None) && AusrüstungsPhase == true)
        {
            Destroy(newCard);
        }
                  
        //stellt sicher das sich die Charakterkarten ändern, sobald eine neue Nachricht kommt. 
        if (oldCharacterGuid != characterGuid)
        {
            setCharacter();
        }
    }

}


