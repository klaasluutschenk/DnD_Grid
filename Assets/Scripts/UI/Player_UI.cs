using UnityEngine;
using TMPro;

public class Player_UI : MonoBehaviour
{
    public Character character;

    [SerializeField] private TextMeshProUGUI characterName = default;

    [SerializeField] private TextMeshProUGUI healthValue = default;
    [SerializeField] private TextMeshProUGUI armorValue = default;
    [SerializeField] private TextMeshProUGUI barrierValue = default;
    [SerializeField] private TextMeshProUGUI movementValue = default;

    [SerializeField] private Transform skillContainer = default;
    [SerializeField] private PlayerAbility_UI abilityPrefab = default;

    private void Start()
    {
        characterName.text = character.Name;

        healthValue.text = character.HealthPoints.ToString();
        armorValue.text = character.Armor.ToString();
        barrierValue.text = character.Barier.ToString();
        movementValue.text = character.Movement.ToString();

        foreach (Ability ability in character.Abilities)
        {
            PlayerAbility_UI newAbilityUI = Instantiate(abilityPrefab, skillContainer);

            newAbilityUI.Setup(ability);
        }
    }
}
