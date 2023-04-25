using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Button um Spiel zu verlassen
 */
public class ExitButton : MonoBehaviour
{
    public Button button;
    public GameHandler gameHandler;

    // Start is called before the first frame update
    void Start()
    {
        setGameHandler(ref Connection.gameHandler);
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(pauseToggle);
    }

    //Es wird mit der Referenz des GameHandlers gearbeitet, da der GameHandler standig geupdatet wird
    //Es wird darauf geachtet, dass auf den GameHandler nur ein lesender Zugriff erfolgt!!
    public void setGameHandler(ref GameHandler gameHandlerReference)
    {
        gameHandler = gameHandlerReference;
    }


    /**
     * sends game leave request (game handler also changes
     * screen to main menu)
     */
    void pauseToggle()
    {
        gameHandler.sendGameLeaveMessage();
    }


}
