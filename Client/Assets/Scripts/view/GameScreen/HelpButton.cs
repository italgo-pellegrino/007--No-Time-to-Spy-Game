using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    //Attribute im Inspector gesetzt
    public Button button;
    public Canvas helpMenu;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(displayHelpMenu);
    }

    void displayHelpMenu()
    {
        // play how to use sound - to test
        this.gameObject.GetComponent<AudioSource>().Play();
        // activate Help Menu
        helpMenu.gameObject.SetActive(true);
    }
}
