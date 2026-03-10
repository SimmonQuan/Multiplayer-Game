using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;

public class HelloWorldManager : MonoBehaviour
{
    VisualElement rootVisualElement;
    Button hostButton;
    Button clientButton;
    TextField ipInputField;
    Label statusLabel;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        rootVisualElement = uiDocument.rootVisualElement;

        hostButton = CreateButton("HostButton", "Host");
        clientButton = CreateButton("ClientButton", "Client");
        ipInputField = CreateTextField("IPInputField", "Enter Host IP...");
        statusLabel = CreateLabel("StatusLabel", "Not Connected");

        rootVisualElement.Clear();
        rootVisualElement.Add(ipInputField);
        rootVisualElement.Add(hostButton);
        rootVisualElement.Add(clientButton);
        rootVisualElement.Add(statusLabel);

        hostButton.clicked += OnHostButtonClicked;
        clientButton.clicked += OnClientButtonClicked;
    }

    void Update()
    {
        UpdateUI();
    }

    void OnDisable()
    {
        hostButton.clicked -= OnHostButtonClicked;
        clientButton.clicked -= OnClientButtonClicked;
    }

    void OnHostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
    }

    void OnClientButtonClicked()
    {
        string ipAddress = ipInputField.value;

        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "Enter Host IP...")
        {
            ipAddress = "127.0.0.1";
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddress, 7777);

        NetworkManager.Singleton.StartClient();
    }

    private Button CreateButton(string name, string text)
    {
        var button = new Button();
        button.name = name;
        button.text = text;
        button.style.width = 240;
        button.style.backgroundColor = Color.white;
        button.style.color = Color.black;
        button.style.unityFontStyleAndWeight = FontStyle.Bold;
        return button;
    }

    private TextField CreateTextField(string name, string placeholder)
    {
        var textField = new TextField();
        textField.name = name;
        textField.value = placeholder;
        textField.style.width = 240;
        textField.style.backgroundColor = Color.white;
        textField.style.color = Color.black;
        return textField;
    }

    private Label CreateLabel(string name, string content)
    {
        var label = new Label();
        label.name = name;
        label.text = content;
        label.style.color = Color.black;
        label.style.fontSize = 18;
        return label;
    }

    void UpdateUI()
    {
        if (NetworkManager.Singleton == null)
        {
            SetStartButtons(false);
            SetStatusText("NetworkManager not found");
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            SetStartButtons(true);
            SetStatusText("Not connected");
        }
        else
        {
            SetStartButtons(false);
            UpdateStatusLabels();
        }
    }

    void SetStartButtons(bool state)
    {
        hostButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        clientButton.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        ipInputField.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void SetStatusText(string text)
    {
        statusLabel.text = text;
    }

    void UpdateStatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : "Client";
        string transport = "Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        string modeText = "Mode: " + mode;
        SetStatusText($"{transport}\n{modeText}");
    }
}