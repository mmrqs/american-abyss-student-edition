using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public List<Character> characters;

    [Header("Current character info")]
    public TMPro.TMP_Text characterName;

    public AreaManager areaManager;

    public TroopsManager troopManager;
    public List<Zone> zones;
    public CanvasGroup recruitMessage;
    public TMPro.TMP_Text popUpMessage;
    
    public Button recruitment;
    public Button movingButton;
    public Button attackingButton;

    private int index;
    private Character currentCharacter;
    public Character Character => currentCharacter;
    
    private Mode currentMode;
    
    public Mode CurrentMode
    {
        get => currentMode;
        set => currentMode = value;
    }

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
    
    public void NextTurn()
    {
        if (index >= characters.Count)
            index = 0;

        currentCharacter = characters[index];
        characterName.SetText(currentCharacter.Name);
        recruitment.enabled = true;
        index++;

        CurrentMode = Mode.DEFAULT;
        
        recruitment.gameObject.SetActive(true);
        attackingButton.gameObject.SetActive(false);
        movingButton.gameObject.SetActive(false);
        areaManager.Init();
    }

    public void Recruit()
    {
        int numberOfUnits = areaManager.troopManager.units
            .Where(b => b.Character.Name == currentCharacter.Name)
            .Sum(b => b.NumberOfUnits);
        
        if (currentCharacter.NumberOfUnits > numberOfUnits)
        {
            DisplayMessagePopUp("Select an area to place your new unit.");

            recruitment.enabled = false;
            List<Zone> zones = troopManager.GetZonesWhereCharacterHasUnits(currentCharacter);
            if (zones.Count == 0)
                zones = troopManager.GetAllZones();
            areaManager.StartFlashing(zones);
            CurrentMode = Mode.RECRUIT;
        }
        recruitment.gameObject.SetActive(false);
        movingButton.gameObject.SetActive(true);
    }

    public void Attack()
    {
        DisplayMessagePopUp("Select an area to attack.");
        CurrentMode = Mode.ATTACK;
        areaManager.AllowedZones = troopManager.units
            .Where(u => u.Character == currentCharacter)
            .Select(u => u.Area)
            .ToList();
        List<Zone> zones = troopManager.GetZonesWhereCharacterHasUnits(currentCharacter);
        if (zones.Count == 0)
            zones = troopManager.GetAllZones();
        areaManager.StartFlashing(zones);
        attackingButton.gameObject.SetActive(false);
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
            message = " you win";
            troopManager.RemoveUnit(character, area);
            for(int i = 0; i < currentCharacter.NumberOfTroopsDestroyed; i++)
                troopManager.RemoveUnit(character, area);
            areaManager.NumberOfUnits = troopManager.GetNumberOfUnitsInArea(area, character);
            loosing = character;
        }
        else
        {
            message = " you loose";
            troopManager.RemoveUnit(currentCharacter, area);
            areaManager.NumberOfUnits = troopManager.GetNumberOfUnitsInArea(area, currentCharacter);
            loosing = currentCharacter;
        }
        
        DisplayMessagePopUp("You VS " + character.Name + " : " + message + " (result : " + die + "), choose an area where to move the removing units.");

        if (troopManager.GetNumberOfUnitsInArea(area, loosing) != 0)
        {
            areaManager.character = loosing;
            areaManager.ActingZones.Add(area);
            Debug.Log(areaManager.troopManager.GetZones(areaManager.troopManager.GetZone(area).Surroundings).Count);
            areaManager.StartFlashing(areaManager.troopManager.GetZones(areaManager.troopManager.GetZone(area).Surroundings));
            Debug.Log("mooooovee");
            CurrentMode = Mode.MOVE;
        }
    }
    
    public void MoveUnits()
    {
        DisplayMessagePopUp("Select an area where you want your troops to move.");
        CurrentMode = Mode.MOVE;
        List<Zone> zones = troopManager.GetZonesWhereCharacterHasUnits(currentCharacter);
        areaManager.StartFlashing(zones);
        attackingButton.gameObject.SetActive(true);
        movingButton.gameObject.SetActive(false);
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
}
