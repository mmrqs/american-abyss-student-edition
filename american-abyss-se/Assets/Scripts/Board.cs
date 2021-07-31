using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Animations;

public class USZone
{
    private Dictionary<Character, int> characters;
    public Dictionary<Character, int> Characters
    {
        get => characters;
        set => characters = value;
    }
    
    public USZone()
    {
        characters = new Dictionary<Character, int>();
    }

    public USZone(Dictionary<Character, int> init)
    {
        characters = new Dictionary<Character, int>();
        foreach (var entry in init)
        {
            characters.Add(entry.Key, entry.Value);
        }
    }
    
    public USZone Clone()
    {
        return new USZone(characters);
    }
}

public class Board
{
    private Dictionary<Area, USZone> usZone;
    public Dictionary<Area, USZone> UsZone
    {
        get => usZone;
        set => usZone = value;
    }

    public Board()
    {
        usZone = new Dictionary<Area, USZone>
        {
            { Area.MIDWEST, new USZone() },
            { Area.NORTHEAST , new USZone() },
            { Area.NORTHWEST, new USZone() },
            { Area.SOUTHEAST, new USZone() },
            { Area.SOUTHWEST, new USZone() },
            { Area.NORTH_CENTRAL, new USZone() },
            { Area.SOUTH_CENTRAL, new USZone() },
        };
    }

    public Board(Dictionary<Area, USZone> board)
    {
        usZone = new Dictionary<Area, USZone>();
        foreach (var entry in board)
            UsZone.Add(entry.Key, entry.Value.Clone());
    }
}
