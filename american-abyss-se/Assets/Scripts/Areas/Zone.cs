using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] protected Area _name;
    public Area Name => _name;

    [SerializeField] protected List<Area> _surroundings;
    public List<Area> Surroundings => _surroundings;
}
