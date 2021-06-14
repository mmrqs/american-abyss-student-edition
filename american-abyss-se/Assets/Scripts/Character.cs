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
}