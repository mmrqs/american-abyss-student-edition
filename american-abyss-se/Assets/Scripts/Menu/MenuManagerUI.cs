using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerUI : MonoBehaviour
{
    public GameObject menu;
    private bool isActiveMenu;

    public GameObject options;
    public GameObject background;
    
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        options.SetActive(false);
        background.SetActive(false);
        isActiveMenu = false;
    }
    
    public void DisplayMenu()
    {
        menu.SetActive(!isActiveMenu);
        isActiveMenu = !isActiveMenu;
        
        options.SetActive(false);
        background.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void DisplayRules()
    {
        options.SetActive(true);
    }

    public void DisplayBackground()
    {
        background.SetActive(true);
    }
}
