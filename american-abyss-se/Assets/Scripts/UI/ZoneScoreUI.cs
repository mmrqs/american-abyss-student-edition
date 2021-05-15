using System;
using TMPro;
using UnityEngine;

[Serializable]
public class ZoneScoreUI
{
    [SerializeField] public Area zone;
    [SerializeField] public TMP_Text scoreRed;
    [SerializeField] public TMP_Text scoreBlue;
    [SerializeField] public TMP_Text scoreGreen;
    [SerializeField] public TMP_Text scoreYellow;
}
