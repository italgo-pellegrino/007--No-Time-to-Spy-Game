using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/**
 * @author Italgo Pellegrino
 * Karten mit diesen Script, können Selektioniert werden und in Groß dargestellt werden.
 **/
public class Replacer : MonoBehaviour, IPointerDownHandler
{
    //Die neue ausgewählte Karte
    private GameObject newCard;

    private void Start()
    {       
    }

    /**
     * Die alte ausgewählte Karte wird zerstört(Nur die Kopie in groß), außerdem
     * wird eine neue in der Mitte des Bildschirm Instaziert. Sie wird außerdem
     * vergrößert und mit dem Tag "HighlightCard" versehen.
     */
    public void OnPointerDown(PointerEventData eventData)
    {       
        Destroy(GameObject.FindGameObjectWithTag("HighlightCard"));
        
        //Die Neue Karte ist eine Kopie der Karte, die diesen Script trägt
        newCard = Instantiate(gameObject,new Vector3 (988,540,0),Quaternion.identity);
        newCard.transform.localScale= new Vector3(1.2f,1.2f,1.2f);
        newCard.name = "HighlightedCard";
        newCard.tag = "HighlightCard";
        newCard.GetComponent<CanvasScaler>().scaleFactor = 1;     
    }
}