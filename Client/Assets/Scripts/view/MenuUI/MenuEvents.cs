using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text.RegularExpressions;
using System;

//Zum Wechseln der Szene
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

/**
    Dieses Skript ist eine Komponente des Verbinden Buttons
*/
public class MenuEvents : MonoBehaviour
{
    GameHandler gameHandler;

    void Start()
    {
        gameHandler = Connection.gameHandler;
    }

    /**
    Methode, um eine Anwendung zu schließen
    **/
    public void closeApp()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    /**
        Diese Variable zeigt an, ob eine Verbindung besteht @deprecaded
    **/

    //private Boolean connected;

    /**
        Methode, um eine Verbindung zum Server aufzubauen
    **/
    public void buildConnection()
    {
        //Variablen für IP Adresse und port, Username
        string adress, port, username;
        adress = port = username = "";
        GameObject adress_input, port_input, username_input;

        //Referenzierung der Objekte in der Spielwelt
        adress_input = GameObject.Find("ServerAdress");
        port_input = GameObject.Find("PortNumber");
        username_input = GameObject.Find("Username");

        //Belegung der String Objekte mit dem Inhalt aus den jeweiligen Input Field in der GUI
        assignText(ref adress, adress_input);
        assignText(ref port, port_input);
        assignText(ref username, username_input);

        /*
        Alle möglichen Input Fehleingaben werden hier aufgefangen!!
        */
        string warning = "";

        //Erst soll sichergegangen werden, dass der Input gegeben ist
        Boolean inputgiven = true;

        //Alle bisherigen Warnungen werden gelöscht
        deleteLabelText("ConnectionProblem");

        //Leeres IP Adresse Feld
        if (adress.Equals(""))
        {
            //TODO Leeres User Inputfeld -> Sinnvoller Warnung an Benutzer 
            //EditorUtility.DisplayDialog("title", "message", "ok", "cancel");
            inputgiven = false;
            warning = "Die IP-Adresse fehlt! \n";
            changeLabelText("ConnectionProblem", warning);
        }

        //Leeres Port Nummer Feld
        if (port.Equals(""))
        {
            inputgiven = false;
            warning = "Die Portnummer fehlt! \n";
            changeLabelText("ConnectionProblem", warning);
        }

        //Leeres Username Feld
        if (username.Equals(""))
        {
            inputgiven = false;
            warning = "Ein Username wurde nicht eingegeben! \n";
            changeLabelText("ConnectionProblem", warning);
        }

        if (!inputgiven) return;
        //Inputfelder sind nicht leer, jetzt wird geprüft, ob der Input valide ist

        Boolean inputvalid = true;

        /**
        Ungültige IP Adresse
        **/
        //string input = "";
        if (!Regex.Match(adress, "^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$").Success
            && !adress.Equals("localhost"))
        {
            inputvalid = false;
            warning = "Ungültige IP Adresse! \n";
            changeLabelText("ConnectionProblem", warning);
        }
        else
        {
            string[] split = adress.Split(new Char[] { '.' });
            foreach (string s in split)
            {
                int ip;
                Int32.TryParse(s, out ip);
                if (!(ip >= 0 && ip <= 255))
                {
                    inputvalid = false;
                    warning = "Ungültige IP Adresse (in der Form x.x.x.x)! \n";
                    changeLabelText("ConnectionProblem", warning);
                }
            }
        }

        /**
        Ungültige Portnummer
        **/
        int portnumber = 0;

        if (!Regex.Match(port, "^[0-9]{1,5}$").Success)
        {
            inputvalid = false;
            warning = "Ungültige Portnummer (zwischen 0 und 65535)! \n";
            changeLabelText("ConnectionProblem", warning);
        }
        else
        {
            Int32.TryParse(port, out portnumber);
            if (!(portnumber >= 0 && portnumber <= 65535))
            {
                inputvalid = false;
                warning = "Ungültige Portnummer (zwischen 0 und 65535)! \n";
                changeLabelText("ConnectionProblem", warning);
            }
        }

        if (!inputvalid) return;

        /**
        Input des Benutzers passt. Nun wird versucht eine Verbindung aufzubauen!
        **/

        //TODO Verbindung aufbauen!!!

        Connection.connectToServer(adress, portnumber);


        //...

        //Falls eine Verbindung besteht
        //connected = true;
        //

        /**
        Falls Verbindung besteht, werden die Button aktiviert und das Label auf connected geändert, sonst ...
        **/

        /*
        if(connected){
            changeButtonInteraction("PlayButton", true);
            changeButtonInteraction("WatchButton", true);
            deleteLabelText("ConnectionState");
            changeLabelText("ConnectionState", "connected");
        }
        else{
            changeButtonInteraction("PlayButton", false);
            changeButtonInteraction("WatchButton", false);
            deleteLabelText("ConnectionState");
            changeLabelText("ConnectionState", "No Connection");
        }
        */

        //Name des Spielers
        uname = username;

    }

