using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerUI : MonoBehaviour
{
    public GameObject menu;
    private bool isActive;
    
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayMenu()
    {
        menu.SetActive(!isActive);
        isActive = !isActive;
    }
}
