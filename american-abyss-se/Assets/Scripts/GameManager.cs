using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public List<Character> characters;

    [Header("Current character info")]
    public TMPro.TMP_Text characterName;

    public AreaManager areaManager;
    
    public Image imageCurrent;

    public List<Image> images;

    public CanvasGroup recruitMessage;

    public Button recruitment;

    private int index;
    private Character currentCharacter;
    public Character Character => currentCharacter;

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
    }

    public void Recruit()
    {
        int numberOfUnits = areaManager.troopManager.units
            .Where(b => b.Character.Name == currentCharacter.Name)
            .Sum(b => b.NumberOfUnits);
        Debug.Log(numberOfUnits);
        if (currentCharacter.NumberOfUnits > numberOfUnits)
        {
            recruitMessage.gameObject.SetActive(true);
            StartCoroutine(FadeCanvasGroup(recruitMessage, recruitMessage.alpha, 0));

            recruitment.enabled = false;
            areaManager.SelectArea();
        }
        
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

            if (percentageComplete >= 1) break;

            yield return new WaitForFixedUpdate();
        }
    }
}
