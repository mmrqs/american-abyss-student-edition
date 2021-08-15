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
    public bool moving = false;
    private bool fighting = false;
    public Area areaOfFight;
    
    public int superPowerBlue;
    public int moneyGreen;
    private bool firstTurn;

    public List<Name> AIList;
    public AIHelper aiHelper;
    public AIRecapUI recapUI;
    
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
        moneyGreen = 0;
        if (characters == null)
            throw new Exception("Empty character list");
        recruitMessage.gameObject.SetActive(false);
        firstTurn = true;
        aiHelper = new AIHelper(troopManager, fightingPopUpUI, recapUI);
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
        if (!firstTurn)
        {
            if (characters[((characters.IndexOf(currentCharacter) - 1) % characters.Count + characters.Count)% characters.Count].Name == Name.DR_GREEN)
                CalculateMoneyGreen();
        }

        
        if (currentCharacter.Name == Name.PRESIDENT_BLUE)
            superPowerBlue = 2;
        
        characterName.SetText(currentCharacter.Name.GetString());
        recruitment.enabled = true;
        index++;

        CurrentMode = Mode.DEFAULT;

        if (!AIList.Contains(currentCharacter.Name))
        {
            recruitment.gameObject.SetActive(true);
            attackingButton.gameObject.SetActive(false);
            movingButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(true);
        }
        else
        {
            aiHelper.SimulatePlayer(troopManager.units.Select(x=> x.Clone()).ToList(), currentCharacter, characters);
        }
        
        CheckEndGame();
        areaManager.Init();
        recruiting = false;
        moving = false;
        fighting = false;
        firstTurn = false;
    }

    private void CalculateMoneyGreen()
    {
        moneyGreen += troopManager.GetTotalNumberOfUnitsInField(currentCharacter);
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
            var zones = troopManager.GetZonesWhereCharacterHasUnits(currentCharacter)
                .Where(z => troopManager.GetCharactersInZone(z.Name).Count > 1)
                .ToList();
            if (zones.Count != 0)
                areaManager.StartFlashing(zones);
        }
    }

    public void InitiateFight(Area area, Character character)
    {
        Random rnd = new Random();
        int die = rnd.Next(1, 7);
        string message = "";

        Character winner;
        Character looser;
        if (die <= troopManager.GetNumberOfUnitsInArea(area, currentCharacter))
        {
            //troopManager.RemoveUnit(character, area);
            for(var i = 0; i < currentCharacter.NumberOfTroopsDestroyed; i++)
                troopManager.RemoveUnit(character, area);
            areaManager.NumberOfUnits = troopManager.GetNumberOfUnitsInArea(area, character);
            winner = currentCharacter;
            looser = character;
        }
        else
        {
            troopManager.RemoveUnit(currentCharacter, area);
            areaManager.NumberOfUnits = troopManager.GetNumberOfUnitsInArea(area, currentCharacter);
            winner = character;
            looser = currentCharacter;
        }

        fightingPopUpUI.BuildUI(currentCharacter.Name.GetString(), character.Name.GetString(), die, winner.Name.GetString(), currentCharacter.NumberOfTroopsDestroyed);
        
        if (troopManager.GetNumberOfUnitsInArea(area, looser) != 0)
        {
            areaManager.character = looser;
            areaManager.ActingZones.Add(area);
            areaManager.NbOfUnitsToMove = troopManager.GetNumberOfUnitsInArea(area, looser);
            areaOfFight = area;
        }
        attackingButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }

    public void afterFight()
    {
        if (areaOfFight != Area.FOO)
        {
            areaManager.StartFlashing(areaManager.troopManager.GetZones(areaManager.troopManager.GetZone(areaOfFight).Surroundings));
            CurrentMode = Mode.MOVE;
            areaOfFight = Area.FOO;   
        }
    }
    
    public void MoveUnits()
    {
        if (!moving)
        {
            DisplayMessagePopUp("Select an area where you want your troops to move.");
            CurrentMode = Mode.MOVE;
            List<Zone> zones = Character.Name == Name.PRESIDENT_BLUE ? 
                troopManager.GetZonesWhereCharacterHasCertainAmountOfUnits(currentCharacter, 1) : 
                troopManager.GetZonesWhereCharacterHasUnits(currentCharacter);
            areaManager.StartFlashing(zones);   
        }
    }
    
    public void DisplayMessagePopUp(string message)
    {
        popUpMessage.SetText(message);
        recruitMessage.gameObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(recruitMessage, recruitMessage.alpha, 0));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 1)
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
            case Name.AGENT_YELLOW:
                percentage = ((troopManager.GetTotalNumberOfUnitsInField(character) - 1)
                              / (float)troopManager.GetTotalNumberOfUnitsInField(
                                  characters.Find(c => c.Name == Name.COLONEL_RED))) * 100;
                break;
            case Name.COLONEL_RED:
                percentage = ((troopManager.GetTotalControlledZones(character) +
                               troopManager.GetTotalNumberOfUnitsInField(character)) / ((float)character.NbOfTerritoriesToControl + 1)) * 100;
                break;
            case Name.DR_GREEN:
                float m = moneyGreen > 15 ? 15 : moneyGreen;
                float n = troopManager.GetZonesWhereCharacterHasUnits(character).Count > 4
                    ? 4
                    : troopManager.GetZonesWhereCharacterHasUnits(character).Count;
                percentage = (((float)(m) / (float)30) * 100) + (((float)n/(float)8) * 100);
                break;
            case Name.PRESIDENT_BLUE:
                percentage = (troopManager.GetTotalControlledZones(character) / (float)character.NbOfTerritoriesToControl) * 100;
                break;
        }
        return percentage;
    }
}
