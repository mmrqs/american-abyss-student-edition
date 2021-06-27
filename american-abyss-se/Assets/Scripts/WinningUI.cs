using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinningUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TMP_Text victoryText;
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void BuildUI(List<Character> winners)
    {
        var text = winners.Aggregate("", (current, winner) => current + winner.Name + " won, \n He managed to " + winner.Aim);
        victoryText.SetText(text);
        gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void BackMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
