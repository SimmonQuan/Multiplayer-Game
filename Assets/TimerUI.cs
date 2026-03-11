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
        if (GameManager.Instance == null) //ensures gamemanager has started
        {
            return;
        }
        if (!GameManager.Instance.IsSpawned)
        {
            return;
        }
        int seconds = Mathf.CeilToInt(GameManager.Instance.timeRemaining.Value); //reads time remaining and stores it as an integer
        uiText.text = "Time: " + seconds; //changes text on screen to allow player to see time remaining in the game
    }
}