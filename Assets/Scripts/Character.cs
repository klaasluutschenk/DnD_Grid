using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : Entity
{
    public bool IsPlayer;
    public bool CustomInitiative;

    public bool IsPlayerTeam;

    public Color InitativeColor;

    public int initiative;

    public int HealthPoints;
    public int Armor;
    public int Movement;
    public int Perception;
    public int Dodge;

    public List<Ability> Abilities;
}
