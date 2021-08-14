using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopsManager : MonoBehaviour
{
    public List<Battalion> units;
    public TroopsManagerUI troopsManagerUI;
    public List<Zone> zones;
    
    private void Start()
    {
        troopsManagerUI.BuildUI(units);
    }

    public void AddUnitToZone(Character character, Area area, int numberOfUnits = 1)
    {
        if(!units.Exists(t => t.Character.Name == character.Name && t.Area == area))
            units.Add(new Battalion(character, area));
        units
            .First(t => t.Character.Name == character.Name && t.Area == area)
            .NumberOfUnits += numberOfUnits;
        
        troopsManagerUI.BuildUI(units);
    }

    public void MoveUnit(Character character, Area startingArea, Area endArea, int numberOfUnitsToMove)
    {
        units.Find(u => u.Character.Name == character.Name && u.Area == startingArea).NumberOfUnits -= numberOfUnitsToMove;
        
        if(!units.Exists(b => b.Character.Name == character.Name && b.Area == endArea))
            units.Add(new Battalion(character, endArea));
        units.Find(u => u.Character.Name == character.Name && u.Area == endArea).NumberOfUnits += numberOfUnitsToMove;
        troopsManagerUI.BuildUI(units);
    }

    public void RemoveUnit(Character character, Area area)
    {
        Debug.Log(units.Find(u => u.Character == character && u.Area == area).NumberOfUnits);
        if(units.Find(u => u.Character == character && u.Area == area).NumberOfUnits > 0)
            units.Find(u => u.Character == character && u.Area == area).NumberOfUnits -= 1;

        troopsManagerUI.BuildUI(units);
    }

    public List<Character> GetCharactersInZone(Area area)
    {
        return units.FindAll(b => b.Area == area)
            .Select(b => b.Character)
            .ToList();
    }

    public int GetNumberOfUnitsInArea(Area area, Character character)
    {
        return units.Find(b => b.Character.Name == character.Name && b.Area == area).NumberOfUnits;
    }

    public List<Zone> GetZonesWhereCharacterHasUnits(Character character)
    {
        List<Area> areas = units.Where(b => b.Character.Name == character.Name && b.NumberOfUnits > 0)
            .Select(u => u.Area)
            .ToList();
        return GetZones(areas);
    }
    
    public List<Zone> GetZonesWhereCharacterHasCertainAmountOfUnits(Character character, int nb)
    {
        List<Area> areas = units.Where(b => b.Character.Name == character.Name && b.NumberOfUnits > nb)
            .Select(u => u.Area)
            .ToList();
        return GetZones(areas);
    }

    public List<Zone> GetAllZones()
    {
        return zones;
    }

    public Zone GetZone(Area area)
    {
        return zones.First(z => z.Name == area);
    }

    public List<Zone> GetZones(List<Area> areas)
    {
        return zones.Where(t2 => areas.Contains(t2.Name)).ToList();
    }

    public List<Zone> GetSurroundingZonesInPerimeter(int distance, Area startingZone)
    {
        HashSet<Area> result = new HashSet<Area>(GetZone(startingZone).Surroundings);

        for (int i = 0; i < distance - 1; i++)
            foreach (Area area in result.Reverse())
                result.UnionWith(GetZone(area).Surroundings);
        result.Remove(startingZone);
        return GetZones(result.ToList());
    }

    public int GetDistanceBetweenTwoZones(Area area1, Area area2)
    {
        for (var i = 1; i < 3; i++)
        {
            Debug.Log(GetSurroundingZonesInPerimeter(i, area1));
            if (GetSurroundingZonesInPerimeter(i, area1).Contains(GetZone(area2)))
                return i;
        }
        return 0;
    }

    public int GetTotalNumberOfUnitsInField(Character character)
    {
        return units
            .Where(b => b.Character.Name == character.Name)
            .Sum(b => b.NumberOfUnits); 
    }

    public List<Battalion> GetZoneMasters()
    {
        var result = new List<Battalion>();
        foreach (var battalion in units)
        {
            Battalion concurrent = result.Find(b => battalion.Area == b.Area);
            
            if(concurrent == null && battalion.NumberOfUnits > 0)
                result.Add(battalion);
            else if (concurrent != null && concurrent.NumberOfUnits < battalion.NumberOfUnits)
            {
                result.Remove(concurrent);
                result.Add(battalion);
            }
            else if(concurrent != null && concurrent.NumberOfUnits == battalion.NumberOfUnits)
                result.Remove(concurrent);
        }
        return result;
    }

    public Character GetMaster(Zone zone)
    {
        int bestVal = 0;
        Battalion result = null;
        
        foreach (var battalion in units.FindAll(b => b.Area == zone.Name))
        {
            if (result == null && battalion.NumberOfUnits > 0 && battalion.NumberOfUnits > bestVal)
            {
                result = battalion;
                bestVal = battalion.NumberOfUnits;
            }
            else if (result != null && result.NumberOfUnits < battalion.NumberOfUnits &&
                     battalion.NumberOfUnits > bestVal)
            {
                result = battalion;
                bestVal = battalion.NumberOfUnits;
            }
            else if (result != null && result.NumberOfUnits == battalion.NumberOfUnits)
                result = null;
        }
        return result?.Character;
    }
    
    
    public int GetTotalControlledZones(Character character)
    {
        return GetZoneMasters()
            .FindAll(b => b.Character.Name.Equals(character.Name))
            .Count;
    }
}
