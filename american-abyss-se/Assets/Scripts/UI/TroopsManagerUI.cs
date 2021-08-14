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
                case Name.AGENT_YELLOW:
                    zone.scoreYellow.SetText(u.NumberOfUnits.ToString());
                    break;
                case Name.COLONEL_RED:
                    zone.scoreRed.SetText(u.NumberOfUnits.ToString());
                    break;
                case Name.DR_GREEN:
                    zone.scoreGreen.SetText(u.NumberOfUnits.ToString());
                    break;
                case Name.PRESIDENT_BLUE:
                    zone.scoreBlue.SetText(u.NumberOfUnits.ToString());
                    break;
            }
        });
    }
}
