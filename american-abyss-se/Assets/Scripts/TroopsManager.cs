using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopsManager : MonoBehaviour
{
    public List<Battalion> units;
    public TroopsManagerUI troopsManagerUI;

    private void Start()
    {
        troopsManagerUI.BuildUI(units);
    }

    public void AddUnitToZone(Character character, Area area)
    {
        if(!units.Exists(t => t.Character.Name == character.Name && t.Area == area))
            units.Add(new Battalion(character, area));
        units
            .First(t => t.Character.Name == character.Name && t.Area == area)
            .NumberOfUnits += 1;
        
        troopsManagerUI.BuildUI(units);
    }

    public void MoveUnit(Character character, Area startingArea, Area endArea)
    {
        units.Find(u => u.Character == character && u.Area == startingArea).NumberOfUnits -= 1;
        units.Find(u => u.Character == character && u.Area == endArea).NumberOfUnits += 1;
        
        troopsManagerUI.BuildUI(units);
    }

    public void RemoveUnit(Character character, Area area)
    {
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
}
