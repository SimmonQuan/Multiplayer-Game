using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    public static WinScreen Instance;
    public GameObject player1WinPanel;
    public GameObject player2WinPanel;

    void Awake()
    {
        Instance = this;
        player1WinPanel.SetActive(false); //at the start of the game, hide both winners screens
        player2WinPanel.SetActive(false);
    }

    public void ShowPlayer1Wins()
    {
        player1WinPanel.SetActive(true);
        //show player 1 won, freeze game
        Time.timeScale = 0f;
        StartCoroutine(RestartAfterDelay(4f));
    }

    public void ShowPlayer2Wins()
    {
        player2WinPanel.SetActive(true);
        //show player 2 won, freeze game
        Time.timeScale = 0f; //freeze
        StartCoroutine(RestartAfterDelay(4f));
    }

    IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); //after delay, restart the scene to play again
        Time.timeScale = 1f; //resume
        SceneManager.LoadScene("SampleScene");
    }
}