using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbility_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI abilityName = default;
    [SerializeField] private TextMeshProUGUI abilityDescription = default;
    [Space]
    [SerializeField] private GameObject outcome1 = default;
    [SerializeField] private TextMeshProUGUI outcome1Roll = default;
    [SerializeField] private TextMeshProUGUI outcome1Description = default;
    [Space]
    [SerializeField] private GameObject outcome2 = default;
    [SerializeField] private TextMeshProUGUI outcome2Roll = default;
    [SerializeField] private TextMeshProUGUI outcome2Description = default;
    [Space]
    [SerializeField] private GameObject outcome3 = default;
    [SerializeField] private TextMeshProUGUI outcome3Roll = default;
    [SerializeField] private TextMeshProUGUI outcome3Description = default;

    public void Setup(Ability ability)
    {
        abilityName.text = ability.Name;

        abilityDescription.gameObject.SetActive(ability.HasDescription);
        abilityDescription.text = ability.Description;

        outcome1.SetActive(ability.HasOutcome1);
        outcome1Roll.text = GetOutcomeText(ability.Outcome1Roll);
        outcome1Description.text = ability.Outcome1Description;

        outcome2.SetActive(ability.HasOutcome2);
        outcome2Roll.text = GetOutcomeText(ability.Outcome2Roll);
        outcome2Description.text = ability.Outcome2Description;

        outcome3.SetActive(ability.HasOutcome3);
        outcome3Roll.text = GetOutcomeText(ability.Outcome3Roll);
        outcome3Description.text = ability.Outcome3Description;
    }

    private string GetOutcomeText(OutcomeRoll outcomeRoll)
    {
        switch (outcomeRoll)
        {
            case OutcomeRoll.Tier1:
                return "1 - 9";
            case OutcomeRoll.Tier2:
                return "10 - 16";
            case OutcomeRoll.Tier3:
                return "17+";
            case OutcomeRoll.Action:
                return "Action";
            case OutcomeRoll.InstantAction:
                return "Instant Action";
            case OutcomeRoll.BonusAction:
                return "Bonus Action";
        }

        return "";
    }
}
