using UnityEngine;
using System;
using System.Collections.Generic;

public class Manager_Characters : MonoBehaviour
{
    public static Manager_Characters Instance;

    [SerializeField] private Character_World character_World_Prefab = default;
    [SerializeField] private Character_World character_World_Minion_Prefab = default;
    [SerializeField] private Character_World character_World_Big_Prefab = default;
    [SerializeField] private Character_World character_World_Giant_Prefab = default;
    [SerializeField] private World_Entity worldEntity_Prefab = default;
    [SerializeField] private Transform characterContainer = default;

    private List<Character> activeCharacters = new List<Character>();
    private List<Character_World> activeWorldCharacters = new List<Character_World>();
    private List<Character_World> hiddenWorldCharacters = new List<Character_World>();
    private List<World_Entity> worldEntities = new List<World_Entity>();

    private void Awake()
    {
        Instance = this;

        Manager_Combat.OnCombatEncounterEnded += OnCombatEncounterEnded;
        Manager_Combat.OnCombatEncounterLoaded += OnCombatEncounterLoaded;

        World_Entity.OnDeSpawned += OnDeSpawned;
    }

    private void OnCombatEncounterEnded()
    {
        activeCharacters.Clear();

        activeWorldCharacters.ForEach(ac => Destroy(ac.gameObject));
        activeWorldCharacters.Clear();
    }

    private void OnCombatEncounterLoaded(CombatEncounter combatEncounter)
    {
        List<CombatEncounter_Character> combatEncounter_Characters = combatEncounter.Characters;

        foreach (CombatEncounter_Character cc in combatEncounter_Characters)
        {
            SpawnCharacter(cc.Character, cc.Position);
        }

        List<CombatEncounter_Entity> combatEncounter_Entities = combatEncounter.WorldEntities;

        foreach (CombatEncounter_Entity entity in combatEncounter_Entities)
        {
            SpawnEntity(entity.Entity, entity.Position);
        }
    }

    public void SpawnCharacter(Character character, Vector3 position)
    {
        Character_World newWorldCharacter = null;

        if (character.IsMinion)
            newWorldCharacter = Instantiate(character_World_Minion_Prefab, characterContainer);
        else if(character.IsBig)
            newWorldCharacter = Instantiate(character_World_Big_Prefab, characterContainer);
        else if(character.IsGiant)
            newWorldCharacter = Instantiate(character_World_Giant_Prefab, characterContainer);
        else
            newWorldCharacter = Instantiate(character_World_Prefab, characterContainer);

        newWorldCharacter.gameObject.name = character.Name;

        newWorldCharacter.transform.position = position;

        newWorldCharacter.Setup(character);

        //if (newWorldCharacter.Tile.IsRevealed)
        //{
        //    activeWorldCharacters.Add(newWorldCharacter);
        //    Manager_Initative.Instance.AddToInitiative(character);
        //}
        //else
        //    hiddenWorldCharacters.Add(newWorldCharacter);
    }

    public void RevealCharacter(Character_World character)
    {
        if (!hiddenWorldCharacters.Contains(character))
            return;

        hiddenWorldCharacters.Remove(character);
        activeWorldCharacters.Add(character);

        Manager_Initative.Instance.AddToInitiative(character.Character);
    }

    public void SpawnEntity(Entity entity, Vector3 position)
    {
        World_Entity newEntity = Instantiate(worldEntity_Prefab, characterContainer);

        newEntity.gameObject.name = entity.Name;

        newEntity.transform.position = position;

        newEntity.Setup(entity);

        worldEntities.Add(newEntity);
    }

    private void OnDeSpawned(World_Entity world_Entity)
    {
        Character_World character_World = world_Entity as Character_World;

        if (character_World == null)
            return;

        if (!activeWorldCharacters.Contains(character_World))
            return;

        activeWorldCharacters.Remove(character_World);
    }
}
