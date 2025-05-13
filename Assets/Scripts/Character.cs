using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public bool IsPlayer;
    public bool CustomInitiative;

    public bool IsPlayerTeam;

    public string Name;

    [TextArea]
    public string Description;

    public Color InitativeColor;
    public Sprite Sprite;

    public int initiative;

    public int HealthPoints;
    public int Armor;
    public int Movement;
    public int Perception;
    public int Dodge;


}
