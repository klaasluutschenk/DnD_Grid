using UnityEngine;
using System.Collections;

public class Manager_Encounter : MonoBehaviour
{
    public static Manager_Encounter Instance;

    public CombatEncounter CombatEncounter => activeCombatEncounter;

    [SerializeField] private CombatEncounter combatEncounterToLoad = default;

    private CombatEncounter activeCombatEncounter;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine Initialize()
    {
        return StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        activeCombatEncounter = combatEncounterToLoad;

        yield return null;

        Debug.Log("Encounter Loaded");
    }
}
