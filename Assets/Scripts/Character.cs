using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public bool IsPlayer;
    public bool CustomInitiative;

    public bool IsPlayerTeam;

    public string Name;
    public Color InitativeColor;
    public Sprite Sprite;

    public int initiative;

    public int HealthPoints;
    public int Movement;
}
