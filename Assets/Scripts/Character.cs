using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public bool IsPlayer;
    public bool CustomInitiative;

    public string Name;
    public Color InitativeColor;
    public Sprite Image;

    public int initiative;

    public int HealthPoints;
}
