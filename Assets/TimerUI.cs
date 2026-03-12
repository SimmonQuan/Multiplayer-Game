using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    private TextMeshProUGUI uiText;

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (GameManager.Instance == null) //ensures gamemanager exists
        {
            return;
        }
        if (!GameManager.Instance.IsSpawned) //ensures gamemanager started
        {
            return;
        }
    
        int seconds = Mathf.CeilToInt(GameManager.Instance.timeRemaining.Value); //reads time remaining and stores it as an integer
        uiText.text = "Time: " + seconds; //update UI that displays time remaining in match
    }
}