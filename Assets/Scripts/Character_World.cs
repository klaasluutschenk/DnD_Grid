using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    private Character character;
    private Tile tile;

    private int health;

    private void Awake()
    {
        OnSpawned?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnDeSpawned?.Invoke(this);
    }

    public void Setup(Character character)
    {
        this.character = character;

        SetInitativeSelection(false);

        SetHP(character.HealthPoints);

        image_CharacterSprite.enabled = !character.IsPlayer;
        text_CharacterHP.enabled = character.IsPlayer;
    }

    private void SetInitativeSelection(bool isActive)
    {
        gameObject_InitiativeSelection.SetActive(isActive);
    }

    private void OnInitiativeSet()
    {
        image_Initiative.color = Color.blue;
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

        image_HP.fillAmount = health / character.HealthPoints;
        text_CharacterHP.text = health.ToString();
    }
}
