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
    
    public Image imageCurrent;

    public List<Image> images;

    public CanvasGroup recruitMessage;
    public TMPro.TMP_Text popUpMessage;
    
    public Button recruitment;

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
        System.Random rng = new System.Random();
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
            CurrentMode = Mode.RECRUIT;
        }
        
    }

    public void Attack()
    {
        DisplayMessagePopUp("Select an area to attack.");
        CurrentMode = Mode.ATTACK;
        areaManager.AllowedZones = troopManager.units
            .Where(u => u.Character == currentCharacter)
            .Select(u => u.Area)
            .ToList();
    }

    public void InitiateFight(Area area, Character character)
    {
        Random rnd = new Random();
        int die = rnd.Next(1, 7);
        string message = "";
        if (die <= troopManager.GetNumberOfUnitsInArea(area, currentCharacter))
        {
            message = " you win";
            troopManager.RemoveUnit(character, area);
        }
        else
        {
            message = " you loose";
            troopManager.RemoveUnit(currentCharacter, area);
        }
        DisplayMessagePopUp("You VS " + character.Name + " : " + message + " (result : " + die + ")");
    }
    
    public void MoveUnits()
    {
        DisplayMessagePopUp("Select an area where you want your troops to move.");
        CurrentMode = Mode.MOVE;
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
