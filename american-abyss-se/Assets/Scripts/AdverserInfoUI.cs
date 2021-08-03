using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

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
    
    // Start is called before the first frame update
    void Start()
    {
        //SetColors();
    }

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
        /*if(gameManager.Character.Name != "Dr. Green")
            SetMoney(gameManager.characters.FindIndex(i => i.Name == "Dr. Green"));
        else
            foreach (var item in money)
                item.gameObject.SetActive(false);*/
        SetVictoryConditions();
    }

    private void SetColors()
    {
        var index = gameManager.characters.FindIndex(i => i == gameManager.Character);
        for (var i = 0; i < images.Count; i++)
        {
            switch (gameManager.characters[(index + (i + 1)) % gameManager.characters.Count].Name)
            {
                case " President Blue":
                    images[i].GetComponent<Image>().color = Color.blue;
                    money[i].gameObject.SetActive(false);
                    moneyImage[i].gameObject.SetActive(false);
                    break;
                case "Dr. Green":
                    images[i].GetComponent<Image>().color = Color.green;
                    money[i].SetText(Convert.ToString(15 - gameManager.characters[(index + (i + 1)) % gameManager.characters.Count].AmountOfMoneyToHave + "K/"
                        + "15K"));
                    money[i].gameObject.SetActive(true);
                    moneyImage[i].gameObject.SetActive(true);
                    break;
                case "Colonel Red":
                    images[i].GetComponent<Image>().color = Color.red;
                    money[i].gameObject.SetActive(false);
                    moneyImage[i].gameObject.SetActive(false);
                    break;
                case "Agent Yellow":
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
