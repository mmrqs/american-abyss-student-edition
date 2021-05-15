using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

public class AreaManager : MonoBehaviour
{
    private RaycastHit theObject;
    private Camera cam;
    
    private GameObject current;
    private Color currentColor;

    public TroopsManager troopManager;
    public GameManager gameManager;

    private bool test;
    
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camRay, out theObject))
            {
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
                    PutTroopOnArea(current.gameObject);
                    //test = false;
                }
            }
        }
    }

    public void SelectArea()
    {
        test = true;
    }
    
    public void PutTroopOnArea(GameObject area)
    {
        troopManager.AddUnitToZone(gameManager.Character, (Area)Enum.Parse(typeof(Area), area.name));
    }
}
