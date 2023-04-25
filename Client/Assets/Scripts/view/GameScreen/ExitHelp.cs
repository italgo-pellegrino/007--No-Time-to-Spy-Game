using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// deactivate help menu on gamescreen
public class ExitHelp : MonoBehaviour
{
    public Button button;
    public Canvas helpCanvas;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(closeHelp);
    }

    void closeHelp()
    {
        helpCanvas.gameObject.SetActive(false);
    }
}
