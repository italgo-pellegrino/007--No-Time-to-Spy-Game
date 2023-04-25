using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Dieser Skript definiert das Verhalten vom PauseButton
 */
public class PauseButton : MonoBehaviour
{
    public Button button;
    public GameHandler gameHandler;

    // Start is called before the first frame update
    void Start()
    {
        setGameHandler(ref Connection.gameHandler);
        button = gameObject.GetComponent<Button>();
        //click Funktion
        button.onClick.AddListener(pauseToggle);
    }

    //Es wird mit der Referenz des GameHandlers gearbeitet, da der GameHandler standig geupdatet wird
    //Es wird darauf geachtet, dass auf den GameHandler nur ein lesender Zugriff erfolgt!!
    public void setGameHandler(ref GameHandler gameHandlerReference)
    {
        gameHandler = gameHandlerReference;
    }


    /**
     * sends pause request if game is going on and
     * continue request if game is paused
     */
    void pauseToggle()
    {
        gameHandler.sendPauseRequest();
    }


}
