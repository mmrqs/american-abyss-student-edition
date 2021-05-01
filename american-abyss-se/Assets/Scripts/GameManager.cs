using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public List<Character> characters;

    [Header("field character's name")]
    public TMPro.TMP_Text characterName;

    private int index = 0;
    
    void Start()
    {
        if (characters == null)
            throw new Exception("Empty character list");

        // we make the order of player random
        System.Random rng = new System.Random();
        characters = characters.OrderBy(a => rng.Next()).ToList();

        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextTurn()
    {
        if (index >= characters.Count)
            index = 0;
        characterName.SetText(characters[index].Name);
        index++;
    }
}
