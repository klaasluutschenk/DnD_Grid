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

    [SerializeField] private Transform abilityContainer = default;
    [SerializeField] private PlayerAbility_UI abilityPrefab = default;

    private List<PlayerAbility_UI> activeUIs = new List<PlayerAbility_UI>();

    private void Awake()
    {
        Manager_Initative.OnCharacterTurnStart += OnCharacterTurnStart;
    }

    private void OnCharacterTurnStart(Character_Initiative character_Initative)
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

        activeUIs.ForEach(aUI => Destroy(aUI.gameObject));
        activeUIs.Clear();

        foreach (Ability ability in character.Abilities)
        {
            PlayerAbility_UI newUI = Instantiate(abilityPrefab, abilityContainer);

            newUI.Setup(ability);

            activeUIs.Add(newUI);
        }

        StartCoroutine(RefreshContainer());
    }

    // Very Hacky solution to circumvent the dynamic size breaking.
    private IEnumerator RefreshContainer()
    {
        abilityContainer.gameObject.SetActive(false);

        yield return null;

        abilityContainer.gameObject.SetActive(true);

        yield return null;

        abilityContainer.gameObject.SetActive(false);

        yield return null;

        abilityContainer.gameObject.SetActive(true);

        yield return null;

        abilityContainer.gameObject.SetActive(false);

        yield return null;

        abilityContainer.gameObject.SetActive(true);
    }

}
