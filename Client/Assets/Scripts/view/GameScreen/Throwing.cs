using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/**
    Diese Klasse ist für die Aniamtionen zuständig
    Konventionen werden vereinbart: 
        - Die Animationsclips haben den gleichen Namen wie die jeweiligen Zustände im State Machine
        - Alle Animationsclips haben die Länge 1 (Normierung)
        - Erstes Kind des Agenten: Wassertropfen (symblisch für klamme Klammotten Eigenschaft)
        - Zweites Kind des Agenten: Ohrstöpsel
        - Drittes Kind des Agenten: Enemy Label


        SEHR WICHTIG
        - Bei den Koordinaten handelt es sich um die Koordinaten in der View mit positivem x und negativem y
        - Bei den Zielkoordinaten soll es sich jeweils die Mitte des Zielfeldes handeln!!! 
        - Animiert wird weder Schaden, Erhalt von IP o.Ä.
**/
public class Throwing : MonoBehaviour
{
    /**
        Parameter, die für die Animationen wichtig sind ... 
    **/



    //Es werden alle Bilder geladen
    private static Sprite[] sprites;
    //Startpunkt
    private static float x_start,y_start;
    //Ziel
    private static float x_target;
    private static float y_target;
    //Dauer der Animation
    private static float duration = 3;
    //Startpunkt der Animation
    private static float startTime;

    //Animator des Agenten
    //private Animator anim;

    //Wurfobjekt
    private static GameObject Object;

    /**
        Dieses Array enthält alle Sprites, um die Zustände des Tisches abzubilden
        0: neutral
        1: mit unvergiftetem Cocktail
        2: mit vergiftetem Cocktail
    **/
    private static Sprite[] table_sprites;

    public void setDuration(float d)
    {
        duration = d;
    }

    void Start() {
        startTime = Time.time;
        //Sprite Sheet
        sprites = Resources.LoadAll<Sprite>("Sprites/SpriteSheet");
        //Zustand-Sprites des Tisches
        //table_sprites = Resources.LoadAll<Sprite>("Sprites/TableStates");
    }


    /**
    Diese Methode wird in Animationen benutzt, in denen das Objekt fest in der Hand gehalten wird und rotiert wird
    @params x_target,y_target: Zielkoordinaten
    @param object_inHand: Das Objekt, das in der Hand gehalten wird
    @param agent: Agent 
    @param withRotation: Soll das Objekt in der Hand des Agenten in die "richtige" Richtung rotiert werden oder nicht
    @param destroy_Object: Das Objekt, das demonstrativ gezeigt wird, soll zerstört werden
**/
    private static IEnumerator Holding_InHand(float x_target, float y_target, GameObject object_inHand, GameObject agent, bool withRotation, bool destroy_Object)
    {
        //startTime = Time.time;

        Debug.Log("HOLD");
        /**
            Animator des Agenten
        **/
        Animator anim = agent.GetComponent<Animator>();
        //Geschwindigkeit der Animationsabspielung wird auf duration angepasst
        //anim.speed = 1/duration;

        /**
            Koordinaten zur (feinen) Anpassung der Startposition des Wurfobjekts
        **/
        float x_adapt, y_adapt;
        x_adapt = y_adapt = 0;

        /**
            Positionen des Agenten
        **/
        float x_pos, y_pos;
        x_pos = (int)agent.transform.position.x;
        y_pos = (int)agent.transform.position.y;


        string anim_name = "";
        /**
        Bestimmung der Animationsrichtung, dabei wird auf die Spiegelung in x Richtung zurückgegriffen
        **/

        //Wurf nach oben
        if (x_pos == x_target && y_pos < y_target)
        {
            x_adapt = 0;
            y_adapt = 2.1f;
            anim_name = "Throw_Above";
        }
        //Wurf nach unten
        else if (x_pos == x_target && y_pos > y_target)
        {
            x_adapt = 0;
            y_adapt = 1.5f;
            anim_name = "Throw_Below";
        }
        //Wurf nach Rechts
        else if (x_pos < x_target && y_pos == y_target)
        {
            x_adapt = 0.3f;
            y_adapt = 1.55f;
            anim_name = "Throw_Right";
        }
        //Wurf nach Links
        else if (x_pos > x_target && y_pos == y_target)
        {
            x_adapt = -0.3f;
            y_adapt = 1.55f;
            object_inHand.transform.rotation = Quaternion.Euler(0, 180, 0);
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            anim_name = "Throw_Right";
        }
        //Wurf nach unten rechts (diagonal)
        else if (x_pos < x_target && y_pos > y_target)
        {
            x_adapt = 0.2f;
            y_adapt = 1.6f;
            anim_name = "Throw_BelowRight";
        }
        //Wurf nach unten links (diagonal)
        else if (x_pos > x_target && y_pos > y_target)
        {
            x_adapt = -0.2f;
            y_adapt = 1.6f;
            object_inHand.transform.rotation = Quaternion.Euler(0, 180, 0);
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            anim_name = "Throw_BelowRight";
        }
        //Wurf nach oben links (diagonal)
        else if (x_pos > x_target && y_pos < y_target)
        {
            x_adapt = -0.35f;
            y_adapt = 1.67f;
            anim_name = "Throw_AboveLeft";
        }
        //Wurf nach oben rechts (diagonal)
        else if (x_pos < x_target && y_pos < y_target)
        {
            x_adapt = 0.35f;
            y_adapt = 0.67f;
            object_inHand.transform.rotation = Quaternion.Euler(0, 180, 0);
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            anim_name = "Throw_AboveLeft";
        }

        anim.SetTrigger(anim_name);

        object_inHand.transform.SetParent(agent.transform);
        //Positionierung des Wurfobjekts
        object_inHand.transform.localPosition = new Vector3(x_adapt, y_adapt, 0);


        /**
            Sorting Layer Order Nummer des Wurfobjekts muss bestimmt werden!

            Wenn nach hinten geworfen wird, sieht man das Gadget trotzdem!!! - Discuss
        **/
        int max = Math.Max((int)Math.Abs(y_pos), (int)Math.Abs(y_target));
        SpriteRenderer object_sprenderer = object_inHand.GetComponent<SpriteRenderer>();
        object_sprenderer.sortingOrder = max + 1;

        /*
        //Rotation
        if(withRotation){
            object_inHand.transform.Rotate(0,0, calcRotation(Vector2.right, new Vector2(x_start,y_start), new Vector2(x_target, y_target)));
            //float angle = Mathf.Atan2(y_target, x_target) * Mathf.Rad2Deg;
            //Object1.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        */



        //Die Animation dauert duration lang
        yield return new WaitForSeconds(1);
        anim.ResetTrigger(anim_name);
        /**
            Nachdem das Gadget demonstrativ gezeigt wurde, wird dieses anschließend zerstört, nur wenn dies erwünscht ist
        **/
        //if(destroy_Object)
        object_inHand.transform.SetParent(null);
        Destroy(object_inHand);
    }

