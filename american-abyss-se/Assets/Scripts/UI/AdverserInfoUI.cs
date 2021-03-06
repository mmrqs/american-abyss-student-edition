using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdverserInfoUI : MonoBehaviour
{
    public List<GameObject> images;
    public List<TMPro.TMP_Text> garizon;
    public List<TMPro.TMP_Text> controledAreas;
    public List<TMPro.TMP_Text> money;
    public List<Image> moneyImage;
    public List<TMPro.TMP_Text> percentages;

    
    public GameManager gameManager;
    public TroopsManager troopsManager;

    // Update is called once per frame
    void Update()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        SetColors();
        SetGarizon();
        SetControledAreas();
        SetVictoryConditions();
    }

    private void SetColors()
    {
        var index = gameManager.characters.FindIndex(i => i == gameManager.Character);
        for (var i = 0; i < images.Count; i++)
        {
            switch (gameManager.characters[(index + (i + 1)) % gameManager.characters.Count].Name)
            {
                case Name.PRESIDENT_BLUE:
                    images[i].GetComponent<Image>().color = Color.blue;
                    money[i].gameObject.SetActive(false);
                    moneyImage[i].gameObject.SetActive(false);
                    break;
                case Name.DR_GREEN:
                    images[i].GetComponent<Image>().color = Color.green;
                    money[i].SetText(Convert.ToString(gameManager.moneyGreen + "K/"
                        + "15K"));
                    money[i].gameObject.SetActive(true);
                    moneyImage[i].gameObject.SetActive(true);
                    break;
                case Name.COLONEL_RED:
                    images[i].GetComponent<Image>().color = Color.red;
                    money[i].gameObject.SetActive(false);
                    moneyImage[i].gameObject.SetActive(false);
                    break;
                case Name.AGENT_YELLOW:
                    images[i].GetComponent<Image>().color = Color.yellow;
                    money[i].gameObject.SetActive(false);
                    moneyImage[i].gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void SetGarizon()
    {
        var index = gameManager.characters.FindIndex(i => i == gameManager.Character);
        for (var i = 0; i < garizon.Count; i++)
        {
            string s = Convert.ToString(gameManager.characters[(index + (i + 1)) % gameManager.characters.Count].NumberOfUnits -
                       troopsManager.GetTotalNumberOfUnitsInField(
                           gameManager.characters[(index + (i + 1)) % gameManager.characters.Count])
                       + "/"
                       + gameManager.characters[(index + (i + 1)) % gameManager.characters.Count].NumberOfUnits);
            
            garizon[i].SetText(s);
        }
    }

    private void SetControledAreas()
    {
        var index = gameManager.characters.FindIndex(i => i == gameManager.Character);
        for (var i = 0; i < controledAreas.Count; i++)
        {
            controledAreas[i].SetText(Convert.ToString(troopsManager.GetTotalControlledZones(gameManager.characters[(index + (i + 1)) % gameManager.characters.Count])));
        }
    }
    
    private void SetVictoryConditions()
    {
        var index = gameManager.characters.FindIndex(i => i == gameManager.Character);
        for (int i = 0; i < percentages.Count; i++)
        {
            var percentage = gameManager.GetVictoryPercentage(gameManager.characters[(index + (i + 1)) % gameManager.characters.Count]);
            percentages[i].SetText(percentage.ToString("0") + " %");
        }
    }
}
