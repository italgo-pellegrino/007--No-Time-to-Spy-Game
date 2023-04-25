using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * @author Italgo Pellegrino
 * Diese Klasse ermöglicht durch Selektionieren einer Charakterkarte, das hinzufügen von Gadgets zu den Inventare der Charaktere.
 * Außerdem deaktiviert sie bereits ausgewählte Gadgets.
 **/
public class ItemSlot : MonoBehaviour, IDropHandler
{
    private GameObject highlightedCard;
    private GameObject cardManager;
    private string guid;

    /**
     * Durch Selektionieren eines Charakter, wird das inventar des Charakter dargestellt und deaktiviert bereits ausgewählte Gadgets
     */
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            //Implementiert die Funktion das die ausgewählte Charakterkarte, den Gadget über Drag and Drop(EquipSlotManager) im Inventar hinzugefügt wird
            guid = highlightedCard.GetComponent<CardSetup>().characterGuid;

            //Erster Charakter
            if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().One.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {
                //Hinzufügen ins Inventar
                cardManager.GetComponent<SlotData>().inventar1.Add(eventData.pointerDrag.GetComponent<CardSetup>().gadgetCopy);
                //Deaktiviert den Gadget und macht ihn durchsichtiger
                eventData.pointerDrag.GetComponent<EquipSlotManager>().alphaEnd = 0.6f;
                eventData.pointerDrag.GetComponent<EquipSlotManager>().enabled = false;

            }
            //Zweiter Charakter
            if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().Two.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {
                cardManager.GetComponent<SlotData>().inventar2.Add(eventData.pointerDrag.GetComponent<CardSetup>().gadgetCopy);
                eventData.pointerDrag.GetComponent<EquipSlotManager>().alphaEnd = 0.6f;
                eventData.pointerDrag.GetComponent<EquipSlotManager>().enabled = false;
            }
            //Dritter Charakter
            if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().Three.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {
                cardManager.GetComponent<SlotData>().inventar3.Add(eventData.pointerDrag.GetComponent<CardSetup>().gadgetCopy);
                eventData.pointerDrag.GetComponent<EquipSlotManager>().alphaEnd = 0.6f;
                eventData.pointerDrag.GetComponent<EquipSlotManager>().enabled = false;
            }
            //Vierter Charakter
            if (highlightedCard.GetComponent<CardSetup>().characterGuid == cardManager.GetComponent<CardManagerScript>().Four.GetComponent<CardSetup>().characterGuid && highlightedCard.GetComponent<CardSetup>().characterGuid != "")
            {
                cardManager.GetComponent<SlotData>().inventar4.Add(eventData.pointerDrag.GetComponent<CardSetup>().gadgetCopy);
                eventData.pointerDrag.GetComponent<EquipSlotManager>().alphaEnd = 0.6f;
                eventData.pointerDrag.GetComponent<EquipSlotManager>().enabled = false;
            }
            //Diese Zeile lässt bei Erfolgreichem Drag And Drop die Karte in den Slot "fallen"
            eventData.pointerDrag.GetComponent<RectTransform>().SetPositionAndRotation(eventData.pointerDrag.GetComponent<EquipSlotManager>().startPosition, Quaternion.identity);
        }
    }
    public void Start()
    {
        cardManager = GameObject.FindGameObjectWithTag("CardManager");
    }

    public void Update()
    {
        //Sucht immer wieder die Karte die ausgewählt wurde
        highlightedCard = GameObject.FindGameObjectWithTag("HighlightCard");
    }
}