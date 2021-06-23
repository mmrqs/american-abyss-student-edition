using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour
{
    public TMPro.TMP_Text garison;

    public TMPro.TMP_Text controlledAreas;

    public Image photo;

    public TMPro.TMP_Text characterName;
    
    public TMPro.TMP_Text victoryPercentage;
    public TMPro.TMP_Text money;
    public GameObject gameObjectMoney;

    public GameManager gameManager;

    public TroopsManager troopsManager;
    public Slider victoryTracker;
    
    // Start is called before the first frame update
    void Start()
    {
        BuildUI();
    }

    // Update is called once per frame
    void Update()
    {
        BuildUI();
    }

    public void BuildUI()
    {
        garison.SetText(GetGarrison());
        controlledAreas.SetText(GetControlledAreas());
        SetVictoryConditions();
        if(gameManager.Character.Name == "Dr. Green")
            money.SetText(GetAmountOfMoney());
        else
            gameObjectMoney.SetActive(false);
    }

    private string GetGarrison()
    {
        return Convert.ToString(gameManager.Character.NumberOfUnits - troopsManager.GetTotalNumberOfUnitsInField(gameManager.Character) 
                                + " / "
                                + gameManager.Character.NumberOfUnits);
    }

    private string GetControlledAreas()
    {
        return Convert.ToString(troopsManager.GetTotalControlledZones(gameManager.Character));
    }

    private string GetAmountOfMoney()
    {
        gameObjectMoney.SetActive(true);
        return Convert.ToString(15 - gameManager.Character.AmountOfMoneyToHave)
               + "K / "
               + "15K";
    }

    private void SetVictoryConditions()
    {
        float percentage = 0;
        
        switch (gameManager.Character.Name)
        {
            case "Agent Yellow":
                percentage = (troopsManager.GetTotalNumberOfUnitsInField(gameManager.Character)
                              / (float)troopsManager.GetTotalNumberOfUnitsInField(
                                  gameManager.characters.Find(c => c.Name == "Colonel Red"))) * 100;
                break;
            case "Colonel Red":
                percentage = ((troopsManager.GetTotalControlledZones(gameManager.Character) +
                               troopsManager.GetTotalNumberOfUnitsInField(gameManager.Character)) / (float)gameManager.Character.NbOfTerritoriesToControl) * 100;
                break;
            case "Dr. Green":
                percentage = ((15 - gameManager.Character.AmountOfMoneyToHave) / 30) * 100 + ((troopsManager.GetTotalControlledZones(gameManager.Character) +
                    troopsManager.GetTotalNumberOfUnitsInField(gameManager.Character)) / (float)gameManager.Character.NbOfTerritoriesToControl) * 100 / 2;
                break;
            case " President Blue":
                percentage = (troopsManager.GetTotalControlledZones(gameManager.Character) / (float)gameManager.Character.NbOfTerritoriesToControl) * 100;
                break;
        }
        victoryPercentage.SetText(percentage.ToString("0.00") + " %");
        victoryTracker.value = percentage / 100;
    }
}
