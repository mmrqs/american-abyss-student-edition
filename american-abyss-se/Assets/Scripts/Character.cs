using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Character/Character")]
public class Character : ScriptableObject, IComparable<Character>
{
    [SerializeField] protected string _name;
    public string Name => _name;

    [SerializeField] protected int _numberOfUnits;
    public int NumberOfUnits => _numberOfUnits;

    [SerializeField] protected string _background;
    public string Background => _background;

    [SerializeField] protected int _movingZoneDistance;
    public int MovingZoneDistance => _movingZoneDistance;
    
    [SerializeField] protected int _numberOfTroopsDestroyed;
    public int NumberOfTroopsDestroyed => _numberOfTroopsDestroyed;
    
    // victory conditions
    [SerializeField] protected int _nbOfTerritoriesToControl;
    public int NbOfTerritoriesToControl => _nbOfTerritoriesToControl;
    
    [SerializeField] protected int _amountOfMoneyToHave;
    public int AmountOfMoneyToHave
    {
        get => _amountOfMoneyToHave;
        set => _amountOfMoneyToHave = value;
    }

    [SerializeField] protected Color32 color;
    public Color Color => color;

    [SerializeField] protected string aim;
    public string Aim => aim;

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(Name)}: {Name}, {nameof(NumberOfUnits)}: {NumberOfUnits}, {nameof(Background)}: {Background}, {nameof(MovingZoneDistance)}: {MovingZoneDistance}, {nameof(NumberOfTroopsDestroyed)}: {NumberOfTroopsDestroyed}, {nameof(NbOfTerritoriesToControl)}: {NbOfTerritoriesToControl}, {nameof(AmountOfMoneyToHave)}: {AmountOfMoneyToHave}, {nameof(Color)}: {Color}, {nameof(Aim)}: {Aim}";
    }

    public int CompareTo(Character obj)
    {
        if (obj == null)
            return 1;
        else
            return string.Compare(this.Name, obj.Name, StringComparison.Ordinal);
    }
}