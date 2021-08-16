using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorsZone
{
    [SerializeField] private Zone _zone;
    public Zone Zone => _zone;

    [SerializeField] private Color _color;
    public Color Color => _color;
}
