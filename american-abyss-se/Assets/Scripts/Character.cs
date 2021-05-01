using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character")]
public class Character : ScriptableObject
{
    [SerializeField] protected string _name;
    public string Name => _name;
}