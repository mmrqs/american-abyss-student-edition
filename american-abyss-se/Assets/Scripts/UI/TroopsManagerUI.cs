using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopsManagerUI : MonoBehaviour
{
    public List<ZoneScoreUI> areas;
    
    public void BuildUI(List<Battalion> units)
    {

        units.ForEach(u =>
        {
            ZoneScoreUI zone = areas.Find(a => a.zone == u.Area);
            switch (u.Character.Name)
            {
                case "Agent Yellow":
                    zone.scoreYellow.SetText(u.NumberOfUnits.ToString());
                    break;
                case "Colonel Red":
                    zone.scoreRed.SetText(u.NumberOfUnits.ToString());
                    break;
                case "Dr. Green":
                    zone.scoreGreen.SetText(u.NumberOfUnits.ToString());
                    break;
                case " President Blue":
                    zone.scoreBlue.SetText(u.NumberOfUnits.ToString());
                    break;
            }
        });
    }
}
