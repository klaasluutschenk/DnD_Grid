using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Manager_Combat : MonoBehaviour
{
    public static Manager_Combat Instance;

    public static Action OnCombatEncounterEnded;
    public static Action<CombatEncounter> OnGridRequest;
    public static Action<CombatEncounter> OnCombatEncounterLoaded;
    public static Action OnCombatEncounterStarted;

    public CombatEncounter ActiveCombatEncounter => activeCombatEncounter;

    [SerializeField] private CombatEncounter defaultCombatEncounter;

    private CombatEncounter activeCombatEncounter;

    private Coroutine startCombatEcounterRoutine;

    [Header("Combat Prep")]
    [SerializeField] private Transform CharacterContainer;
    [SerializeField] private Transform EntityContainer;
    [SerializeField] private CombatPrepCharacter CombatPrepCharacterPrefab;
    [SerializeField] private CombatPrepEntity CombatPrepEntityPrefab;
    [SerializeField] private SpriteRenderer background;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCombatEncounter(defaultCombatEncounter);
    }

    public void StartCombatEncounter(CombatEncounter combatEncounter)
    {
        if (startCombatEcounterRoutine != null)
            StopCoroutine(startCombatEcounterRoutine);

        startCombatEcounterRoutine = StartCoroutine(StartCombatEcounterRoutine(combatEncounter));

    }

    private IEnumerator StartCombatEcounterRoutine(CombatEncounter combatEncounter)
    {
        OnCombatEncounterEnded?.Invoke();

        yield return null;

        OnGridRequest?.Invoke(combatEncounter);

        yield return null;
        yield return null;
        yield return null;

        activeCombatEncounter = combatEncounter;
        OnCombatEncounterLoaded?.Invoke(activeCombatEncounter);

        yield return null;
        yield return null;
        yield return null;

        OnCombatEncounterStarted?.Invoke();
    }

    public void SaveCombatEncounter()
    {
        // Characters
        List<CombatEncounter_Character> combatEncounterCharacters = new List<CombatEncounter_Character>();

        List<CombatPrepCharacter> availableCharacters = CharacterContainer.GetComponentsInChildren<CombatPrepCharacter>().ToList();
        foreach (CombatPrepCharacter character in availableCharacters)
        {
            combatEncounterCharacters.Add(character.GetCombatPrepCharacterData());
        }

        defaultCombatEncounter.Characters = combatEncounterCharacters;

        // Entities
        List<CombatEncounter_Entity> combatEncounterEntities = new List<CombatEncounter_Entity>();

        List<CombatPrepEntity> availableEntities = EntityContainer.GetComponentsInChildren<CombatPrepEntity>().ToList();
        foreach (CombatPrepEntity entity in availableEntities)
        {
            combatEncounterEntities.Add(entity.GetCombatPrepEntityData());
        }

        defaultCombatEncounter.WorldEntities = combatEncounterEntities;
    }

    public void LoadCombatPrep()
    {
        RemoveCombatPrep();

        background.sprite = defaultCombatEncounter.Background;

        foreach (CombatEncounter_Character character in defaultCombatEncounter.Characters)
        {
            CombatPrepCharacter newCharacter = Instantiate(CombatPrepCharacterPrefab, CharacterContainer);

            newCharacter.transform.position = character.Position;

            newCharacter.SetCharacter(character.Character);
        }

        foreach (CombatEncounter_Entity entity in defaultCombatEncounter.WorldEntities)
        {
            CombatPrepEntity newEntity = Instantiate(CombatPrepEntityPrefab, EntityContainer);

            newEntity.transform.position = entity.Position;

            newEntity.SetEntity(entity.Entity);
        }
    }

    public void RemoveCombatPrep()
    {
        for (int i = CharacterContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(CharacterContainer.GetChild(i).gameObject);
        }

        for (int i = EntityContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(EntityContainer.GetChild(i).gameObject);
        }
    }
}
