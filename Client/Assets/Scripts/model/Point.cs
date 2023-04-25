using System;
using System.Security.Cryptography;
using UnityEngine;

public class Point {
    public int x { get; set; }
    public int y { get; set; }

    public Point(int x, int y){
        this.x = x;
        this.y = y;
    }

    //default constructor, braucht man für JSON Deserialisierung
    public Point() { }

    public static Vector3Int point_to_vector3int(Point p)
    {
        if(p == null)
        {
            return Vector3Int.zero;
        }
        return new Vector3Int(p.x, p.y, 0);
    }

    /**
     * + Operator damit man mit Point a + Point b rechnen kann
     * (quality of life)
     */
    public static Point operator +(Point p1, Point p2) => new Point(p1.x + p2.x, p1.y + p2.y);

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }

    /**
     * Gibt die Grössere der beiden Koordinaten an
     */
    public int max()
    {
        return Math.Max(x, y);
    }

    /**
     * Gibt zurück ob das Objekt gleiche Koordinaten hat wie des Parameters
     * Ja, wir haetten einfach equals überschreiben können aber so gehts ja auch
     */
    public bool EqualsPoint(Point p)
    {
        return p != null && x == p.x && y == p.y;
    }

}