using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

public class Character_World : MonoBehaviour
{
    public static Action<Character_World> OnSpawned;
    public static Action<Character_World> OnDeSpawned;

    public Character Character => character;
    public Tile Tile => tile;

    [SerializeField] private GameObject gameObject_InitiativeSelection;
    [SerializeField] private Image image_HP;
    [SerializeField] private Image image_Initiative;
    [SerializeField] private Image image_CharacterSprite;
    [SerializeField] private TextMeshProUGUI text_CharacterHP;

    [SerializeField] private Color color_Healthy = default;
    [SerializeField] private Color color_Wounded = default;
    [SerializeField] private Color color_Dying = default;

    private Character character;
    private Tile tile;

    private int health;

    private void Awake()
    {
        OnSpawned?.Invoke(this);

        Manager_Initative.OnInitiativeOrderUpdated += OnInitiativeOrderUpdated;
        Manager_Initative.OnInitiativeSelectionUpdated += OnInitiativeSelectionUpdated;
    }

    private void OnDestroy()
    {
        Manager_Initative.OnInitiativeOrderUpdated -= OnInitiativeOrderUpdated;
        Manager_Initative.OnInitiativeSelectionUpdated -= OnInitiativeSelectionUpdated;

        OnDeSpawned?.Invoke(this);
    }

    private void OnInitiativeOrderUpdated(List<character_Initative> initiativeOrder)
    {
        character_Initative myCharacter = initiativeOrder.Where(c => c.Name == character.Name).FirstOrDefault();

        if (myCharacter == null)
            return;

        SetInitativeColor(myCharacter);
    }

    private void OnInitiativeSelectionUpdated(character_Initative character_Initative)
    {
        SetInitativeSelection(character_Initative.Name == character.Name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Damage(1);
    }

    public void Setup(Character character)
    {
        this.character = character;

        SetInitativeSelection(false);

        tile = Manager_Grid.Instance.GetTileByWorldPosition(transform.position);

        SetInitativeColor(Manager_Initative.Instance.GetInitiativeCharacter(character.name));

        SetHP(character.HealthPoints);

        image_CharacterSprite.enabled = !character.IsPlayer;
        text_CharacterHP.enabled = character.IsPlayer;
    }

    private void SetInitativeSelection(bool isActive)
    {
        gameObject_InitiativeSelection.SetActive(isActive);
    }

    private void SetInitativeColor(character_Initative character_Initative)
    {
        if (character_Initative == null)
            return;

        image_Initiative.color = character_Initative.InitativeColor;
    }

    public void Damage(int damage)
    {
        SetHP(health - damage);
    }

    public void Heal(int heal)
    {
        SetHP(health + heal);
    }

    private void SetHP(int value)
    {
        if (value < 0)
            value = 0;

        if (value > character.HealthPoints)
            value = character.HealthPoints;

        if (value == health)
            return;

        health = value;

        if (health == 0)
        {
            Kill();
            return;
        }

        float healthPercentage = (float)health / (float)character.HealthPoints;

        image_HP.fillAmount = healthPercentage;
        SetHealthColor(healthPercentage);

        text_CharacterHP.text = health.ToString();
    }

    private void SetHealthColor(float percentage)
    {
        if (percentage > 0.5f)
            image_HP.color = color_Healthy;
        else if (percentage > 0.25f)
            image_HP.color = color_Wounded;
        else
            image_HP.color = color_Dying;
    }

    public void Kill()
    {
        OnDeSpawned?.Invoke(this);
        tile.ClearCharacter();

        Destroy(this.gameObject);
    }
}
