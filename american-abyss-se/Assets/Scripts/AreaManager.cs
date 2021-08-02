using System;
using System.Collections;
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
    
    public List<Area> ActingZones { get; set; }
    
    public Character character { get; set; }

    private bool choosen;
    public List<Zone> ColorsZones;

    private Coroutine flashing;

    public Coroutine Flashing
    {
        get => flashing;
        set => flashing = value;
    }

    private Coroutine flashing2;

    public MovingTroopsNumberUI movingTroopsUI;

    void Start()
    {
        cam = Camera.main;
        Init();
    }

    void Update()
    {
        if (gameManager.CurrentMode != Mode.DEFAULT)
        {
            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camRay, out theObject))
            {
                if (Input.GetMouseButton(0) && ColorsZones.Contains(troopManager.GetZone((Area) Enum.Parse(typeof(Area), theObject.transform.gameObject.name))))
                {
                    ActingZones.Add((Area) Enum.Parse(typeof(Area), theObject.transform.gameObject.name));
                    choosen = true;
                    
                    if(gameManager.CurrentMode == Mode.ATTACK)
                        AttackArea(ActingZones[0]);
                    if(gameManager.CurrentMode == Mode.RECRUIT)
                        PutTroopOnArea(ActingZones[0]);
                    if (gameManager.CurrentMode == Mode.MOVE && ActingZones.Count == 1)
                    {
                        choosen = false;
                        StopFlashing();
                        ResetColors();
                        
                        gameManager.DisplayMessagePopUp("Choose a zone to move your unit");
                        flashing2 = StartCoroutine(LightZones(troopManager
                            .GetSurroundingZonesInPerimeter(gameManager.Character.MovingZoneDistance, ActingZones[0])));
                        
                        movingTroopsUI.BuildUI(Input.mousePosition, troopManager.GetTotalNumberOfUnitsInField(gameManager.Character));
                    }
                    if(gameManager.CurrentMode == Mode.MOVE && ActingZones.Count == 2)
                        MoveTroop(ActingZones[0], ActingZones[1]);
                }
            }
            
        }
    }

    private IEnumerator LightZones(List<Zone> zones)
    {
        ColorsZones = zones;
        // we change the color
        while (!choosen)
        {
            foreach (var zone in zones)
            {
                zone.gameObject.GetComponent<Renderer>().material.color =
                    Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));
            }
            yield return null;
        }
    }

    private void ResetColors()
    {
        var zones = troopManager.GetAllZones();
        foreach (var zone in zones)
        {
            var ca = troopManager.GetMaster(zone);
            zone.gameObject.GetComponent<Renderer>().material.color 
                = ca == null ? Color.grey : ca.Color;
        }
    }

    public void AttackArea(Area area)
    {
        chooseFactionUI.AreaUnderAttack = area;
        chooseFactionUI.Characters = troopManager
            .GetCharactersInZone(chooseFactionUI.AreaUnderAttack)
            .Where(c => c.Name != gameManager.Character.Name)
            .ToList();
        Init();
        chooseFactionUI.show();
    }
    
    public void PutTroopOnArea(Area area)
    {
        troopManager.AddUnitToZone(gameManager.Character, area, NumberOfUnits);
        Init();
        gameManager.NextAction();
    }

    public void MoveTroop(Area startingZone, Area endingZone)
    {
        if (character == null)
            character = gameManager.Character;
        troopManager.MoveUnit(character, startingZone, endingZone, movingTroopsUI.Number);
        StopCoroutine(flashing2);
        movingTroopsUI.Init();
        Init();
        gameManager.NextAction();
    }

    public void StartFlashing(List<Zone> zones)
    {
        flashing = StartCoroutine(LightZones(zones));
    }

    public void StopFlashing()
    {
        if (Flashing != null)
        {
            StopCoroutine(Flashing);
            Flashing = null;
        }
    }
    
    public void Init()
    {
        StopFlashing();
        ColorsZones = new List<Zone>();
        NumberOfUnits = 1;
        ActingZones = new List<Area>();
        character = null;
        choosen = false;
        ResetColors();
        gameManager.CurrentMode = Mode.DEFAULT;
    }
}
