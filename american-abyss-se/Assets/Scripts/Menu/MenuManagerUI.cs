using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerUI : MonoBehaviour
{
    public GameObject menu;
    private bool isActiveMenu;

    public GameObject options;
    private bool isActiveOptions;
    
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        options.SetActive(false);
        isActiveMenu = false;
        isActiveOptions = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayMenu()
    {
        menu.SetActive(!isActiveMenu);
        isActiveMenu = !isActiveMenu;
        
        if(isActiveOptions)
            options.SetActive(false);
        isActiveOptions = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void DisplayRules()
    {
        options.SetActive(true);
        isActiveOptions = true;
    }
}
