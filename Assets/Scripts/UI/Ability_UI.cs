using UnityEngine;
using TMPro;

public class Ability_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_Name = default;
    [SerializeField] private TextMeshProUGUI text_Description = default;

    [SerializeField] private GameObject gameObject_Outcomes = default;
    [SerializeField] private TextMeshProUGUI text_Outcome_1 = default;
    [SerializeField] private TextMeshProUGUI text_Outcome_2 = default;
    [SerializeField] private TextMeshProUGUI text_Outcome_3 = default;

    public void Setup(Ability ability)
    {
        text_Name.text = ability.Name;
        text_Description.text = ability.Description;

        gameObject_Outcomes.SetActive(ability.HasOutcomes);

        text_Outcome_1.text = ability.Outcome1;
        text_Outcome_2.text = ability.Outcome2;
        text_Outcome_3.text = ability.Outcome3;
    }
}
