using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * @author Italgo Pellegrino
 * Auf diese Klasse lässt isch zugreifen um den Kartennamen und Kartentext zu verändern. Wird für die Charaktere benötigt.
 */
public class TextChanger : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public string NameString;

    public TextMeshProUGUI Text;
    public string TextString;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Name.text = NameString;
        Text.text = TextString;
    }
}
