using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class FightingPopUpUI : MonoBehaviour
{
    public TMPro.TMP_Text victoryText;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildUI()
    {
        
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
        
}
