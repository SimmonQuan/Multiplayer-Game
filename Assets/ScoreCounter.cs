using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ScoreCounter : NetworkBehaviour
{
    [Header("Dynamic")]
    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    //score is readonly for all, but only server can edit the score value

    private TextMeshProUGUI uiText;

    public override void OnNetworkSpawn()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        score.OnValueChanged += OnScoreChanged;
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