using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//Only for Testing
using UnityEngine.U2D;

/**
 * Diese Klasse ist eine erweiterte Version von der Tile Klasse in Unity Engine.
 * Ein GameTile hat 3 zusaetzliche GameObjekte, die auf dem Tile liegen
 * Die Positionen dieser Objekte werden an die Tile Location orientiert
 */
public class GameTile : Tile
{
    /**
     * feste Objekte wie Tische, Kamin, Tresore
     */
    public GameObject staticGameObject;
    /**
     * bewegliche GameObjekte (Agenten, Hausmeister)
     */
    public GameObject dynamicGameObject;

    /**
     * position des Tiles (wird glaub nie benutzt)
     */
    public Vector3Int tilePosition;
    /**
     * Gadget Objekt, das auf dem Tile liegt (Diamanthalsband oder Hut)
     */
    public GameObject gadget;

    /**
    Dieses Attribut gibt den Zustand des Feldes an
**/
    public FieldStateEnum stateEnum;
    /**
        Dieses Attribut gibt an, ob sich auf dem jeweiligen Feld Nebel befindet
    **/
    public GameObject fog;

    /**
        Dieses Attribut aktiviert den Modus "Sichtbarkeitseinschränkung durch Felder mit Agenten" bei Anwendung des Gadgets: Klingen-Huts
    **/
    public static bool raycast_specialMode = false;

    /**
     * Diese Methode dient dazu Tile Sprite zu aktualisieren und 
     * ein statisches Objekt auf dem Tile zu setzen
     * Die Methode wird jedes Mal aufgerufen wenn setTile oder refreshTile aufgerufen wird
     */
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = base.sprite;
        //Setzt einen KLON von staticGameObject auf dem Tile
        tileData.gameObject = this.staticGameObject;

        tilePosition = position;
    }

    /**
     * Dem staticGameObject Attribut wird einem Wert zugewiesen
     * aber das Objekt wird noch nicht auf dem Tile gesetzt (instantiiert)
     */
    public void setStaticGameObject(ref GameObject staticGameObject)
    {
        this.staticGameObject = staticGameObject;
    }

    /**
     * Dem dynamicGameObject Attribut wird einem Wert zugewiesen
     * das Objekt soll schon auf dem Feld existieren
     * hier wird er nur auf dem Tile platziert
     */
    public void setDynamicGameObject(ref GameObject dynamicGameObject)
    {
        if(dynamicGameObject != null)
        {
            // (x + 0.5) damit objekt in der Mitte steht
            dynamicGameObject.transform.localPosition = tilePosition + new Vector3(0.5f, 0, 0);
            // Sorting order gesetzt damit die Objekte, die unter im Map stehen vorne gezeigt sind
            dynamicGameObject.GetComponent<SpriteRenderer>().sortingOrder = -tilePosition.y + 1;
        }
        this.dynamicGameObject = dynamicGameObject;
    }

    /**
     * Dem Gadget Attribut wird einem Wert zugewiesen
     * das Objekt soll schon auf dem Feld existieren
     * hier wird er nur auf dem Tile platziert
     */
    public void setGadget(GameObject gadget)
    {
        if (this.gadget == null) // only set gadget if there is none on the field
        {
            if (gadget != null)
            {
                gadget.transform.localPosition = tilePosition + new Vector3(0.5f, 0, 0);
                // Sorting order gesetzt damit die Objekte, die unter im Map stehen vorne gezeigt sind
                gadget.GetComponent<SpriteRenderer>().sortingOrder = -tilePosition.y + 2;
            }
            this.gadget = gadget;
        }
    }

    /**
     * Falls kein Gadget mehr auf dem Feld liegt,
     * wird das Objekt gelöscht
     */
    public void destroyGadget()
    {
        if(this.gadget != null)
        {
            Destroy(this.gadget);
        }
    }

    public void setStateEnum(FieldStateEnum stateEnum)
    {
        this.stateEnum = stateEnum;
    }

    public FieldStateEnum GetStateEnum()
    {
        return stateEnum;
    }

    /**
     * Setzt Nebel auf dem Feld
     * Instantiiert den Prefabs neben auf dem Tile
     */
    public void setFog(bool foggy)
    {
        if (foggy && fog == null)
        {
            fog = Instantiate(Resources.Load("Prefabs/Fog")) as GameObject;
            fog.transform.localPosition = tilePosition + new Vector3(0.5f, 0, 0);
            fog.GetComponent<SpriteRenderer>().sortingOrder = -tilePosition.y + 2;
        }
        else if(!foggy && fog != null)
        {
            Destroy(fog);
        }
    }

}
