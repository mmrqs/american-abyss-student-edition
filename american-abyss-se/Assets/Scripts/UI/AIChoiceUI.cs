using UnityEngine;

public class AIChoiceUI : MonoBehaviour
{
    public Name name;
    private const string SELECT = "OK";
    private const string UNSELECT = "X";
    public TMPro.TMP_Text text;
    public GameManager gameManager;
    
    
    public void Check()
    {
        if (text.text == SELECT)
        {
            text.SetText(UNSELECT);
            UnSelect(name);
        }
        else
        {
            text.SetText(SELECT);
            Select(name);
        }
    }
    
    // Update is called once per frame
    private void Select(Name name)
    {
        gameManager.AIList.Add(name);
    }

    private void UnSelect(Name name)
    {
        gameManager.AIList.Remove(name);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
