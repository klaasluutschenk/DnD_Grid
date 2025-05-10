using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CustomInitiativeUI : MonoBehaviour
{
    public static Action<character_Initative> OnCharacterSet;
    
    public Action<CustomInitiativeUI> OnRemove;

    [SerializeField] private TextMeshProUGUI characterName = default;
    [SerializeField] private Image characterSprite = default;

    [SerializeField] private InitiativeButton buttonPrefab = default;
    [SerializeField] private Transform buttonContainer = default;

    private character_Initative character;

    public void Setup(character_Initative character_Initative)
    {
        character = character_Initative;

        characterName.text = character.Name;
        characterSprite.sprite = character.Sprite;

        SetupButtons();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < 25; i++)
        {
            InitiativeButton newButton = Instantiate(buttonPrefab, buttonContainer);

            newButton.Setup(i + 1);
            newButton.OnClicked += OnButtonClicked;
        }
    }

    private void OnButtonClicked(int index)
    {
        character.Initiative = index;
        OnCharacterSet?.Invoke(character);
        OnRemove?.Invoke(this);
    }
}
