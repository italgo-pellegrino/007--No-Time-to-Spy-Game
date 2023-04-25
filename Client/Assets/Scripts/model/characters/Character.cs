//GUID
using System;
//HashSet
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Analytics;

//s. Standard
public class Character {
    // alle Attribute müssen public sein, für JSON Deserialisierung
    public Guid characterId { get; set; }
    public string name { get; set; }
    public Point coordinates { get; set; }
    public int mp { get; set; }
    public int ap { get; set; }
    public int hp { get; set; }
    public int ip { get; set; }
    public int chips { get; set; }

public HashSet<PropertyEnum> properties { get; set; }
public HashSet<Gadget> gadgets { get; set; }

// for char list creation
public Character(Guid characterId, string name, HashSet<PropertyEnum> properties)
    {
        this.characterId = characterId;
        this.name = name;
        this.properties = properties;
    }
    public Character(Guid characterId, string name, Point coordinates, int mp, int ap,
        int hp, int ip, int chips, HashSet<PropertyEnum> properties, HashSet<Gadget> gadgets)
    {
        this.characterId = characterId;
        this.name = name;
        this.coordinates = coordinates;
        this.mp = mp;
        this.ap = ap;
        this.hp = hp;
        this.ip = ip;
        this.chips = chips;
        this.properties = properties;
        this.gadgets = gadgets;
    }

    //default constructor, braucht man für JSON Deserialisierung
    public Character() { }

    /**
     * GETTERS/SETTERS
     */
    public Guid getGuid(){
        return characterId;
    }


    public string getName(){
        return name;
    }

    /** 
        Positionen des Charakters 
    **/
    public Point getCoordinates(){
        return coordinates;
    }

    public void setCoordinates(Point p)
    {
        this.coordinates = p;
    }

    public int getHp(){
        return hp;
    }

    public void setHp(int hp)
    {
        this.hp = hp;
    }

    public int getIp(){
        return ip;
    }

    public void setIp(int ip)
    {
        this.ip = ip;
    }

    public int getMp(){
        return mp;
    }

    public void setMp(int mp)
    {
        this.mp = mp;
    }

    public int getAp(){
        return ap;
    }

    public void setAp(int ap)
    {
        this.ap = ap;
    }

    public int getChips(){
        return chips;
    }
    public void setChips(int chips)
    {
        this.chips = chips;
    }


    public HashSet<PropertyEnum> getProperties(){
        return properties;
    }

    public void setProperties(HashSet<PropertyEnum> properties)
    {
        this.properties = properties;
    }

    /**
        Diese Methode überprüft, ob der Charakter diese angegebene Eigenschaft hat
        @property: Eigenschaft, auf die der Charakter geprüft werden soll
    **/
    public bool hasProperty(PropertyEnum property){
        System.Collections.Generic.HashSet<PropertyEnum>.Enumerator enu = properties.GetEnumerator();
        while(enu.MoveNext()){
            PropertyEnum prop = enu.Current;
            if(prop.Equals(property)) return true;
        }
        return false;
    }


    public HashSet<Gadget> GetGadgets(){
        return gadgets;
    }


    /**
        Es wird abgefragt, ob ein Charakter ein bestimmtes Gadget hat
        @param gadgetEnum: Typ des Gadgets
        
        return Gadget, sonst null
    **/
    public Gadget hasGadget(GadgetEnum gadgetEnum){
        HashSet<Gadget>.Enumerator enu = gadgets.GetEnumerator();
        while(enu.MoveNext()){
            Gadget g = enu.Current;
            if(g.gadget.Equals(gadgetEnum)) return g;
        }
        return null;
    }

    public override string ToString()
    {
        return "ID: " + characterId + ", Name: " + name + ", Coordinates: " + coordinates.ToString();
    }
}
