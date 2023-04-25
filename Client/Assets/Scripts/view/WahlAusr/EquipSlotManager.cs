using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * @author Italgo Pellegrino
 * Diese Klasse ist ein MonoBehavior der ein GameObject die Eigenschaft gibt, mittels Drag and Drop bewegt zu werden.
 * */
public class EquipSlotManager : MonoBehaviour, IPointerDownHandler,IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] public Canvas canvas;   
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public float alphaEnd =1;

    private void Awake()
    {
        //Initialisiert die Variablem um den Drag and Drop zu Initialisieren
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPosition = gameObject.GetComponent<RectTransform>().localPosition;
        canvasGroup.alpha = 1;
    }

    /**
     * macht die Karte durchsichtig und deaktiviert die Raycast abfrage, da man während man 
     * zieht keine Kollisionen haben möchte
     */
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    /**
     * Beim ziehen wird die Position verändert.
     */
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta*1920/Screen.width;
    }

    /**
     * Beim loslassen, möchte man wieder eine Kolliderabfrage um in den "Slot zu fallen"
     */
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = alphaEnd;      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }


}

   

