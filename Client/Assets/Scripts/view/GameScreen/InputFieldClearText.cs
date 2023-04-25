using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldClearText : MonoBehaviour, IPointerClickHandler
{
    
    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        InputField inputfield = gameObject.GetComponent<InputField>();
        inputfield.text = "";
    }   
}
