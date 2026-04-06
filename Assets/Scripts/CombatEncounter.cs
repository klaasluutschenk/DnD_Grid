using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CombatEncounter", menuName = "Scriptable Objects/CombatEncounter")]
public class CombatEncounter : ScriptableObject
{
    public Sprite Background;
    public int BackgroundSizeModifier = 1;
    public GameObject PhysicsObject;
    public GameObject FogObject;

    public List<CombatEncounter_Character> Characters;
    public List<Character> SpawnableCharacters;
    public List<CombatEncounter_Entity> WorldEntities;
}
