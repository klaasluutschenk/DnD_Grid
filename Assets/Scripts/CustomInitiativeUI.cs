using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CustomInitiativeUI : MonoBehaviour
{
    public static Action<Character_Initiative_Custom> OnCharacterSet;
    
    public Action<CustomInitiativeUI> OnRemove;

    [SerializeField] private TextMeshProUGUI characterName = default;
    [SerializeField] private Image characterSprite = default;

    [SerializeField] private InitiativeButton buttonPrefab = default;
    [SerializeField] private Transform buttonContainer = default;

    private Character character;

    public void Setup(Character character)
    {
        this.character = character;

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
        Character_Initiative_Custom customInitative = new Character_Initiative_Custom(character, index);
        OnCharacterSet?.Invoke(customInitative);
        OnRemove?.Invoke(this);
    }
}
