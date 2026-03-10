using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{
    static private TextMeshProUGUI _UI_TEXT;
    static private int _SCORE = 1000;
    private TextMeshProUGUI txtCom;

    void Awake()
    {
        _UI_TEXT = this.GetComponent<TextMeshProUGUI>();
        //if the playerpref highscore already exists, read it
        if (PlayerPrefs.HasKey("HighScore"))
        {
            SCORE = PlayerPrefs.GetInt("HighScore");
        }
       //assign the high score to highscore
       PlayerPrefs.SetInt("HighScore", SCORE);
       
    }
    static public int SCORE
    {
        get { return _SCORE; }
        private set
        {
            _SCORE = value;
            PlayerPrefs.SetInt("HighScore", value);
            if (_UI_TEXT != null)
            {
                _UI_TEXT.text = "High Score: " + value.ToString("#,0");
            }
        }
    }

    static public void TRY_SET_HIGH_SCORE(int scoreToTry)
    {
        if (scoreToTry <= SCORE) return;
        
            SCORE = scoreToTry;
        
    }
    [Tooltip("Check this box to reset the HighScore in PlayerPrefs")]
    public bool resetHighScoreNow = false;

    void OnDrawGizmos()
    {
        if (resetHighScoreNow)
        {
            resetHighScoreNow = false;
            PlayerPrefs.SetInt("HighScore", 1000);
            Debug.LogWarning("PlayerPrefs HighScore reset to 1,000");
        }
    }

    void OnScoreChanged(int previous, int current)
    {
        _UI_TEXT.text += current.ToString("#,0");
        HighScore.TRY_SET_HIGH_SCORE(current);
    }
}
