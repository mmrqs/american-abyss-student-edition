using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class MovingTroopsNumberUI : MonoBehaviour
{
    public TMPro.TMP_Text victoryText;

    public Button minusButton;

    public Button plusButton;

    private int maxUnits;

    private int number;

    public int Number => number;

    private void Start()
    {
        Init();        
    }

    public void BuildUI(int max)
    {
        maxUnits = max;
        victoryText.SetText(number.ToString(CultureInfo.InvariantCulture));
        gameObject.SetActive(true);
        CheckButtons();
    }

    public void Plus()
    {
        number++;
        victoryText.SetText(number.ToString(CultureInfo.InvariantCulture));
        CheckButtons();
    }

    public void Minus()
    {
        number--;        
        victoryText.SetText(number.ToString(CultureInfo.InvariantCulture));
        CheckButtons();
    }

    private void CheckButtons()
    {
        if(number <= 0)
            minusButton.gameObject.SetActive(false);
        
        if(number >= maxUnits)
            plusButton.gameObject.SetActive(false);
        
        if (number <= 0 || number >= maxUnits) return;
        
        minusButton.gameObject.SetActive(true);
        plusButton.gameObject.SetActive(true);
    }

    public void Init()
    {
        gameObject.SetActive(false);   
        number = 1;                                                                                 
    }
}
