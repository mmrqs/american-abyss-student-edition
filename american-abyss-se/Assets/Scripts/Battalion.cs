using System;
using UnityEngine;

[Serializable]
public class Battalion
{
    [SerializeField] protected Character _character;
    public Character Character
    {
        get => _character;
        set => _character = value;
    }

    [SerializeField] protected Area _area;
    public Area Area
    {
        get => _area;
        set => _area = value;
    }

    [SerializeField] protected int _numberOfUnits;
    public int NumberOfUnits
    {
        get => _numberOfUnits;
        set => _numberOfUnits = value;
    }

    public Battalion()
    {
        
    }

    public Battalion(Character character, Area area)
    {
        Character = character;
        Area = area;
        NumberOfUnits = 0;
    }
}
