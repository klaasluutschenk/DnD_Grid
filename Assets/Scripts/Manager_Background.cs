using UnityEngine;

public class Manager_Background : MonoBehaviour
{
    public static Manager_Background Instance;

    [SerializeField] private SpriteRenderer spriteRenderer = default;

    private void Awake()
    {
        Instance = this;

        Manager_Combat.OnCombatEncounterEnded += OnCombatEncounterEnded;
        Manager_Combat.OnCombatEncounterLoaded += OnCombatEncounterLoaded;
    }

    private void OnCombatEncounterEnded()
    {
        spriteRenderer.sprite = null;
        spriteRenderer.enabled = false;
    }

    private void OnCombatEncounterLoaded (CombatEncounter combatEncounter)
    {
        spriteRenderer.sprite = combatEncounter.Background;
        spriteRenderer.enabled = true;
    }
}
