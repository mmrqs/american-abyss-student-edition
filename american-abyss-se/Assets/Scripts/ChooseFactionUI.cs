using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFactionUI : MonoBehaviour
{
    public GameManager gameManager;
    public List<Button> buttons;

    public List<Character> Characters { get; set; }
    public Area AreaUnderAttack { get; set; }

    public void show()
    {
        for (int i = 0; i < Characters.Count; i++)
            buttons[i].GetComponentInChildren<TMP_Text>().SetText(Characters[i].Name);
        for(int i = Characters.Count; i < buttons.Count; i++)
            buttons[i].gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void selectFaction(TMP_Text faction)
    {
        gameManager.InitiateFight(AreaUnderAttack, Characters.First(c => c.Name.Equals(faction.text)));
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
