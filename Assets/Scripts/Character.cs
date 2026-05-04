using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : Entity
{
    public bool IsPlayer;
    public bool CustomInitiative;
    public bool IsMinion;

    public bool IsPlayerTeam;

    public Color InitativeColor;
    public Color CombatPrepColor;

    public bool HasNoInitiative;
    public int initiative;
    public List<int> AdditionalInitiatives;


    [Header("Base Stats")]
    public int HealthPoints;
    public int Armor;
    public int Barier;
    public int Movement;
    public int Perception;
    public int Dodge;

    [Header("Base Effects")]
    public int Bleed;
    public int Blinded;
    public int Burn;
    public int Reload;
    public int Root;
    public int Slowed;
    public int Stun;
    public int Venom;

    public List<Ability> Abilities;

    [Header("Testing")]
    public bool HasShip;
}
