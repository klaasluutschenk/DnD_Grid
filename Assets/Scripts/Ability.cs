using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string Name;

    [TextArea] public string Description;

    public bool HasOutcomes;

    [TextArea] public string Outcome1;
    [TextArea] public string Outcome2;
    [TextArea] public string Outcome3;
}
