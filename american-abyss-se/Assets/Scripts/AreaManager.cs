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
    public int NumberOfUnits { get; set; }
    
    public Area? StartingZone { get; set; }
    public Area? EndingZone { get; set; }
    
    public Character character { get; set; }

    void Start()
    {
        cam = Camera.main;
        init();
    }

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
                    case Mode.MOVE:
                        if (current == null || !current.Equals(theObject.transform.gameObject))
                        {
                            if(current != null)
                                current.transform.gameObject.GetComponent<Renderer>().material.color = currentColor;
                            current = theObject.transform.gameObject;
                            currentColor = current.transform.gameObject.GetComponent<Renderer>().material.color;
                        }

                        //if (distanceBetweenAreas(current, theObject.transform.gameObject) == 1)
                        //{
                            theObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));

                            if (Input.GetMouseButton(0))
                            {
                                current.transform.gameObject.GetComponent<Renderer>().material.color = currentColor;
                                if (StartingZone == null)
                                    StartingZone = (Area)Enum.Parse(typeof(Area), current.gameObject.name);
                                else if (StartingZone != null && EndingZone == null)
                                    EndingZone = (Area)Enum.Parse(typeof(Area), current.gameObject.name);
                                
                                if(StartingZone != null && EndingZone != null)
                                    MoveTroop();
                            }
                        //}
                        break;
                }
            }
        }
    }
    
    private int distanceBetweenAreas(GameObject area1, GameObject area2)
    {
        Zone zone1 = area1.transform.gameObject.GetComponent<Zone>();
        Zone zone2 = area2.transform.gameObject.GetComponent<Zone>();

        int distance = 0;

        List<Area> researchingAreas = zone1.Surroundings;
        
        while (!researchingAreas.Contains(zone2.Name)&& zone2 != null)
        {
            foreach (Area area in researchingAreas)
                if(!researchingAreas.Contains(area))
                    researchingAreas.Add(area);
            distance++;
        }

        return distance;
    }

    public void AttackArea(GameObject area)
    {
        chooseFactionUI.AreaUnderAttack = (Area)Enum.Parse(typeof(Area), area.name);
        chooseFactionUI.Characters = troopManager
            .GetCharactersInZone(chooseFactionUI.AreaUnderAttack)
            .Where(c => c.Name != gameManager.Character.Name)
            .ToList();
        chooseFactionUI.show();
    }
    
    public void PutTroopOnArea(GameObject area)
    {
        troopManager.AddUnitToZone(gameManager.Character, (Area)Enum.Parse(typeof(Area), area.name), NumberOfUnits);
        gameManager.CurrentMode = Mode.DEFAULT;
        init();
    }

    public void MoveTroop()
    {
        if (character == null)
            character = gameManager.Character;
        troopManager.MoveUnit(character, (Area)StartingZone, (Area) EndingZone);
        gameManager.CurrentMode = Mode.DEFAULT;
        init();
    }

    public void init()
    {
        NumberOfUnits = 1;
        StartingZone = null;
        EndingZone = null;
        character = null;
    }
}
