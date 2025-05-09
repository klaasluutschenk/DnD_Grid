using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CombatEncounter", menuName = "Scriptable Objects/CombatEncounter")]
public class CombatEncounter : ScriptableObject
{
    public Sprite Background;
    public GameObject PhysicsObject;

    public List<CombatEncounter_Character> Characters;
}
