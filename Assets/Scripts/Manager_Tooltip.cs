using UnityEngine;
using TMPro;

public class Manager_Tooltip : MonoBehaviour
{
    public static Manager_Tooltip Instance;

    public static float TooltipTimer = 2;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipContainer = default;

    [Header("Tile")]
    [SerializeField] private GameObject tile_Burning = default;
    [SerializeField] private GameObject tile_Freezing = default;

    [Header("Character")]
    [SerializeField] private GameObject character_Container = default;
    [SerializeField] private TextMeshProUGUI character_Name = default;

    [SerializeField] private GameObject character_Health = default;
    [SerializeField] private TextMeshProUGUI character_Health_Value = default;
    [SerializeField] private GameObject character_Armor = default;
    [SerializeField] private TextMeshProUGUI character_Armor_Value = default;
    [SerializeField] private GameObject character_Barrier = default;
    [SerializeField] private TextMeshProUGUI character_Barrier_Value = default;

    [SerializeField] private GameObject character_Burning = default;
    [SerializeField] private TextMeshProUGUI character_Burning_Value = default;
    [SerializeField] private GameObject character_Poisoned = default;
    [SerializeField] private TextMeshProUGUI character_Poisoned_Value = default;
    [SerializeField] private GameObject character_Bleeding = default;
    [SerializeField] private TextMeshProUGUI character_Bleeding_Value = default;
    [SerializeField] private GameObject character_Slowed = default;
    [SerializeField] private TextMeshProUGUI character_Slowed_Value = default;
    [SerializeField] private GameObject character_Stunned = default;
    [SerializeField] private TextMeshProUGUI character_Stunned_Value = default;
    [SerializeField] private GameObject character_Blinded = default;
    [SerializeField] private TextMeshProUGUI character_Blinded_Value = default;

    private void Awake()
    {
        Instance = this;

        Tile.OnTooltipCalled += OnTooltipCalled;
    }

    private void OnTooltipCalled(Tile tile)
    {
        Debug.Log($"Tile: {tile.gameObject.name}, Character: {tile.World_Entity.Entity.Name}.");
    }

    public void SetToolTip(Tile tile)
    {

    }
}
