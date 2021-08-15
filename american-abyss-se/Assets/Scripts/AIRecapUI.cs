using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRecapUI : MonoBehaviour
{
    public TMPro.TMP_Text recruit;
    public TMPro.TMP_Text move;
    public TMPro.TMP_Text fight;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void BuildUI(Action action, bool winner)
    {
        string message = winner ? " won." : "lost"; 
        if(action.recruitingZone != Area.FOO)
            recruit.SetText("Recruiting zone :" + action.recruitingZone.GetString());
        else
            recruit.SetText("No recruitment");
        if(action.movingZone.Item1 != Area.FOO)
            move.SetText("Move from : " 
                         + action.movingZone.Item1.GetString() 
                         + " to : " + action.movingZone.Item2.GetString() 
                         + " " + action.movingZone.Item3 + " units");
        else 
            move.SetText("No movements");
        if(action.fightingZone.Item1 != Area.FOO)
            fight.SetText("Fought : " + action.fightingZone.Item2.Name.GetString() + " on " + action.fightingZone.Item3.GetString() + " and " + message);
        else
        {
            fight.SetText("Did not fight.");
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