    /**
        Methode um ein Button zu aktivieren oder deaktivieren
        ButtonName: Name des Buttons zur Referenzierung,
        inter: true zur Aktivierung, sonst false
    **/
    private void changeButtonInteraction(string ButtonName, Boolean inter)
    {
        GameObject obj = GameObject.Find(ButtonName);
        Component comp = obj.GetComponent<Button>();
        Button button = (Button)comp;
        button.interactable = inter;
    }


    /**
        Text des Labels wird gesetzt, bzw. konkateniert,
        LabelName: Name des Labels zur Referenzierung,
        warning: zu setzende Text
    **/
    public static void changeLabelText(string LabelName, string warning)
    {
        GameObject obj = GameObject.Find(LabelName);
        Component comp = obj.GetComponent<Text>();
        Text t = (Text)comp;
        t.text += warning;
    }

    /**
        Text des Labels wird gelöscht,
        LabelName: Name des Labels zur Referenzierung
    **/
    private void deleteLabelText(string LabelName)
    {
        GameObject obj = GameObject.Find(LabelName);
        Component comp = obj.GetComponent<Text>();
        Text t = (Text)comp;
        t.text = "";
    }


    /**
    Diese Methode weist der String Variable input den Textinhalt vom GameObject gObject zu, wobei gObject vom Typ InputField ist
    **/
    private void assignText(ref string input, GameObject gObject)
    {
        Component comp = gObject.GetComponent<InputField>();
        InputField i = (InputField)comp;
        input = i.text;
    }

    public static bool conn_button_pressed;
    private bool change_done;

    void Update()
    {
        //Es muss ständig der Zustand der Verbindung überprüft werden!
        if (conn_button_pressed && Connection.isWebSocketOpen() && !change_done)
        {
            change_done = true;
            changeButtonInteraction("PlayButton", true);
            changeButtonInteraction("WatchButton", true);
            deleteLabelText("ConnectionState");
            changeLabelText("ConnectionState", "connected");
        }
        else if(conn_button_pressed && !Connection.isWebSocketOpen() && change_done)
        {
            change_done = false;
            changeButtonInteraction("PlayButton", false);
            changeButtonInteraction("WatchButton", false);
            deleteLabelText("ConnectionState");
            changeLabelText("ConnectionState", "No Connection");
        }

    }

    /**
        Diese Methode schickt dem Server eine Nachricht, in dem sich der Benutzer als Zuschauer registriert
    **/
    public void watchGame()
    {
        gameHandler.sendHelloMessage(uname, RoleEnum.SPECTATOR);
    }

    /*
        Diese Methode schickt dem Server eine Nachricht, in dem sich der Benutzer als Spieler registriert
    */
    public void playGame()
    {
        gameHandler.sendHelloMessage(uname, RoleEnum.PLAYER);
    }


    /*
        Name des Spielers, im InputFeld angegeben
    */
    private static string uname;

    //3 weitere Funktionen 

    /*
        Falls es beim Verbinden zum Server ein Problem auftaucht, so soll dieser Fehler dem Benutzer angezeigt
        @param error: Typ des Fehlers
        @param debugMessage: DebugMessage im MessageContainer, falls error = General

        Im Hauptmenü können zwei mögliche Fehler auftreten:
            - NAME_NOT_AVAILABLE: Der Name, der ausgesucht wurde, ist schon vergeben
            - ALREADY_SERVING: Der Server ist bereits belegt
            - General: Ein generelles Problem ist aufgetreten, die debugMessage sagt Weiteres aus
    */
    public static void ConnectionError(ErrorTypeEnum error, string debugMessage = "")
    {
        switch (error)
        {
            case ErrorTypeEnum.NAME_NOT_AVAILABLE:
                changeLabelText("ConnectionProblem", "Der gewünschte Name ist bereits vergeben!");
                break;
            case ErrorTypeEnum.ALREADY_SERVING:
                changeLabelText("ConnectionProblem", "Der Server ist bereits belegt!");
                break;
            case ErrorTypeEnum.GENERAL:
                changeLabelText("ConnectionProblem", debugMessage);
                break;
        }
    }

    /**
        Diese Methode wird aufgerufen, wenn das Spiel aus Sicht eines Spielers beginnt!
    **/
    public static void gameHasStartedForPlayer(bool random)
    {
        if (random)
        {
            SceneManager.LoadScene("Scenes/GameScene");
        }
        else
        {
            SceneManager.LoadScene("Scenes/WahlphaseScene");
        }
    }

    /**
        Diese Methode wird aufgerufen, wenn das Spiel aus Sicht eines Spielers beginnt!
    **/
    public static void gameHasStartedForSpectator()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    }

    /**
     * Beim Aktivieren/Deaktivieren vom Random Checkbox wird die Zufallsmodus
     * ein/ausgeschaltet (aendert der Wert im GameHandler)
     */
    public void toggleRandomMode(bool checkBoxOn)
    {
        gameHandler.setRandomMode(checkBoxOn);
    }

}

