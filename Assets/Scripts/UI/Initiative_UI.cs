using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Initiative_UI : MonoBehaviour
{
    public character_Initative Character => character;

    [SerializeField] private GameObject gameObject_InitiativeSelection;

    [SerializeField] private Image image_Initiative;
    [SerializeField] private Image image_CharacterSprite;

    [SerializeField] private TextMeshProUGUI textCharacterName;

    private character_Initative character;

    public void Setup(character_Initative character_Initative)
    {
        character = character_Initative;

        image_Initiative.color = character.InitativeColor;
        image_CharacterSprite.sprite = character.Sprite;

        textCharacterName.text = character.Name;
    }

    public void SetInitative(bool isActive)
    {
        gameObject_InitiativeSelection.SetActive(isActive);
    }
}
