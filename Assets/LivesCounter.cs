using UnityEngine;
using TMPro;
using Unity.Netcode;

public class LivesCounter : MonoBehaviour
{
    private TextMeshProUGUI uiText;
    private ApplePicker applePicker;

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        uiText.text = "Lives: 3"; 
    }

    void Update()
    {
        if (applePicker == null)
        {
            GameObject basketGO = GameObject.FindGameObjectWithTag("Basket");
            if (basketGO != null)
            {
                applePicker = basketGO.GetComponent<ApplePicker>();
                applePicker.lives.OnValueChanged += OnLivesChanged; //when lives changes, OnlivesChanged is called
                uiText.text = "Lives: " + applePicker.lives.Value;
            }
        }
    }

    void OnLivesChanged(int previous, int current)
    {
        uiText.text = "Lives: " + current; //update the text to display updated life count for player
    }
}