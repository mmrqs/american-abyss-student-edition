using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    private RaycastHit theObject;
    private Camera cam;
    
    private GameObject current;
    private Color currentColor;

    public TroopsManager troopManager;
    public GameManager gameManager;
    public ChooseFactionUI chooseFactionUI;
    
    private bool test;

    public List<Area> AllowedZones { get; set; }
    public List<Character> Ennemies { get; set; }

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.CurrentMode != Mode.DEFAULT)
        {
            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camRay, out theObject))
            {
                switch (gameManager.CurrentMode)
                {
                    case Mode.RECRUIT:
                        if (current == null || !current.Equals(theObject.transform.gameObject))
                        {
                            if(current != null)
                                current.transform.gameObject.GetComponent<Renderer>().material.color = currentColor;
                            current = theObject.transform.gameObject;
                            currentColor = current.transform.gameObject.GetComponent<Renderer>().material.color;
                        }
                        theObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));

                        if (Input.GetMouseButton(0))
                        {
                            current.transform.gameObject.GetComponent<Renderer>().material.color = currentColor;
                            PutTroopOnArea(current.gameObject);
                        }
                        break;
                    
                    case Mode.ATTACK:
                        if (current == null || !current.Equals(theObject.transform.gameObject))
                        {
                            if(current != null)
                                current.transform.gameObject.GetComponent<Renderer>().material.color = currentColor;
                            current = theObject.transform.gameObject;
                            currentColor = current.transform.gameObject.GetComponent<Renderer>().material.color;
                        }

                        if (AllowedZones.Contains((Area)Enum.Parse(typeof(Area), current.gameObject.name)))
                        {
                            theObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));

                            if (Input.GetMouseButton(0))
                            {
                                current.transform.gameObject.GetComponent<Renderer>().material.color = currentColor;
                                AttackArea(current.gameObject);
                            }
                        }
                        break;
                }
            }
        }
    }

    public void AttackArea(GameObject area)
    {
        chooseFactionUI.AreaUnderAttack = (Area) Enum.Parse(typeof(Area), area.name);
        chooseFactionUI.Characters = troopManager
            .GetCharactersInZone(chooseFactionUI.AreaUnderAttack)
            .Where(c => c.Name != gameManager.Character.Name)
            .ToList();
        chooseFactionUI.show();
    }
    
    public void PutTroopOnArea(GameObject area)
    {
        troopManager.AddUnitToZone(gameManager.Character, (Area)Enum.Parse(typeof(Area), area.name));
        gameManager.CurrentMode = Mode.DEFAULT;
    }
}
