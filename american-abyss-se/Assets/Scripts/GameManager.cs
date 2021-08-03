using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public List<Character> characters;

    [Header("Current character info")]
    public TMPro.TMP_Text characterName;

    public AreaManager areaManager;

    public TroopsManager troopManager;
    public CanvasGroup recruitMessage;
    public TMPro.TMP_Text popUpMessage;
    
    public Button recruitment;
    public Button movingButton;
    public Button attackingButton;
    public Button skipButton;

    private int index;
    private Character currentCharacter;
    public Character Character => currentCharacter;
    
    private Mode currentMode;
    
    private bool recruiting = false;
    private bool moving = false;
    private bool fighting = false;
    
    public Mode CurrentMode
    {
        get => currentMode;
        set => currentMode = value;
    }

    public FightingPopUpUI fightingPopUpUI;

    public WinningUI winningUI;
    public bool test = false;
    void Start()
    {
        if (characters == null)
            throw new Exception("Empty character list");
        recruitMessage.gameObject.SetActive(false);

        // we make the order of player random
        Random rng = new Random();
        characters = characters.OrderBy(a => rng.Next()).ToList();
        NextTurn();
        
        
    }

    void Update()
    {
        /*if (!test)
        {
            List<Battalion> boardCopy = troopManager.Units.Select(x=> x.Clone()).ToList();
            AI ai = new AI();
            ai.test(Character, boardCopy, characters);
            test = true;
        }*/
        
    }

    public void NextTurn()
    {
        if (index >= characters.Count)
            index = 0;

        currentCharacter = characters[index];
        if (currentCharacter.Name == "Dr. Green")
            currentCharacter.AmountOfMoneyToHave -= troopManager.GetTotalNumberOfUnitsInField(currentCharacter);
        
        characterName.SetText(currentCharacter.Name);
        recruitment.enabled = true;
        index++;

        CurrentMode = Mode.DEFAULT;
        
        recruitment.gameObject.SetActive(true);
        attackingButton.gameObject.SetActive(false);
        movingButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(true);
        CheckEndGame();
        areaManager.Init();
        recruiting = false;
        moving = false;
        fighting = false;
    }

    public void NextAction()
    {
        CurrentMode = Mode.DEFAULT;
        if (recruitment.IsActive())
        {
            recruitment.gameObject.SetActive(false);
            movingButton.gameObject.SetActive(true);
        } else if (movingButton.IsActive())
        {
            attackingButton.gameObject.SetActive(true);
            movingButton.gameObject.SetActive(false);
        } else if (attackingButton.IsActive())
        {
            attackingButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
        }
        recruiting = false;
        moving = false;
        fighting = false;
        areaManager.Init();
    }

    public void CancelAction()
    {
        recruiting = false;
        moving = false;
        fighting = false;
        areaManager.Init();
        areaManager.InitMovingTroops();
    }

    public void Recruit()
    {
        if (!recruiting)
        {
            recruiting = true;
            int numberOfUnits = troopManager.GetTotalNumberOfUnitsInField(Character);
        
            if (currentCharacter.NumberOfUnits > numberOfUnits)
            {
                DisplayMessagePopUp("Select an area to place your new unit.");

                //recruitment.enabled = false;
                List<Zone> zones = troopManager.GetZonesWhereCharacterHasUnits(currentCharacter);
                if (zones.Count == 0)
                    zones = troopManager.GetAllZones();
                areaManager.StartFlashing(zones);
                CurrentMode = Mode.RECRUIT;
            }
        }
    }

    public void Attack()
    {
        if (!fighting)
        {
            fighting = true;
            DisplayMessagePopUp("Select an area to attack.");
            CurrentMode = Mode.ATTACK;
            areaManager.AllowedZones = troopManager.units
                .Where(u => u.Character == currentCharacter && troopManager.GetCharactersInZone(u.Area).Count > 1)
                .Select(u => u.Area)
                .ToList();
            List<Zone> zones = troopManager.GetZonesWhereCharacterHasUnits(currentCharacter);
            zones = zones.Where(z => troopManager.GetCharactersInZone(z.Name).Count > 1).ToList();
            if (zones.Count != 0)
                areaManager.StartFlashing(zones);
        }
    }

    public void InitiateFight(Area area, Character character)
    {
        Debug.Log(area);
        Random rnd = new Random();
        int die = rnd.Next(1, 7);
        string message = "";

        Character loosing;
        if (die <= troopManager.GetNumberOfUnitsInArea(area, currentCharacter))
        {
            //troopManager.RemoveUnit(character, area);
            for(var i = 0; i < currentCharacter.NumberOfTroopsDestroyed; i++)
                troopManager.RemoveUnit(character, area);
            areaManager.NumberOfUnits = troopManager.GetNumberOfUnitsInArea(area, character);
            loosing = character;
        }
        else
        {
            troopManager.RemoveUnit(currentCharacter, area);
            areaManager.NumberOfUnits = troopManager.GetNumberOfUnitsInArea(area, currentCharacter);
            loosing = currentCharacter;
        }
        
        fightingPopUpUI.BuildUI(currentCharacter.Name, character.Name, die, loosing.Name, currentCharacter.NumberOfTroopsDestroyed);

        if (troopManager.GetNumberOfUnitsInArea(area, loosing) != 0)
        {
            areaManager.character = loosing;
            areaManager.ActingZones.Add(area);
            areaManager.StartFlashing(areaManager.troopManager.GetZones(areaManager.troopManager.GetZone(area).Surroundings));
            CurrentMode = Mode.MOVE;
        }
        attackingButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }
    
    public void MoveUnits()
    {
        if (!moving)
        {
            moving = true;
            DisplayMessagePopUp("Select an area where you want your troops to move.");
            CurrentMode = Mode.MOVE;
            List<Zone> zones = troopManager.GetZonesWhereCharacterHasUnits(currentCharacter);
            areaManager.StartFlashing(zones);   
        }
    }
    
    public void DisplayMessagePopUp(string message)
    {
        popUpMessage.SetText(message);
        recruitMessage.gameObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(recruitMessage, recruitMessage.alpha, 0));
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 1)
    {
        yield return new WaitForSeconds(1.5f);
        var startingTime = Time.time;

        while (true)
        {
            var timeSinceStarted = Time.time - startingTime;
            var percentageComplete = timeSinceStarted / lerpTime;

            var currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1)
            {
                cg.alpha = start;
                recruitMessage.gameObject.SetActive(false);
                break;
            } 

            yield return new WaitForFixedUpdate();
        }
    }

    public void CheckEndGame()
    {
        List<Character> winners = characters.Where(character => GetVictoryPercentage(character) >= 100).ToList();
        if(winners.Count != 0)
            winningUI.BuildUI(winners);
    }
    
    public float GetVictoryPercentage(Character character)
    {
        float percentage = 0;
        
        switch (character.Name)
        {
            case "Agent Yellow":
                percentage = (troopManager.GetTotalNumberOfUnitsInField(character)
                              / (float)troopManager.GetTotalNumberOfUnitsInField(
                                  characters.Find(c => c.Name == "Colonel Red"))) * 100;
                break;
            case "Colonel Red":
                percentage = ((troopManager.GetTotalControlledZones(character) +
                               troopManager.GetTotalNumberOfUnitsInField(character)) / (float)character.NbOfTerritoriesToControl) * 100;
                break;
            case "Dr. Green":
                percentage = ((15 - character.AmountOfMoneyToHave) / 30) * 100 + ((troopManager.GetTotalControlledZones(character) +
                    troopManager.GetTotalNumberOfUnitsInField(character)) / (float)character.NbOfTerritoriesToControl) * 100 / 2;
                break;
            case " President Blue":
                percentage = (troopManager.GetTotalControlledZones(character) / (float)character.NbOfTerritoriesToControl) * 100;
                break;
        }
        return percentage;
    }
}
