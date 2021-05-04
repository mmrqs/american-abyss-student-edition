using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SetupZone
{
    [SerializeField] protected Area _area;
    public Area Area => _area;

    [SerializeField] protected int _nbOfUnits;
    public int NbOfUnits => _nbOfUnits;
}