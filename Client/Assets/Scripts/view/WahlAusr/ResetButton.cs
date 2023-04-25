using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author Italgo Pellegrino
 * Implementiert die Reset Funktion der Ausrüstungsphase.
 * 1.Das Inventar der Charaktere wird geleert. 
 * 2.Die Gadgetkopien bekommen nochmal den GadgetEnum den sie haben sollten.
 * 3.Durch die Vertauschung der Karten werdend die Karten neu gesetzt und damit stehen wieder alle zur Verfügung.
 **/
public class ResetButton : MonoBehaviour
{
    public Button yourButton;
    GameObject cardManager;
    GadgetEnum gadget1Copy;
    GadgetEnum gadget2Copy;
    GadgetEnum gadget3Copy;
    GadgetEnum gadget4Copy;
    GadgetEnum gadget5Copy;
    GadgetEnum gadget6Copy;

    private void Start()
    {
        cardManager = GameObject.FindGameObjectWithTag("CardManager");
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    /**
     * Die Karten werden Zurückgesetzt
     */
    void TaskOnClick()
    {
        //Das Inventar der Charaktere wird geleert. 
        cardManager.GetComponent<SlotData>().inventar1.Clear();
        cardManager.GetComponent<SlotData>().inventar2.Clear();
        cardManager.GetComponent<SlotData>().inventar3.Clear();
        cardManager.GetComponent<SlotData>().inventar4.Clear();

        //Die Gadgetkopien bekommen nochmal den GadgetEnum den sie haben sollten.
        gadget1Copy = cardManager.GetComponent<CardManagerScript>().Gadget1.GetComponent<CardSetup>().gadget;
        gadget2Copy = cardManager.GetComponent<CardManagerScript>().Gadget2.GetComponent<CardSetup>().gadget;
        gadget3Copy = cardManager.GetComponent<CardManagerScript>().Gadget3.GetComponent<CardSetup>().gadget;
        gadget4Copy = cardManager.GetComponent<CardManagerScript>().Gadget4.GetComponent<CardSetup>().gadget;
        gadget5Copy = cardManager.GetComponent<CardManagerScript>().Gadget5.GetComponent<CardSetup>().gadget;
        gadget6Copy = cardManager.GetComponent<CardManagerScript>().Gadget6.GetComponent<CardSetup>().gadget;

        //Durch die Vertauschung der Karten werdend die Karten neu gesetzt und damit stehen wieder alle zur Verfügung.
        cardManager.GetComponent<CardManagerScript>().Gadget1.GetComponent<CardSetup>().gadget = gadget2Copy;
        cardManager.GetComponent<CardManagerScript>().Gadget2.GetComponent<CardSetup>().gadget = gadget1Copy;
        cardManager.GetComponent<CardManagerScript>().Gadget3.GetComponent<CardSetup>().gadget = gadget4Copy;
        cardManager.GetComponent<CardManagerScript>().Gadget4.GetComponent<CardSetup>().gadget = gadget3Copy;
        cardManager.GetComponent<CardManagerScript>().Gadget5.GetComponent<CardSetup>().gadget = gadget6Copy;
        cardManager.GetComponent<CardManagerScript>().Gadget6.GetComponent<CardSetup>().gadget = gadget5Copy;

    }

}
