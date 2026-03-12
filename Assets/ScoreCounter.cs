using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ScoreCounter : NetworkBehaviour
{
    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    //establish score is read only for all, but only server can edit the score value. Ensures score integrity/prevents cheating

    private TextMeshProUGUI uiText;

    public override void OnNetworkSpawn()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        score.OnValueChanged += OnScoreChanged; //when score updates, call OnScoreChanged
        uiText.text = score.Value.ToString("#,0");
    }

    public override void OnNetworkDespawn()
    {
        score.OnValueChanged -= OnScoreChanged;
    }

    void OnScoreChanged(int previous, int current)
    {
        uiText.text = current.ToString("#,0"); //update score tracker
        HighScore.TRY_SET_HIGH_SCORE(current);
    }
}