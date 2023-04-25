using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Diese Klasse ist nur dafür da,
 * damit das GameObject mit dem GameHandler
 * über das ganze Spiel hinauslebt
 */
public class LongLiveTheKing : MonoBehaviour
{

    void Start()
    {
        //Debug.Log(this.gameObject.name);
        DontDestroyOnLoad(this.gameObject);
    }
}
