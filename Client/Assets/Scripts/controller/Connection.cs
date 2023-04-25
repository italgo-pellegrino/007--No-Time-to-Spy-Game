using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using UnityEditor;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Runtime.CompilerServices;

/**
 * @author Italgo Pellegrino
 * Diese Klasse stellt die Verbindung zwischen Client und Server her, kümmert sich um die korrekte Weiterleitung von Empfangene Nachricht
 * und dem korrektem Senden der Nachrichten
 */
public class Connection : MonoBehaviour
{
    static WebSocket websocket;
    public static GameHandler gameHandler = new GameHandler();

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    async void Update()
    {
        //Implementiert das automatische Wiederverbinden, falls die Verbindung abbricht
        if (websocket != null && websocket.State == WebSocketState.Closed && gameHandler.sessionId != Guid.Empty)
        {
            await websocket.Connect();
        }
    }

    /**
     * Versendet die HelloMessage
     */
    async public static void SendWebSocketMessage(HelloMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text
            string json = JsonConvert.SerializeObject(message);
            json = parseDate(json);
            await websocket.SendText(json);
        }
    }

    /**
     * Versendet die ReconnectMessage
     */
    async public static void SendWebSocketMessage(ReconnectMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text
            string json = JsonConvert.SerializeObject(message);
            json = parseDate(json);
            await websocket.SendText(json);
        }
    }

    /**
     * Versendet die ItemChoiceMessage
     */
    async public static void SendWebSocketMessage(ItemChoiceMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text           
            string json = JsonConvert.SerializeObject(message);
            json = parseDate(json);
            await websocket.SendText(json);
        }
    }

    /**
     * Versendet die EquipmentChoiceMessage
     */
    async public static void SendWebSocketMessage(EquipmentChoiceMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text
            string json = JsonConvert.SerializeObject(message);
            json = parseDate(json);
            await websocket.SendText(json);
        }
    }

    /**
     * Versendet die GameOperationMessage
     */
    async public static void SendWebSocketMessage(GameOperationMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text

            string json = JsonConvert.SerializeObject(message);
            json = parseDate(json);
            await websocket.SendText(json);
        }
    }

    /**
     * Versendet die GameLeaveMessage
     */
    async public static void SendWebSocketMessage(GameLeaveMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text

            string json = JsonConvert.SerializeObject(message);
            json = parseDate(json);
            await websocket.SendText(JsonConvert.SerializeObject(message));
        }
    }

    /**
     * Versendet die RequestGamePauseMessage
     */
    async public static void SendWebSocketMessage(RequestGamePauseMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text
            await websocket.SendText(JsonConvert.SerializeObject(message));
        }
    }

    /**
     * Versendet die RequestmetaInformationMessage
     */
    async public static void SendWebSocketMessage(RequestMetaInformationMessage message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });
            // Sending plain text
            await websocket.SendText(JsonConvert.SerializeObject(message));
        }
    }

    /**
     * Stellt die Websocketverbindung zum Server her.  
     */
    public static async void connectToServer(string ip, int port)
    {
        websocket = new WebSocket("ws://" + ip + ":" + port + "/websockets/game");

        //Die Verbindung wird eröffnet
        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            MenuEvents.conn_button_pressed = true;

            //Falls bereits eine sessionId exisitert, wird eine ReconnectMessage gesendet.
            if (gameHandler.sessionId != Guid.Empty)
            {
                Debug.Log(gameHandler.sessionId);
                SendWebSocketMessage(new ReconnectMessage(gameHandler.playerId, gameHandler.sessionId, DateTime.Now));
            }

        };

        //Verbindungs Fehlerfall
        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            MenuEvents.changeLabelText("ConnectionProblem", e);

        };

        //Falls Verbindung abbricht wird im Hauptmenü den Connection Button wieder aktiviert
        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed! " + e);
            MenuEvents.conn_button_pressed = false;
        };

        //Ankommende Nachrichten
        websocket.OnMessage += (bytes) =>
        {
            try
            {
                //Bytes werden in String umgewandelt
                var messageString = System.Text.Encoding.UTF8.GetString(bytes);
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy HH:mm:ss" };

                // Ausgabe/Logging der Nachrichten
                Debug.Log(messageString);

                //String Nachricht wird in einer MessageContainer Object umgewandelt, Datetime wird In der Form "dd.MM.yyyy HH:mm:ss" gebracht, da so vom Standard definiert
                MessageContainer messageJson = JsonConvert.DeserializeObject<MessageContainer>(messageString, dateTimeConverter);

                //Nun lässt sich den MessageType auslesen um die Korrekte Message zu erzeugen. Diese Message wird dann vom GameHandler gelesen und in der Client Logik integriert.
                switch (messageJson.type)
                {
                    case MessageTypeEnum.HELLO:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.HELLO_REPLY:
                        HelloReplyMessage helloReplyJson = JsonConvert.DeserializeObject<HelloReplyMessage>(messageString, dateTimeConverter);
                        gameHandler.onHelloReply(helloReplyJson);
                        break;
                    case MessageTypeEnum.RECONNECT:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.GAME_STARTED:
                        GameStartedMessage gameStartedJson = JsonConvert.DeserializeObject<GameStartedMessage>(messageString, dateTimeConverter);
                        gameHandler.onGameStarted(gameStartedJson);
                        break;
                    case MessageTypeEnum.REQUEST_ITEM_CHOICE:
                        RequestItemChoiceMessage requestItemChoiceJson = JsonConvert.DeserializeObject<RequestItemChoiceMessage>(messageString, dateTimeConverter);
                        gameHandler.onRequestItemChoice(requestItemChoiceJson);
                        break;
                    case MessageTypeEnum.ITEM_CHOICE:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.REQUEST_EQUIPMENT_CHOICE:
                        RequestEquipmentChoiceMessage requestEquipmentChoiceJson = JsonConvert.DeserializeObject<RequestEquipmentChoiceMessage>(messageString, dateTimeConverter);
                        gameHandler.onRequestEquipmentChoice(requestEquipmentChoiceJson);
                        break;
                    case MessageTypeEnum.EQUIPMENT_CHOICE:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.GAME_STATUS:
                        GameStatusMessage gameStatusJson = JsonConvert.DeserializeObject<GameStatusMessage>(messageString, dateTimeConverter);
                        GameStatusMessage gameStatusMessage = new GameStatusMessage(gameStatusJson.clientId, gameStatusJson.creationDate, gameStatusJson.activeCharacterId, statusOperations(messageString, gameStatusJson), statusState(messageString, gameStatusJson), gameStatusJson.isGameOver);
                        gameHandler.onGameStatus(gameStatusMessage);
                        break;
                    case MessageTypeEnum.REQUEST_GAME_OPERATION:
                        RequestGameOperationMessage requestGameOperationJson = JsonConvert.DeserializeObject<RequestGameOperationMessage>(messageString, dateTimeConverter);
                        gameHandler.onRequestGameOperation(requestGameOperationJson);
                        break;
                    case MessageTypeEnum.GAME_OPERATION:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.STATISTICS:
                        StatisticsMessage statisticsJson = JsonConvert.DeserializeObject<StatisticsMessage>(messageString, dateTimeConverter);
                        gameHandler.onStatistics(statisticsJson);
                        break;
                    case MessageTypeEnum.GAME_LEAVE:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.GAME_LEFT:
                        GameLeftMessage gameLeftJson = JsonConvert.DeserializeObject<GameLeftMessage>(messageString, dateTimeConverter);
                        gameHandler.onGameLeft(gameLeftJson);
                        break;
                    case MessageTypeEnum.REQUEST_GAME_PAUSE:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.GAME_PAUSE:
                        GamePauseMessage gamePauseJson = JsonConvert.DeserializeObject<GamePauseMessage>(messageString, dateTimeConverter);
                        gameHandler.onGamePauseMessage(gamePauseJson);
                        break;
                    case MessageTypeEnum.REQUEST_META_INFORMATION:
                        Debug.Log("Wrong Message");
                        break;
                    case MessageTypeEnum.META_INFORMATION:
                        MetaInformationMessage metaInfoJson = JsonConvert.DeserializeObject<MetaInformationMessage>(messageString, dateTimeConverter);
                        MetaInformationMessage metaInfoMsg = metaInfos(messageString, metaInfoJson);
                        gameHandler.onMetaInformation(metaInfoMsg);
                        break;
                    case MessageTypeEnum.STRIKE:
                        StrikeMessage strikeJson = JsonConvert.DeserializeObject<StrikeMessage>(messageString, dateTimeConverter);
                        gameHandler.onStrike(strikeJson);
                        break;
                    case MessageTypeEnum.ERROR:
                        ErrorMessage errorJson = JsonConvert.DeserializeObject<ErrorMessage>(messageString, dateTimeConverter);
                        gameHandler.onError(errorJson);
                        break;
                    case MessageTypeEnum.REQUEST_REPLAY:
                        Debug.Log("Wrong Message");
                        break;
                    default:
                        Console.WriteLine("Default");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        };
        //Verbindung wird hergestellt
        await websocket.Connect();
    }

    /**
     * Überprüft ob das Websocket offen ist
     */
    public static bool isWebSocketOpen()
    {
        return websocket.State == WebSocketState.Open;
    }

    /**
     * Diese Methode korigiert den DateTime sodass es Serverkonform ist. Nur bei Sendende Nachrichten benützt
     */
    private static string parseDate(string json)
    {
        json = json.Remove(json.IndexOf("Date"), 26);
        json = json.Replace("creation", "creationDate" + "\"" + ":" + "\"" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));

        return json;
    }

    /**
     * In einer GameStatus Nachricht gibt es eine Liste von Operationen, die spaeter in spezifische Klassen
     * downcastet werden müssen. Der Parser speichert alle Operationen als Base Operation, d.h. dass die
     * Nachricht-spezifische Informationen (wie from Attribut von Movement Operation) nicht rausgelesen
     * werden kann.
     * Diese Methode erzeugt eine lokale Liste von BaseOperations (lokal erzeugte Operationen können downcastet
     * werden), die danach zu einem Konstruktor direkt übergeben wird.
     */
    private static List<BaseOperation> statusOperations(string json, GameStatusMessage status)
    {
        // geparste Liste von Operations - Infos fehlen hier
        List<BaseOperation> op_list = new List<BaseOperation>();

        // abstraktes Objekt die aus JSON Nachricht rausgelesen wird
        JObject jObject = JObject.Parse(json);
        // Operations attribut des abstrakten Objekts
        JArray array = jObject.GetValue("operations").ToObject<JArray>();
        for (int i = 0; i < status.operations.Count; i++)
        {
            BaseOperation element = status.operations[i];
            // abstrakte Einzeloperationen
            JToken token = array[i];
            switch (element.type)
            {
                case OperationEnum.CAT_ACTION:
                case OperationEnum.JANITOR_ACTION:
                case OperationEnum.RETIRE:
                case OperationEnum.SPY_ACTION: //falls Spy-Action in Operation Objekt umgestellt
                    op_list.Add(token.ToObject<Operation>());
                    break;
                case OperationEnum.EXFILTRATION: //falls Exfiltration in Exfiltration Objekt umgestellt
                    op_list.Add(token.ToObject<Exfiltration>());
                    break;
                case OperationEnum.GADGET_ACTION: //falls Gadget-Aktion in GadgetAction Objekt umgestellt
                    op_list.Add(token.ToObject<GadgetAction>());
                    break;
                case OperationEnum.GAMBLE_ACTION: //you get the idea
                    op_list.Add(token.ToObject<GambleAction>());
                    break;
                case OperationEnum.MOVEMENT:
                    op_list.Add(token.ToObject<Movement>());
                    break;
                case OperationEnum.PROPERTY_ACTION:
                    op_list.Add(token.ToObject<PropertyAction>());
                    break;
            }
        }
        return op_list;
    }

    /**
     * In einer GameStatus Nachricht gibt es ein State Objekt, das downcastbare Attribute enthaelt,
     * wie mögliche Cocktail oder WiretapWithEarplugs Objekte der einzelnen Charaktere oder Cocktails
     * auf Tischen. Damit sie im View richtig angezeigt und behandelt werden, müssen Objekte den richtigen
     * Typen haben
     * Diese Methode ersetzt alle Vorkommen von diesen Gadgets von Gadget Objekt zu dem Entsprechenden um
     */
    private static State statusState(string json, GameStatusMessage status)
    {
        // geparste Status - fehlerhafte Gadgets
        State state = status.state;

        // Character Gadgets (Cocktail, WiretapWithEarplugs)
        List<Character> falseCharList = status.state.characters.ToList<Character>();
        // abstraktes Objekt aus dem JSON
        JObject jObject = JObject.Parse(json);
        // state attribut vom abstrakten Objekt
        JObject jState = jObject.GetValue("state").ToObject<JObject>();
        // char liste vom abstrakten state
        JArray jChars = jState.GetValue("characters").ToObject<JArray>();
        // richtige liste die wir auffüllen
        HashSet<Character> correctChars = new HashSet<Character>();
        for (int i = 0; i < falseCharList.Count; i++)
        {
            Character currentChar = falseCharList.ElementAt(i);
            //falls keine fehlerhafte gadgets einfach einfuegen
            if (currentChar.hasGadget(GadgetEnum.COCKTAIL) == null
                && currentChar.hasGadget(GadgetEnum.WIRETAP_WITH_EARPLUGS) == null)
            {
                correctChars.Add(currentChar);
                continue;
            }
            //sonst
            JObject jChar = jChars[i].ToObject<JObject>();
            // abstrakte gadget liste zum downcasten
            JArray jCharGadgets = jChar.GetValue("gadgets").ToObject<JArray>();
            // eigentliche liste um einfacher zu iterieren
            List<Gadget> charGadgetList = currentChar.gadgets.ToList<Gadget>();
            // gadget set die wir auffüllen
            HashSet<Gadget> charGadgets = new HashSet<Gadget>();
            for (int j = 0; j < charGadgetList.Count; j++)
            {
                //cocktail gadget
                if (currentChar.gadgets.ElementAt(j).gadget == GadgetEnum.COCKTAIL)
                {
                    //abstrakte gadget in cocktail umstellen
                    JToken jCharCocktail = jCharGadgets[j];
                    Cocktail charCocktail = jCharCocktail.ToObject<Cocktail>();
                    charGadgets.Add(charCocktail);
                    continue;
                }
                //ohrstoepsel gadget
                if (currentChar.gadgets.ElementAt(j).gadget == GadgetEnum.WIRETAP_WITH_EARPLUGS)
                {
                    //analog
                    JToken jCharCocktail = jCharGadgets[j];
                    WiretapWithEarplugs charEarplugs = jCharCocktail.ToObject<WiretapWithEarplugs>();
                    charGadgets.Add(charEarplugs);
                    continue;
                }
                //falls kein cocktail oder ohrstoepsel einfach einfügen
                charGadgets.Add(charGadgetList.ElementAt(j));
            }
            //aendere charakter object, speichere
            currentChar.gadgets = charGadgets;
            correctChars.Add(currentChar);
        }
        // ersetze eigentliche Charakter Liste mit dem erzeugten
        state.characters = correctChars;

        // Cocktails auf Tische - geht analog
        // anstatt neue liste zu erzeugen wir überschreiben gadget attribut des Feldes durch den Richtigen
        JObject jFieldmap = jState.GetValue("map").ToObject<JObject>();
        JArray jFieldMapArr = jFieldmap.GetValue("map").ToObject<JArray>(); //2d array with [][] format
        for (int i = 0; i < state.map.map.GetLength(0); i++)
        {
            for (int j = 0; j < state.map.map.GetLength(1); j++)
            {
                Field f = state.map.map[i, j];
                if (f.state == FieldStateEnum.BAR_TABLE && f.gadget != null && f.gadget.gadget == GadgetEnum.COCKTAIL)
                {
                    JObject jField = jFieldMapArr[i][j].ToObject<JObject>();
                    JToken jGadget = jField.GetValue("gadget");
                    Cocktail c = jGadget.ToObject<Cocktail>();
                    f.gadget = c;
                }
            }
        }
        return state;
    }

    /**
     * MetaInformationen müssen auch aus Objekten in Dictionary Downcastet werden
     * Ich frag nur nach Fraktionen nach, deshalb muss ich nur die gescheit darstellen können
     * Objekt -> Guid downcasting
     * UNSERE SERVER HAT META INFOS UNTERSCHIEDLICH AUFGEBAUT
     */
    private static MetaInformationMessage metaInfos(string json, MetaInformationMessage message)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        JObject jMessage = JObject.Parse(json);
        JObject jDict = jMessage.GetValue("information").ToObject<JObject>();
        JArray jP1 = jDict.GetValue("Faction.Player1").ToObject<JArray>();
        JArray jP2 = jDict.GetValue("Faction.Player2").ToObject<JArray>();
        Guid[] p1 = new Guid[jP1.Count];
        Guid[] p2 = new Guid[jP2.Count];
        for(int i=0; i<jP1.Count; i++)
        {
            JToken jGuid = jP1[i];
            p1[i] = jGuid.ToObject<Guid>();
        }
        for (int i = 0; i < jP2.Count; i++)
        {
            JToken jGuid = jP2[i];
            p2[i] = jGuid.ToObject<Guid>();
        }
        dict.Add("Faction.Player1", p1);
        dict.Add("Faction.Player2", p2);
        return new MetaInformationMessage(message.clientId, message.creationDate, (Dictionary<string,object>) dict);
    }

    /**
     * Falls die Applikation beendet wird, wird auch das Websocket geschlossen
     */
    private async void OnApplicationQuit()
    {
        if(websocket != null) // Damit keine Nullpointer vorkommen
        {
            await websocket.Close();
        }
    }

    public static async void closeWebsocket()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}
