using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

        activeCombatEncounter = combatEncounter;
        OnCombatEncounterLoaded?.Invoke(activeCombatEncounter);

        yield return null;

        OnCombatEncounterStarted?.Invoke();
    }
}
