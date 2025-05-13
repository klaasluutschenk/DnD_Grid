using UnityEngine;
using System;
using System.Collections.Generic;

public class Manager_Characters : MonoBehaviour
{
    public static Manager_Characters Instance;

    [SerializeField] private Character_World character_World_Prefab = default;
    [SerializeField] private Transform characterContainer = default;

    private List<Character> activeCharacters = new List<Character>();
    private List<Character_World> activeWorldCharacters = new List<Character_World>();

    private void Awake()
    {
        Instance = this;

        Manager_Combat.OnCombatEncounterEnded += OnCombatEncounterEnded;
        Manager_Combat.OnCombatEncounterLoaded += OnCombatEncounterLoaded;

        Character_World.OnDeSpawned += OnDeSpawned;
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
    }

    public void SpawnCharacter(Character character, Vector3 position)
    {
        Character_World newWorldCharacter = Instantiate(character_World_Prefab, characterContainer);

        newWorldCharacter.gameObject.name = character.Name;

        newWorldCharacter.transform.position = position;

        newWorldCharacter.Setup(character);

        activeWorldCharacters.Add(newWorldCharacter);

        if (newWorldCharacter.IsRevealed)
            Manager_Initative.Instance.InjectNewCharacter(character);
    }

    private void OnDeSpawned(Character_World character_World)
    {
        if (!activeWorldCharacters.Contains(character_World))
            return;

        activeWorldCharacters.Remove(character_World);
    }
}
