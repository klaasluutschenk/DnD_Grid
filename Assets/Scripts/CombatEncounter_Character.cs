using UnityEngine;

[System.Serializable]
public class CombatEncounter_Character
{
    public Character Character;
    public Vector3 Position;

    public CombatEncounter_Character(Character character, Vector3 position)
    {
        Character = character;
        Position = position;
    }
}

[System.Serializable]
public class CombatEncounter_Entity
{
    public Entity Entity;
    public Vector3 Position;

    public CombatEncounter_Entity(Entity entity, Vector3 position)
    {
        Entity = entity;
        Position = position;
    }
}
