using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Area
{
    NORTHWEST,
    NORTH_CENTRAL,
    MIDWEST,
    NORTHEAST,
    SOUTHEAST, 
    SOUTH_CENTRAL,
    SOUTHWEST
}

[CreateAssetMenu(menuName = "Character/Character")]
public class Character : ScriptableObject
{
    [SerializeField] protected string _name;
    public string Name => _name;

    [SerializeField] protected GameObject _unit;
    public GameObject Unit => _unit;

    [SerializeField] protected int _numberOfUnits;
    public int NumberOfUnits => _numberOfUnits;

    [SerializeField] protected string _background;
    public string Background => _background;

    [SerializeField] protected List<SetupZone> _setup;
    public List<SetupZone> Setup => _setup;
}