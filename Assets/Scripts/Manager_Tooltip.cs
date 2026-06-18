using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Manager_Tooltip : MonoBehaviour
{
    public static Manager_Tooltip Instance;

    public static float TooltipTimer = 2;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipContainer = default;

    [Header("Tile")]
    [SerializeField] private GameObject tile_Container = default;
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
    [SerializeField] private GameObject character_Reload = default;
    [SerializeField] private TextMeshProUGUI character_Reload_Value = default;
    [SerializeField] private GameObject character_Rooted = default;
    [SerializeField] private TextMeshProUGUI character_Rooted_Value = default;

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
        ClearTooltip();

        Debug.Log("Initiative Loaded");

        yield return null;
    }

    public void SetToolTip(Tile tile)
    {
        ClearTooltip();

        tooltipContainer.SetActive(true);

        SetTileTooltip(tile);
    }

    public void ClearTooltip()
    {
        tooltipContainer.SetActive(false);
        tile_Container.SetActive(false);
        character_Container.SetActive(false);
    }

    private void SetTileTooltip(Tile tile)
    {
        tile_Container.SetActive(true);

        Character_World character = null;

        if (tile.World_Entity != null)
        {
            if (tile.World_Entity is Character_World)
                character = tile.World_Entity as Character_World;
        }

        if (character == null)
            character_Container.SetActive(false);
        else
            SetCharacterTooltip(character);
    }

    private void SetCharacterTooltip(Character_World character)
    {
        character_Container.SetActive(true);

        character_Name.text = character.Character.Name;

        character_Health_Value.text = character.Health.StatValue.ToString();

        character_Armor.SetActive(character.Armor.StatActive);
        character_Armor_Value.text = character.Armor.StatValue.ToString();

        character_Barrier.SetActive(character.Barrier.StatActive);
        character_Barrier_Value.text = character.Barrier.StatValue.ToString();

        character_Burning.SetActive(character.Burning.StatActive);
        character_Burning_Value.text = character.Burning.StatValue.ToString();

        character_Poisoned.SetActive(character.Poisoned.StatActive);
        character_Poisoned_Value.text = character.Poisoned.StatValue.ToString();

        character_Bleeding.SetActive(character.Bleeding.StatActive);
        character_Bleeding_Value.text = character.Bleeding.StatValue.ToString();

        character_Slowed.SetActive(character.Slowed.StatActive);
        character_Slowed_Value.text = character.Slowed.StatValue.ToString();

        character_Stunned.SetActive(character.Stunned.StatActive);
        character_Stunned_Value.text = character.Stunned.StatValue.ToString();

        character_Blinded.SetActive(character.Blinded.StatActive);
        character_Blinded_Value.text = character.Blinded.StatValue.ToString();

        character_Reload.SetActive(character.Reloading.StatActive);
        character_Reload_Value.text = character.Reloading.StatValue.ToString();

        character_Rooted.SetActive(character.Rooted.StatActive);
        character_Rooted_Value.text = character.Rooted.StatValue.ToString();
    }
}
