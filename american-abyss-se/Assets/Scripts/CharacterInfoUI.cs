using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour
{
    public TMPro.TMP_Text garison;

    public TMPro.TMP_Text controlledAreas;
    
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

    private void BuildUI()
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
        var percentage = gameManager.GetVictoryPercentage(gameManager.Character);
        victoryPercentage.SetText(percentage.ToString("0.00") + " %");
        victoryTracker.value = percentage / 100;
    }
}
