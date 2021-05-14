using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] protected int _numberOfUnits;
    public int NumberOfUnits => _numberOfUnits;

    [SerializeField] protected string _background;
    public string Background => _background;

    [SerializeField] protected Image _image;
    public Image Image => _image;
}