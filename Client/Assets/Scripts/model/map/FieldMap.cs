
using System;
using System.Collections.Generic;
/**
Geupdated
**/
public class FieldMap
{
    public Field[,] map { get; set; }

    public Field[,] getMap()
    {
        return map;
    }
    public FieldMap(Field[,] map)
    {
        this.map = map;
    }

    //default constructor, braucht man für JSON Deserialisierung
    public FieldMap() { }


    /**
	[,] muss geändert werden in [][] weil [,] ein rechteckförmiges Arraygebilde voraussetzt 
	**/


    /**
		Folgende Methoden dienen zur Sichtlinien Berechnung und wurden vom Server übernommen, 11.07.2020
	**/


    /*

     * Method to validate if the target field is in sight.
     *
     * @param start  The coordinates of the start field.
     * @param target The coordinates of the target field.
     * 
	 * returns true, if target is in Sight, else false
     */
    public bool validateIsInSight(Point start, Point target)
    {
        if (target.EqualsPoint(start))
        {
            return false;
        }
        if (!isInSight(start, target))
        {
            return false;
        }

        return true;
    }

    /**
     * Method to check if a target is in sight of the character.
     *
     * @param start  The coordinates of the start field.
     * @param target The coordinates of the target field.
     * @return true if the target field is in sight, false if not.
     */
    public bool isInSight(Point start, Point target)
    {
        foreach (Point point in getPointsInBetween(start, target))
        {
            if (getField(point).isState(FieldStateEnum.WALL) || getField(point).isState(FieldStateEnum.FIREPLACE) || getField(point).getIsFoggy())
            {
                return false;
            }
        }
        return true;
    }


    /**
     * Method to get the coordinates of the fields in between two points.
     *
     * @param start  The coordinates of the start field.
     * @param target The coordinates of the target field.
     * @return The coordinates of the fields in between the given points.
     */
    public HashSet<Point> getPointsInBetween(Point start, Point target)
    {
        HashSet<Point> points = new HashSet<Point>();

        // The coordinates of the start field and target field
        int x = start.x;
        int y = start.y;
        int xTarget = target.x;
        int yTarget = target.y;

        // The x and y ranges to the target field
        int xRange = Math.Abs(xTarget - x);
        int yRange = Math.Abs(yTarget - y);

        // The number of fields to check
        int n = xRange + yRange - 1;

        // The x and y direction from the start to the target field
        int xDirection = xTarget > x ? 1 : -1;
        int yDirection = yTarget > y ? 1 : -1;

        // The offset to the target point
        int offset = xRange - yRange;

        // For intermediate points which are not in the middle of the grid, it suffices to multiply by 2
        xRange *= 2;
        yRange *= 2;

        while (n > 0)
        {
            // Go in x direction
            if (offset > 0)
            {
                x += xDirection;
                offset -= yRange;
            }

            // Go in y direction
            else if (offset < 0)
            {
                y += yDirection;
                offset += xRange;
            }

            // Go diagonal, get both fields which are tangent to the line//vermuteter Fehler
            else
            {
                // points.Add(new Point(x + xDirection, y));//nachträglich entfernt da die implentierung des gekauften servers falsch war
                // points.Add(new Point(x, y + yDirection));////nachträglich entfernt da die implentierung des gekauften servers falsch war
                x += xDirection;
                y += yDirection;
                offset += xRange - yRange;
                n--;
            }
            if (n > 0)
            {
                points.Add(new Point(x, y));
            }
            n--;
        }
        return points;
    }


    /**
     * Method to get the field of the given coordinates.
     *
     * @param point The coordinates of the field.
     * @return The field of the given coordinates.
     */
    public Field getField(Point point)
    {
        return map[point.y, point.x];
    }


    /**
     * Method to validate if the target field is blocked.
     *
     * @param start      The coordinates of the start field.
     * @param target     The coordinates of the target field.
     * @param characters All the characters currently active in the game.
     * @throws TargetBlockedException If the target field is blocked.
     * @throws InvalidTargetException If the target is the character itself.
     */
    public bool validateIsNotBlocked(Point start, Point target, HashSet<Character> characters)
    {
        if (target.EqualsPoint(start))
        {
            return false;
        }
        if (isBlocked(start, target, characters))
        {
            return false;
        }

        return true;
    }

    /**
     * Method to check if a target is blocked by a character.
     *
     * @param start      The coordinates of the start field.
     * @param target     The coordinates of the target field.
     * @param characters All the characters currently active in the game.
     * @return true if the target field is blocked, false if not.
     */
    public Boolean isBlocked(Point start, Point target, HashSet<Character> characters)
    {
        foreach (Point point in getPointsInBetween(start, target))
        {
            if (fieldHasCharacter(point, characters))
            {
                return true;
            }
        }
        return false;
    }

    /**
     * Method to check if a character is on the given field.
     *
     * @param point      The coordinates of the field.
     * @param characters The characters currently active in the game.
     * @return true if there is a character on the given field, false if not.
     */
    public bool fieldHasCharacter(Point point, HashSet<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character.getCoordinates().EqualsPoint(point))
                return true;
        }
        return false;
    }

    public override string ToString()
    {
        string s = "";
        for(int row = 0; row < map.GetLength(0); row++)
        {
            for(int col = 0; col < map.GetLength(1); col++)
            {
                s += map[row, col].getState() + " ";
            }
            s += "\n";
        }
        return s;
    }
}
