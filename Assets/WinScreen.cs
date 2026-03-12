using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    public static WinScreen Instance;
    public GameObject player1WinPanel; //player 1 wins UI
    public GameObject player2WinPanel; //player 2 wins UI

    void Awake()
    {
        Instance = this;
        player1WinPanel.SetActive(false); //hide both winners screens (will be selectively activated later when game winner is determined)
        player2WinPanel.SetActive(false);
    }

    public void ShowPlayer1Wins()
    {
        player1WinPanel.SetActive(true);
        //show player 1 won
        Time.timeScale = 0f; //freeze game
    }

    public void ShowPlayer2Wins()
    {
        player2WinPanel.SetActive(true);
        //show player 2 won
        Time.timeScale = 0f; //freeze game
    }
}