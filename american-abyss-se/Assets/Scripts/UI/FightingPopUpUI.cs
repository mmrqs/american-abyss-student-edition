using UnityEngine;

public class FightingPopUpUI : MonoBehaviour
{
    public TMPro.TMP_Text player1Text;
    public TMPro.TMP_Text player2Text;
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text winnerText;
    public TMPro.TMP_Text troopsDestroyedText;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    public void BuildUI(string player1Name, string player2Name, int score, string winningPlayerName, int troopsDestroyed)
    {
        player1Text.SetText(player1Name);
        player2Text.SetText(player2Name);
        scoreText.SetText(score.ToString());
        troopsDestroyedText.SetText(troopsDestroyed.ToString());
        winnerText.SetText(winningPlayerName);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        gameManager.afterFight();
    }
}
