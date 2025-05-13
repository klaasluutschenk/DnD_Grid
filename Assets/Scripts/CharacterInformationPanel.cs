using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CharacterInformationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_Name = default;
    [SerializeField] private TextMeshProUGUI text_Description = default;

    [SerializeField] private TextMeshProUGUI text_HealthPoints = default;
    [SerializeField] private TextMeshProUGUI text_Armor = default;
    [SerializeField] private TextMeshProUGUI text_Movement = default;
    [SerializeField] private TextMeshProUGUI text_Perception = default;
    [SerializeField] private TextMeshProUGUI text_Dodge = default;

    private void Awake()
    {
        Manager_Initative.OnInitiativeSelectionUpdated += OnInitiativeSelectionUpdated;
    }

    private void OnInitiativeSelectionUpdated(character_Initative character_Initative)
    {
        SetupCharacter(character_Initative.Character);
    }

    private void SetupCharacter(Character character)
    {
        text_Name.text = character.Name;
        text_Description.text = character.Description;

        text_HealthPoints.text = character.HealthPoints.ToString();
        text_Armor.text = character.Armor.ToString();
        text_Movement.text = character.Movement.ToString();
        text_Perception.text = character.Perception.ToString();
        text_Dodge.text = character.Dodge.ToString();
    }
}
