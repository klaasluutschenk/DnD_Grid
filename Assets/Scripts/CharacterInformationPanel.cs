using UnityEngine;
using System.Collections;
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

    [SerializeField] private Ability_UI prefab_Ability = default;
    [SerializeField] private Transform container = default;

    private List<Ability_UI> activeUIs = new List<Ability_UI>();

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

        activeUIs.ForEach(aUI => Destroy(aUI.gameObject));
        activeUIs.Clear();

        foreach (Ability ability in character.Abilities)
        {
            Ability_UI newUI = Instantiate(prefab_Ability, container);

            newUI.Setup(ability);

            activeUIs.Add(newUI);
        }

        StartCoroutine(RefreshContainer());
    }

    private IEnumerator RefreshContainer()
    {
        container.gameObject.SetActive(false);

        yield return null;

        container.gameObject.SetActive(true);

        yield return null;

        container.gameObject.SetActive(false);

        yield return null;

        container.gameObject.SetActive(true);

        yield return null;

        container.gameObject.SetActive(false);

        yield return null;

        container.gameObject.SetActive(true);
    }

}
