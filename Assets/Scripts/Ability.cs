using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string Name;

    public bool HasDescription;
    [TextArea] public string Description;

    public bool HasOutcome1;
    public OutcomeRoll Outcome1Roll;
    [TextArea] public string Outcome1Description;

    public bool HasOutcome2;
    public OutcomeRoll Outcome2Roll;
    [TextArea] public string Outcome2Description;

    public bool HasOutcome3;
    public OutcomeRoll Outcome3Roll;
    [TextArea] public string Outcome3Description;

}

public enum OutcomeRoll
{
    Tier1,
    Tier2,
    Tier3,
    Action,
    InstantAction,
    BonusAction
}