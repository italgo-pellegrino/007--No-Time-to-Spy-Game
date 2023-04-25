using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Skript dient dazu, dass man zum Hauptmenü weitergeleitet wird,
 * wenn man im Stats Canvas auf Exit Button drückt
 */
public class ExitStats : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(button_action);
    }

    void button_action()
    {
        Connection.gameHandler.sendGameLeaveMessage();
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}
