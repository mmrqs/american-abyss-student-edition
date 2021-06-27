using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] protected Color color;
    public Color Color => color;

    [SerializeField] protected string aim;
    public string Aim => aim;
}