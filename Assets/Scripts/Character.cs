using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public bool IsPlayer;
    public bool CustomInitiative;

    public string Name;
    public Sprite Image;

    public int HealthPoints;
}