    /**
        Diese Methode ist für die Animierung der Aufnahme des Cocktails vom Tisch
        @animation: Falls animation true ist, wird die Methode animiert dagestellt, falls animation false ist, so wird diese Methode als Hilfsmethode benutzt
    **/

    /**
    Diese Methode ist das generalisierte Konstrukt aller Wurf-Animationen durch Gadgets!
    @param agent: Agent, der die Wurfanimation verursacht
    @param throwObject_path: Pfad, um das GameObject zu laden
    @param y_target,x_target: Zielkoordinaten
    @param rotation: Soll das Wurfobjekt rotiert werden

    Das Wurfobjekt wird vom Rescource Ordner geladen, geworfen und anschließend zerstört

**/
    private static IEnumerator Throw_Animation(float x_target, float y_target, GameObject agent, string throwObject_path, bool rotation)
    {
        Debug.Log("THROW");
        //startTime = Time.time;

        //SpriteRenderer spriteR = agent.GetComponent<SpriteRenderer>();

        /**
            Animator des Agenten
        **/
        Animator anim = agent.GetComponent<Animator>();
        //Geschwindigkeit der Animationsabspielung wird auf duration angepasst
        //anim.speed = 1 / duration;

        /**
            Koordinaten des Agenten
        **/
        float x_pos, y_pos;
        x_pos = (int) agent.transform.position.x;
        y_pos = (int) agent.transform.position.y;


        /**
            Koordinaten zur (feinen) Anpassung der Startposition des Wurfobjekts
        **/
        float x_adapt, y_adapt;
        x_adapt = y_adapt = 0;

        /**
        Bestimmung der Animationsrichtung, dabei wird auf die Spiegelung in x Richtung zurückgegriffen
        **/

        string anim_name = "";

        //Wurf nach oben
        if (x_pos == x_target && y_pos < y_target)
        {
            x_adapt = 0;
            y_adapt = 2.1f;
            anim_name = "Throw_Above";
        }
        //Wurf nach unten
        else if (x_pos == x_target && y_pos > y_target)
        {
            x_adapt = 0;
            y_adapt = 1.5f;
            anim_name = "Throw_Below";
        }
        //Wurf nach Rechts
        else if (x_pos < x_target && y_pos == y_target)
        {
            x_adapt = 0.3f;
            y_adapt = 1.55f;
            anim_name = "Throw_Right";
        }
        //Wurf nach Links
        else if (x_pos > x_target && y_pos == y_target)
        {
            x_adapt = -0.3f;
            y_adapt = 1.55f;
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            anim_name = "Throw_Right";
        }
        //Wurf nach unten rechts (diagonal)
        else if (x_pos < x_target && y_pos > y_target)
        {
            x_adapt = 0.2f;
            y_adapt = 1.6f;
            anim_name = "Throw_BelowRight";
        }
        //Wurf nach unten links (diagonal)
        else if (x_pos > x_target && y_pos > y_target)
        {
            x_adapt = -0.2f;
            y_adapt = 1.6f;
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            anim_name = "Throw_BelowRight";
        }
        //Wurf nach oben links (diagonal)
        else if (x_pos > x_target && y_pos < y_target)
        {
            x_adapt = -0.35f;
            y_adapt = 1.67f;
            anim_name = "Throw_AboveLeft";
        }
        //Wurf nach oben rechts (diagonal)
        else if (x_pos < x_target && y_pos < y_target)
        {
            x_adapt = 0.35f;
            y_adapt = 0.67f;
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            anim_name = "Throw_AboveLeft";
        }


        Debug.Log("Pos: (" + x_pos + "," + y_pos + ")");
        Debug.Log("Target: (" + x_target + "," + y_target + ")");

        if (anim_name.Equals(""))
        {
            yield break;
        }

        //Wurfobjekt
        GameObject Object1 = Instantiate(Resources.Load(throwObject_path, typeof(GameObject))) as GameObject;
        int max = Math.Max((int)Math.Abs(y_pos), (int)Math.Abs(y_target));
        SpriteRenderer object_sprenderer = Object1.GetComponent<SpriteRenderer>();
        object_sprenderer.sortingOrder = max + 1;

        anim.SetTrigger(anim_name);

        //Positionierung des Wurfobjekts
        Object1.transform.SetParent(agent.transform);
        Object1.transform.localPosition = new Vector3(x_adapt, y_adapt, 0);

        //Rotation des Wurfobjekts
        /*
        if (rotation)
        {
            Object1.transform.Rotate(0, 0, calcRotation(Vector2.right, new Vector2(x_start, y_start), new Vector2(x_target, y_target)));
        }
        */


        /**
            Sorting Layer Order Nummer des Wurfobjekts muss bestimmt werden!

            Wenn nach hinten geworfen wird, sieht man das Gadget trotzdem!!! - Discuss
        **/


        //Start-Koordinaten des Wurfobjekts
        x_start = Object1.transform.position.x;
        y_start = Object1.transform.position.y;

        Object1.transform.Translate(new Vector3(x_target - x_start + 0.5f, y_target - y_start + 0.5f, 0) * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(2);
        //Ziel-Koordinaten des Wurfobjekts, muss noch angepasst werden vlt ?! -> 0.5f, da Objekte "in der Mitte" der Felder stehen TODO
        /*
        Throwing.x_target = x_target;
        Throwing.y_target = y_target;
        */
        //Das Wurfobjekt wird tatsächlich geworfen und anschließend zerstört!
        //yield return StartCoroutine(throwObject1(this.duration, this.startTime, Object1, x_start, y_start, x_target, y_target));
        //Object = Object1;
        //yield return throwObject();
        Destroy(Object1);
        anim.ResetTrigger(anim_name);
    }



    /**
        Diese Methode ist für die Trinkanimation zuständig!
        @param agent: Agent GameObjekt
    **/
    public static IEnumerator Drink_Animation(GameObject agent, bool poisoned){
        Animator animator = agent.GetComponent<Animator>();
        /**
            Dadurch, dass der Cocktail dadurch aufgebraucht wird, müssen alle zustandbasierten Parameter in Bezug auf den Cocktail Zustandsautomaten auf default gesetzt werden
        **/
        string anim = poisoned ? "drink_psn_cocktail" : "drink_cocktail";
        animator.SetTrigger(anim);
        yield return new WaitForSeconds(1);
        animator.ResetTrigger(anim);
    }

    /** 
    Diese Methode ist zur Animierung des Wurfes des Klingen-Huts zuständig! -Animationsart: Wurfanimation 

    @param x_pos,y_pos: X und Y Koordinaten des Agenten
    @param x_target,y_target: X und Y Koordinaten des Wurfziels
    @param hat_newpos_x/y: X und Y Koordinaten des Hutes nach der Wurfanimation
**/
    public static IEnumerator Hat_Animation(float x_target, float y_target, GameObject agent)
    {
        yield return Throw_Animation(x_target, y_target, agent, "Prefabs/Hat", false);
    }

    /**
    Diese Methode ist für die Föhnanimation zuständig
    @param x_target: x-Position des zu Föhnenden
    @param y_target: y-Position des zu Föhnenden
    @param removeClammyClothes: soll die klamme Klamotten Eigenschaft des zu föhnenden entfernt werden (true/false)
    @param (optional) agent_target: Der Agent, dessen klamme Klamotten Eigenschaft entfernt werden soll, muss angegeben werden  , falls removeClammyClothesProperty = true

    Falls der Agent sich selber föhnt, so ist agent_target er selber.
**/
    public static IEnumerator Hairdryer_Animation(GameObject agent, float x_target, float y_target)
    {
        //Name der Animation
        string animation = "";

        /**
            Koordinaten des Agenten
        **/
        float x_pos, y_pos;
        x_pos = agent.transform.position.x;
        y_pos = agent.transform.position.y;

        /**
            Animator des Agenten
        **/
        Animator anim = agent.GetComponent<Animator>();

        //Aktion nach oben
        if (x_pos == x_target && y_pos < y_target)
        {
            animation = "Hairdryer_Above";
        }
        //Aktion nach unten
        else if (x_pos == x_target && y_pos > y_target)
        {
            animation = "Hairdryer_Below";
        }
        //Aktion nach Rechts
        else if (x_pos < x_target && y_pos == y_target)
        {
            animation = "Hairdryer_Right";
        }
        //Aktion nach Links
        else if (x_pos > x_target && y_pos == y_target)
        {
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            animation = "Hairdryer_Right";
        }
        //Aktion nach unten rechts (diagonal)
        else if (x_pos < x_target && y_pos > y_target)
        {
            //Debug.Log("Rechts unten");
            animation = "Hairdryer_RightBelow";
        }
        //Aktion nach unten links (diagonal)
        else if (x_pos > x_target && y_pos > y_target)
        {
            //Debug.Log("Links unten");
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            animation = "Hairdryer_RightBelow";
        }
        //Aktion nach oben links (diagonal)
        else if (x_pos > x_target && y_pos < y_target)
        {
            //Debug.Log("Links oben");
            animation = "Hairdryer_LeftAbove";
        }
        //Aktion nach oben rechts (diagonal)
        else if (x_pos < x_target && y_pos < y_target)
        {
            //Debug.Log("Rechts oben");
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            animation = "Hairdryer_LeftAbove";
        }
        //Aktion gegen sich selber (Föhn)
        else if (x_pos == x_target && y_pos == y_target)
        {
            animation = "Hairdryer_Self";
        }

        if (!animation.Equals(""))
        {
            anim.SetTrigger(animation);
            yield return new WaitForSeconds(1);
            anim.ResetTrigger(animation);
            agent.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }



    /**
        Nachdem der Cocktail verbraucht wurde, müssen alle Attribute in Bezug auf den Cocktail Zustandsautomaten auf false (default) gesetzt werden
    **/
    private static void setDefaultValuesInCocktailStateMachine(GameObject agent){
        Animator animator = agent.GetComponent<Animator>();
        animator.SetBool("Drinking", false);
        animator.SetBool("Cocktail_Lasered", false);
        animator.SetBool("Poisoned", false);
    }


    /**
        Diese Methode sorgt dafür, dass der Agent im "Sitzen" Zustand ist
        @param agent: Agent GameObject
    **/
    public static void Agent_SitState(GameObject agent){
        Animator animator = agent.GetComponent<Animator>();
        animator.SetBool("Sit", true);
    }


    /**
        Diese Methode sorgt dafür, dass der Agent im "Stehen" Zustand ist
        @param agent: Agent GameObject
    **/
    public static void Agent_StandState(GameObject agent){
        Animator animator = agent.GetComponent<Animator>();
        animator.SetBool("Sit", false);
    }



    /**
        Diese Methode ist dafür zuständig, ein gegebenes Objekt auf dem Spielfeld zu positionieren und die Referenzen der TileMaps dementsprechend anzupassen
        @param newobject: neues Objekt auf dem Spielfeld (z.b: der Hut)
        @param object_pos_x/y : x und y Koordinaten der Position des Objekts

        TODO -> Verweisänderung der Tilemap: Paramterliste erweitern, Logik zur Referenzierung des neuen Objekts auf dem Spielfelds
    **/
    public static void NewObjectOnField(ref GameObject newobject, float object_pos_x, float object_pos_y){
        newobject.transform.position = new Vector3(object_pos_x, object_pos_y, 0);
        //TODO: Verweis des GameObject des Tiles in der TileMap auf dieses Objekt
    }



    /* @deprecaded
    /**
        Diese Methode ist für die Animation zuständig, in dem Fall: Animationen mit Wurfobjekten
        Diese Methode greift nur auf lokale Attribute zurück !!!!!!! --- bevorzugt
    
    private IEnumerator throwObject1(float duration1, float startTime1, GameObject Object1, float x_start1, float x_target1, float y_start1, float y_target1){
        do{
            float t = (Time.time - startTime1) / duration1;
            Object1.transform.position = new Vector3(Mathf.SmoothStep(x_start1, x_target1, t), Mathf.SmoothStep(y_start1, y_target1, t), 0);
            yield return null;
        }
        //Wenn das Objekt das Ziel erreicht, dann wird das Wurf Objekt zerstört
        while(Object1 != null && Object1.transform.position != new Vector3(x_target1, y_target1, 0));
        Destroy(Object1);
        //Animation zu Ende
        showTime = false;
    }

    */

    private static IEnumerator throwObject(){
        do{
            float t = (Time.time - startTime) / duration;
            Object.transform.position = new Vector3(Mathf.SmoothStep(x_start, x_target, t), Mathf.SmoothStep(y_start, y_target, t), 0);
            yield return null;
        }
        //Wenn das Objekt das Ziel erreicht, dann wird das Wurf Objekt zerstört
        while(Object != null && Object.transform.position != new Vector3(x_target, y_target, 0));
        Destroy(Object);
    }


    /**
        Diese Methode entfernt einem Charakter die klamme Klamotten Eigenschaft
        - wenn ein Agent geföhnt wird oder
        - ein Agent am Anfang der Runde vor einem Kamin steht

        @param agent: Agent GameObject
        @param change: true, falls der Agent die klamme Klamotten Eigenschaft anzeigen soll; false, falls diese entfernt werden soll
    **/
    public static void changeClammyClothesProperty(GameObject agent, bool change){
        /**
            Das GameObject agent muss ein Agent Objekt darstellen!!
        **/
        GameObject clammy_cloths_property = agent.transform.GetChild(0).gameObject;
        clammy_cloths_property.SetActive(change);

    }

    /**
        Diese Methode blendet den Ohrstöpsel Icon des Agenten ein oder aus-
        Die Wanze wird dem "Opfer" "untergeschossen"(gemäß Latenheft) (-> keine Animation),
        das Einblenden der Ohrstöpsel wird animiert duration lang

        @param agent: Agent, der die Wanze Aktion angewandt hat
        @param show: ein- oder ausblenden (true/false)

        Zeit zur Animierung ???? TODO Discuss.
    **/
    public static IEnumerator Earplugs_Animation(GameObject agent, bool show){
        /**
            Das GameObject agent muss ein Agent Objekt darstellen!!
            In diesem Fall: Der Agent, der die Wanze einsetzt!
        **/
        GameObject earplugs = agent.transform.GetChild(1).gameObject;

        if(show){
            yield return new WaitForSeconds(duration);
            earplugs.SetActive(show);
        }
        else {
            //show ist false, die Ohrstöpsel verschwinden, dazu braucht es keine Animation, dies geschieht am Anfang der Runde
            earplugs.SetActive(show);
        }
        
    }

    /**
        Durch diese Methode wird ein Cocktail auf dem Tisch platziert
    **/
    public static void CocktailOnTable(GameObject table){
        SpriteRenderer spriteR = table.GetComponent<SpriteRenderer>();
        spriteR.sprite = table_sprites[1];
    }


    /**
        Durch diese Methode wird der Cocktail auf dem Tisch vergiftet
    **/
    public static void poisonCocktailOnTable(GameObject table){
        SpriteRenderer spriteR = table.GetComponent<SpriteRenderer>();
        spriteR.sprite = table_sprites[2];
    }

    /**
        Durch diese Methode wird der Cocktail in der Hand 
        @param agent_target: der betroffene Agent, dessen Cocktail vergiftet wird
    **/
    public static void poisonCocktailOfAgent(GameObject agent_target){
        Animator animator = agent_target.GetComponent<Animator>();
        animator.SetBool("Poisoned", true);
    }


    /**
        Durch diese Methode wird der Cocktail in der Hand 
        @param agent_target: der betroffene Agent, dessen Cocktail vergiftet wird
    **/
    public static IEnumerator laserCocktailOfAgent(GameObject agent_target){
        Animator animator = agent_target.GetComponent<Animator>();
        animator.SetBool("Cocktail_Lasered", true);
        //Es wird zur Sicherheit eine Sekunde gewartet bis alle Parameter des Cocktail Zustandsmodells des Agenten auf die default Werte gesetzt werden
        yield return null;

        //setDefaultValuesInCocktailStateMachine(agent_target);
    }

    /**
        Durch diese Methode wird der Cocktail auf dem Tisch entfernt
    **/
    public static void removeCocktailOnTable(GameObject table){
        SpriteRenderer spriteR = table.GetComponent<SpriteRenderer>();
        spriteR.sprite = table_sprites[0];
    }

    /** 
        Animation des Spiegels -Animationsart: fest in der Hand mit Animator Eigenschaft

        @param x_target, y_target: Zielposition (Koordinaten der Zielperson)
        @param mirror_breaking: Wird der Spiegel zerspringen (= true) oder nicht -> Aktion erfolgreich oder nicht 
    **/
    public static IEnumerator Mirror_Animation(float x_target, float y_target, GameObject agent, bool mirror_breaking){
        
        //Objekt in der Hand des Agenten
        GameObject Mirror = Instantiate(Resources.Load("Prefabs/Mirror", typeof(GameObject))) as GameObject;
        
        /**
            Das Objekt soll nach der Demonstration nicht gezeigt werden
        **/
        //duration = duration -1;
        yield return Holding_InHand(x_target,y_target, Mirror, agent,false, false);
        //duration = duration +1;

        
        //Falls der Spiegel zerspringt, so muss dies auch noch animiert werden
        if(mirror_breaking){
            Animator animator = Mirror.GetComponent<Animator>();
            //Dauer 1s
            animator.Play("Mirror_Breaking");
        }
            
        yield return new WaitForSeconds(1);
        //Der Spiegel wird entfernt
        Destroy(Mirror);
        
    } //TODO

    /**
        Diese Methode ist für die Animation des Nuggets zuständig - Animationsart: Wurfanimation (entspricht der Übergabe)
        @param x_target, y_target: Zielposition (Koordinaten der Zielperson)
        @param successful: War die Aktion des Nuggets erfolgreich (= true) oder nicht

        //TODO: Wechsel eines NPCs zur eigenen Fraktion
    **/
    public static IEnumerator Nugget_Animation(float x_target, float y_target, GameObject agent){
        //Übergabe
        yield return Throw_Animation(x_target,y_target,agent, "Prefabs/Nugget", false);
    }

    /**
        Animation zur Übertragung des Chicken Feeds - Animationsart: Wurf (-> entspricht der Übergabe)
    **/
    public static IEnumerator ChickenFeed_Animation(float x_target, float y_target, GameObject agent){
        //"Wurf" -> Übergabe
        yield return Throw_Animation(x_target,y_target,agent,"Prefabs/ChickenFeed", false);

        /**
            Austausch von IPs wird nicht angezeigt!!
            Animation endet mit der Zerstörung des Objektes
        **/ 
    }


    /** 
        Diese Methode ist zur Animierung der Installation des Prismas zuständig - Animationsart: Wurf 

        @param x_target,y_target: X und Y Koordinaten des Wurfziels
    **/
    public static IEnumerator Prism_Animation(float x_target, float y_target, GameObject agent){
        yield return Throw_Animation(x_target, y_target, agent, "Prefabs/Prism", false);

        /**
            Durch die Zerstörung des Prismas wird die Installation angedeutet, bzw. die Negierung auf dem Roullete Tisch wird nicht animiert
        **/

    }

    

    /**
        Diese Methode ist für die Wurfanimation des Maulwürfels zuständig - Animationsart: Wurf
        @param x_target,y_target: X und Y Koordinaten des Wurfziels
    **/
    public static IEnumerator Moledie_Animation(float x_target, float y_target, GameObject agent){
        yield return Throw_Animation(x_target, y_target, agent, "Prefabs/Moledie", false);

        /**
            Der Abprall und das Hüpfen ins Inventars eines anderen Spielers wird nicht animiert - Discuss.
            Durch die Zerstörung des Maulwürfels wird die Animation beendet!
        **/

    }






    /**
        Diese Methode ist für die Wurfanimation einer Giftpille zuständig - Animationsart: Wurf, aber linear (keine Parabel!)
        @param x_target,y_target: X und Y Koordinaten des Wurfziels
        @param agent, table (optional): Agent Objekt oder Tisch Objekt

        Je nachdem wo sich der Cocktail befindet (in der Hand des Agenten oder auf dem Tisch) muss das jeweilige Objekt mitgegeben werden
        Es kann also nur höchstens eines != null sein.

        SEHR WICHTIG:
        Es muss die Sichteinschränkung beachtet werden:
        Nur die Fraktion des Vergifters darf sehen, dass der Cocktail vergiftet wurde!!!

        Beim Aufruf dieser Methode muss also für die Fraktion des Opfers agent == null und table == null sein, so dass diese Fraktion die Vergiftung nicht sieht!!!
    **/
    public static IEnumerator PoisonPills_Animation(float x_target, float y_target,GameObject agent){
        GameObject Poison = Instantiate(Resources.Load("Prefabs/Poison", typeof(GameObject))) as GameObject;
        yield return Holding_InHand(x_target,y_target, Poison, agent, false, true);

        /**
            Nach dem Wurf der Giftpille muss der Cocktail in der Hand des Agenten oder der Cocktail auf dem Tisch sich vergiften
        **/

        /**
            Die Sichtbarkeitseinschränkung !!! TODO
        **/
        /*
        if(agent != null) poisonCocktailOfAgent(agent_target);
        if(table != null) poisonCocktailOnTable(table);
        */
    }


    /**
        Diese Methode berechnet den Winkel, in dem das Wurfobjekt geneigt sein soll im 2 dimensionalen Raum
    **/
    private static float calcRotation(Vector2 object_direction, Vector2 start, Vector2 target ){
        float alpha= Vector2.Angle(object_direction, (target - start));
        return alpha;
    }





    /**
        Dieses Array enthält alle Fog GameObjects, die dann alle nach Ende der Runde zerstört werden können
    **/
    public static GameObject[] fog;

    /**
        Diese Methode erzeugt einen Nebel an der genannten Stelle und allen Nachbarfeldern
        @param x_target,y_target: X und Y Koordinaten "Mitte"-Nebels
        @param orderNumber: Sorting Order - Nummer ALLER Nebelobjekte 
    **/
    public static void createFog(float x_target, float y_target, int orderNumber){
        /**
            Dadurch, dass die Nebeldose auf ein Nicht-Wand geworfen wird, muss hier nicht auf Out-off-Level Fälle geachtet werden
        **/

        fog = new GameObject[9];

        /**
            Instanziierung
        **/
        int index = 0;

        for(int i = -1; i <= 1; i++){
            for(int j = -1; j <= 1; j++){
                //Nebel Objekt
                GameObject Fog_Object = Instantiate(Resources.Load("Prefabs/Fog", typeof(GameObject))) as GameObject;
                //Positionierung des Nebelobjekts
                Fog_Object.transform.position = new Vector3(x_target + i, y_target +j, 0);
                //Anpassung der Sprite Order Nummer
                SpriteRenderer sp = Fog_Object.GetComponent<SpriteRenderer>();
                sp.sortingOrder = orderNumber;
                fog[index] = Fog_Object;
                index++;
            }
        }
    }

    /**
        Diese Methode zerstört alle Fog Objekte auf dem Spielfeld
    **/
    public static void destroyFog(){
        for(int i = 0; i < fog.Length; i++){
            Destroy(fog[i]);
        }
    }

    /**
        Diese Methode ist für die Wurfanimation einer Nebeldose zuständig - Animationsart: Wurf
        @param x_target,y_target: X und Y Koordinaten des Wurfziels

    **/
    public static IEnumerator FogTin_Animation(float x_target, float y_target, GameObject agent){

        //Nachdem die Nebeldose zerstört wurde ... 
        yield return Throw_Animation(x_target,y_target,agent,"Prefabs/FogTin",false);

        /**
            Nach dem Wurf der Nebeldose muss in der Umgebung ein Nebel entstehen: Auf dem Zielfeld und den Nachbarfeldern (Lastenheft),
            es wird die Sorting Nummer aller Nebeleinheiten
        **/
        //createFog(x_target, y_target, (int) y_target + 3);

    }

    /**
       Diese Methode ist für die Wurfanimation einer Mottenkugel zuständig - Animationsart: Wurf
        @param x_target,y_target: X und Y Koordinaten des Wurfziels
        @param fireplace: Kamin
    **/
    public static IEnumerator Mothball_Animation(float x_target, float y_target, GameObject agent){

        //Nachdem die Mottenkugel in den Kamin gelangt.
        //Die Dauer des Wurfs ist eine Sekunde kürzer wegen der Kaminanimation
        //duration = duration -1;
        yield return Throw_Animation(x_target,y_target,agent,"Prefabs/Mothball",false);
        //duration = duration +1;

        /**
            Nachdem die Mottenkugel den Kamin erreicht, muss eine Explosion im Kamin entstehen
        **/
        GameObject exploder = Instantiate(Resources.Load("Prefabs/Explosion")) as GameObject;
        exploder.transform.position = new Vector3(x_target, y_target, 0);
        exploder.GetComponent<SpriteRenderer>().sortingOrder = (int) (-y_target) + 5;
        Animator animator = exploder.GetComponent<Animator>();
        animator.SetTrigger("Explosion");
        //Animationsclip dauert eine Sekunde
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("Explosion");
        Destroy(exploder);
    }



    /**
       Diese Methode ist für die Wurfanimation des Lippenstifts zuständig - Animationsart: Fest in der Hand (ohne Wurf) mit fester Rotation
        @param x_target,y_target: X und Y Koordinaten des Wurfziels
    **/
    public static IEnumerator GasGloss_Animation(float x_target, float y_target, GameObject agent){
        yield return Holding_InHand(x_target,y_target, Instantiate(Resources.Load("Prefabs/Gloss", typeof(GameObject))) as GameObject, agent, true, true);
    }


    /**
       Diese Methode ist für die Wurfanimation der Rakete zuständig - Animationsart: Fest in der Hand (mit Wurf) mit variabler Rotation
        @param x_target,y_target: X und Y Koordinaten des Wurfziels
        @param wall: GameObjekt
    **/
    public static IEnumerator Rocket_Animation(float x_target, float y_target, GameObject agent){
        yield return Throw_Animation(x_target,y_target,agent, "Prefabs/Rocket", true);

        // explosion afterwards
        GameObject exploder = Instantiate(Resources.Load("Prefabs/Explosion")) as GameObject;
        exploder.transform.position = new Vector3(x_target, y_target, 0);
        exploder.GetComponent<SpriteRenderer>().sortingOrder = (int)(-y_target) + 5;
        Animator animator = exploder.GetComponent<Animator>();
        animator.SetTrigger("Explosion");
        //Animationsclip dauert eine Sekunde
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("Explosion");
        Destroy(exploder);
    }


    /**
       Diese Methode ist für die Animation des Laser mit dem Laserschuss zuständig - Animationsart: Fest in der Hand (mit Wurf eines Kindobjekts) mit variabler Rotation
        @param x_target,y_target: X und Y Koordinaten des Wurfziels
        @param agent: Agent, der das Cocktail in der Hand hält
        @param table: Tisch, auf dem der Cocktail steht
        @param successful: Gelingt die Laser Aktion oder nicht ???

        Für das anschließende Verdampfen des Cocktails muss genau agent oder table != null sein
    **/
    public static IEnumerator Laser_Animation(float x_target, float y_target, GameObject agent){
        //Das Laser-Objekt wird in der Hand gehalten und der Agent schaut in die richtige Schießrichtung
        GameObject Laser = Instantiate(Resources.Load("Prefabs/Laser", typeof(GameObject))) as GameObject;
        Holding_InHand(x_target,y_target, Laser, agent, true, true);

        /**
            Koordinaten des Agenten
        **/
        float x_pos,y_pos;
        x_pos = (int) agent.transform.position.x;
        y_pos = (int) agent.transform.position.y;

        /**
            Sorting Layer Order Nummer des Wurfobjekts muss bestimmt werden!
        **/

        /**
            Dabei handelt es sich hierbei um den Laser Schuss als "Wurfobjekt"
        **/
        GameObject LaserBullet = Laser.transform.GetChild(0).gameObject;
        int max = Math.Max((int) y_pos, (int) y_target);
        SpriteRenderer object_sprenderer = LaserBullet.GetComponent<SpriteRenderer>();
        object_sprenderer.sortingOrder = max + 1;

        //Start-Koordinaten des Wurfobjekts
        /**
            Das Schussobjekt ist fest im Prefab integriert.
        **/

        //Start-Koordinaten des Wurfobjekts
        x_start = LaserBullet.transform.position.x;
        y_start = LaserBullet.transform.position.y;

        LaserBullet.transform.Translate(new Vector3(x_target - x_start, y_target - y_start, 0) * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(1);
        Destroy(LaserBullet);


        //Ziel-Koordinaten des Wurfobjekts, muss noch angepasst werden vlt ?! -> 0.5f, da Objekte "in der Mitte" der Felder stehen
        //Throwing.x_target = x_target;
        //Throwing.y_target = y_target;

        //"Wurfobjekt"
        //Object = LaserBullet;



        /**
            Der Laser Schuss wird abgefeuert!
        **/
        //yield return throwObject();

        /**
            Nachdem der Laser Schuss das Ziel erreicht, verdampft der Cocktail in der Hand des Agenten oder auf dem Tisch
        **/
        //if(!successful) yield break;

/*        
        //Verdampfung des Cocktails
        if(agent_target != null) laserCocktailOfAgent(agent_target);
        if(table != null) removeCocktailOnTable(table);
*/
    }


    /**
       Diese Methode ist für die Wurfanimation des Wurfhakens zuständig - Animationsart: Wurf mit variabler Rotation, kein länger werdender Griff !!!
        @param x_target,y_target: X und Y Koordinaten des Wurfziels
        @param successfull: Ist die Aktion erfolgreich
        @param gadget_OnGround: Gadget, das auf dem Boden liegt
        @param table: Cocktail auf dem Tisch wird aufgenommen
        
    **/
    public static IEnumerator Grapple_Animation(float x_target, float y_target, GameObject agent){ //,bool successful, GameObject gadget_OnGround = null, GameObject table = null){

        /**
            Die Wurfhaken wird mit der richtigen Rotation geworfen
        **/
        yield return Throw_Animation(x_target,y_target,agent,"Prefabs/Grapple",true);

        /**
            Nachdem der Wurfhaken abgeworfen wurde, soll der Agent das Gadget aufnehmen, wenn die Aktion erfolgreich war
        **/
        //if(!successful) yield break;
        
        /**
            Objekt im Inventar, das Objekt verschwindet aus dem Spielfeld
        **/
        //if(gadget_OnGround != null) Destroy(gadget_OnGround);

        /**
            Auf dem Tisch verschwindet das Cocktail, und gelangt in die Hand des Agenten

            Der Tisch hat 2 Cocktail Zustände: Tisch mit unvergiftetem Cocktail, Tisch mit vergiftetem Cocktail 
            Dadurch, dass beim Vergiten des Cocktails der Tisch Zustand in der View ändert und dieser konsistent bleibt zur Logik ist, wird nach dessen sprite abgefragt

            Da die Aufnahme nicht animiert werden soll, ist animation = false. Diese Methode wird somit als Hilfsmethode benutzt.
        **/
    }

    /**
        @param player: Agent, der Roulette spielst 
        @param x_tagret,y_target: x,y Ziel 
        @param RouletteTable: Roulette Tisch
        @param newChipsAmount: Neue Anzahl an Spielchips
    **/
    public static IEnumerator PlayRoulette_Animation(GameObject player,float x_target, float y_target){
        //duration = duration -1;

        //Objekt in der Hand des Agenten -könnten auch Spielchips sein
        GameObject Empty = Instantiate(Resources.Load("Prefabs/Empty", typeof(GameObject))) as GameObject;
        yield return Holding_InHand(x_target,y_target,Empty, player,false,true);

        //duration = duration +1;

    }

    /**
        Diese Methode implementiert die Bang And Burn Aktion, wodurch der Roulette Tisch unbrauchbar gemacht wird
    **/
    public static IEnumerator BangAndBurn_Animation(GameObject player, float x_target, float y_target){
        //Objekt in der Hand des Agenten - könnte auch etwas zum Zerstören sein
        GameObject Empty = Instantiate(Resources.Load("Prefabs/Empty", typeof(GameObject))) as GameObject;
        yield return Holding_InHand(x_target,y_target,Empty, player,false,true);

        //Roulette-Tisch wird unbrauchbar gemacht
        //changeLabelOfRouletteTable(RouletteTable, "defect");
    }



    /**
        Diese Methode ändert das Label (Beschriftung) auf dem Roulette Tisch
        @param RouletteTable: Roulette Tisch
        @param label: Label, das auf dem Roulette Tisch geprintet wird
    **/
    private static void changeLabelOfRouletteTable(GameObject RouletteTable, string label){
        //Das zweite Kind ist die Canvas
        GameObject Canvas = RouletteTable.transform.GetChild(1).gameObject;

        //Dessen erstes Kind ist der Text
        GameObject Text_GameObject = Canvas.transform.GetChild(0).gameObject;
        Text text = Text_GameObject.GetComponent<Text>();
        text.text = label;
    }


    /**
        Diese Methode ist zur Animierung der Tresor Öffnung zuständig, welches dem "in den Tresor spicken" entspricht

        - Dabei schaut der Agent nicht in die Richtung des Tresors, damit dieser nicht erkannt wird
    **/
    public static IEnumerator OpenTresor_Animation(GameObject Safe){
        /**
            TODO
            Sichtbarkeitseinschränkung:

            Nur die eigene Fraktion darf sehen, wie der Tresor geöffnet wird
        **/

        Animator animator = Safe.GetComponent<Animator>();
        animator.Play("SafeOpening");

        //Warte duration lang, auf die vorangehende Animation wird nicht gewartet
        yield return new WaitForSeconds(0.75f);
    }

    /**
        Diese Methode ist zur Animierung des "Verschütten"s eines Cocktails zuständig - Animationsart: Wurf des Cocktails
        @param agent: Agent, der die Aktion ausführt
        @param Zielkoordinaten
        @param successful: Aktion erfolgreich -> Ziel_Agent bekommt die klamme Klamotten Eigenschaft 
    **/
    public static IEnumerator SpillCocktail_Animation(GameObject agent, float x_target, float y_target, bool cocktail_poisoned){
        //Dieser Boolean Flag frägt ab, ob der Cocktail vergiftet ist oder nicht
        string gadget_Path = cocktail_poisoned ? "Prefabs/Cocktail_Poisoned" : "Prefabs/Cocktail";
        yield return Throw_Animation(x_target,y_target,agent,gadget_Path,false);

        /*
        if(successful){
            //Die Zielperson kommt im Erfolgsfall die klammen Klamotten Eigenschaft
            changeClammyClothesProperty(agent_target, true);
        }
        */

        /**
            Der Cocktail verschwindet dadurch, es müssen die Default Werte im Cocktail Zustandsautomaten wiederhergestellt werden
        **/
        //animator.Play("Stand");
        //setDefaultValuesInCocktailStateMachine(agent);
    }

    /**
        Diese Methode ist für die Animation des Ausspionierens zuständig
        @param agent: Ausspionierer
        @param Zielkoordinaten
    **/
    public static IEnumerator Spy_Animation(GameObject agent, float x_target, float y_target){
        //Frage-Zeichen erscheint in der Hand des Agenten, welches das Ausspionieren symbolisiert
        GameObject QuestionMark = Instantiate(Resources.Load("Prefabs/QuestionMark", typeof(GameObject))) as GameObject;

        /**
            TODO -> Sichtbarkeitseinschränkung
            Eigene Fraktion sieht immer die Animation.
            Gegner sieht nur die Animation, falls dieser ausspioniert wird (Lastenheft:" Das Mitglied der anderen Fraktion bemerkt den Versuch aber in jedem Fall, und der 
            Spion ist als Angehöriger der anderen Fraktion enttarnt. ") -> entspricht der Enttarnung 

            Eventuell muss die Paramterliste erweitert werden!!!

        **/

        //if(eigene Fraktion)
        yield return Holding_InHand(x_target,y_target,QuestionMark, agent,false,true);

        //if(Es wurde versucht, den Gegner auszuspionieren)
        //Gleiche Animation wie oben
        //yield return StartCoroutine(Holding_InHand(x_target,y_target,QuestionMark, agent,false,true));
        //Enttarnung -> Agent_Exposed(agent);
    }

    private static void Agent_Exposed(GameObject agent){
        GameObject Enemy_Label = agent.transform.GetChild(2).gameObject;
        Enemy_Label.SetActive(true);
    }

    /**
        Diese Methode ist für Animation des Observation zuständig
    **/
    public static IEnumerator Observation_Animation(GameObject agent, float x_target, float y_target){
        GameObject Empty = Instantiate(Resources.Load("Prefabs/Empty", typeof(GameObject))) as GameObject;
        yield return Holding_InHand(x_target,y_target,Empty, agent,false,true);

        //if(!successful) yield break;

        /**
            TODO: Sichtbarkeitseinschränkung
            Nur die Fraktion des Agenten, die Observation angewandt hat, sieht im Erfolgsfall die Enttarnung

            Agent_Exposed(agent_target);
        **/

    }


    /**
        Folgende Methoden sind für die Animation des Hausmeister zuständig
    **/

    //Dies ist das GameObject des Hausmeisters
    static GameObject Janitor;

    /**
        Durch diese Methode wird der Hausmeister respawnt.
    **/
    public static void Respawn_Janitor(float x_pos, float y_pos){
        Janitor = Instantiate(Resources.Load("Prefabs/Janitor", typeof(GameObject))) as GameObject;
        Janitor.transform.position = new Vector3(x_pos,y_pos,0);
        SpriteRenderer sp = Janitor.GetComponent<SpriteRenderer>();
        sp.sortingOrder = (int) y_pos;
    }


    /**
        Diese Methode ist für "das Hinauskehren" des Agenten aus dem Casino zuständig.
        Dabei bewegt sich der Hausmeister zum jeweiligen Agenten hin.
    **/
    public static IEnumerator SweepingAgent_Animation(GameObject agent){
        //Der Hausmeister beamt sich zum Agenten hin
        SpriteRenderer sp = Janitor.GetComponent<SpriteRenderer>();
        sp.sortingOrder = (int) agent.transform.position.y + 1;
        Janitor.transform.position = agent.transform.position;

        //Animationszeit
        yield return new WaitForSeconds(duration);

        //Der Agent verschwindet aus der Map
        Destroy(agent);

    }

    /**
        Diese Methode ist für die Animation der Exfiltration zuständig
    **/
    public static IEnumerator Exfiltration(GameObject agent, float x_target, float y_target){
        agent.transform.position = new Vector3(x_target, y_target, 0);
        yield return new WaitForSeconds(duration);
        Animator animator = agent.GetComponent<Animator>();
        animator.SetBool("Sit", true);
        
        /**
            TODO: Verweisänderung
        **/

    }

}
