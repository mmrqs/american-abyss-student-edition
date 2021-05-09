using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public List<Character> characters;

    [Header("Current character info")]
    public TMPro.TMP_Text characterName;

    public Image imageCurrent;

    public List<Image> images;
    

    private int index = 0;
    private Character currentCharacter;

    private bool recruitment = false;
    private GameObject current = null;
    private Camera cam;
    public LayerMask layer;

    void Start()
    {
        if (characters == null)
            throw new Exception("Empty character list");

        // we make the order of player random
        System.Random rng = new System.Random();
        characters = characters.OrderBy(a => rng.Next()).ToList();
        cam = Camera.main;
        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (recruitment)
        {
            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 8;

            if (Physics.Raycast(camRay, out hit, layerMask))
            {
                current.transform.position = new Vector3(hit.point.x, current.transform.position.y, hit.point.z);
            }
        }
    }

    public void NextTurn()
    {
        if (index >= characters.Count)
            index = 0;

        currentCharacter = characters[index];
        characterName.SetText(currentCharacter.Name);

        index++;
    }

    public void Recruit()
    {
        recruitment = true;
        current = Instantiate(currentCharacter.Unit.gameObject, new Vector3((float)0.15, (float)2.62, (float)-2.8), Quaternion.identity);       
    }
}
