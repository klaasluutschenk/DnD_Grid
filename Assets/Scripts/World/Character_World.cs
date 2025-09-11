using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

public class Character_World : World_Entity
{
    public Character Character => character;

    [SerializeField] private GameObject gameObject_InitiativeSelection;
    [SerializeField] private Image image_HP;
    [SerializeField] private Image image_Initiative;
    [SerializeField] private TextMeshProUGUI text_CharacterHP;

    [SerializeField] private Color color_Healthy = default;
    [SerializeField] private Color color_Wounded = default;
    [SerializeField] private Color color_Dying = default;

    [SerializeField] private ParticleSystem particleSystem_Burn = default;
    [SerializeField] private ParticleSystem particleSystem_Venom = default;
    [SerializeField] private ParticleSystem particleSystem_Bleed = default;

    private Character character;

    private int health;

    private int burnValue;
    private int venomValue;
    private int bleedValue;

    protected override void Awake()
    {
        base.Awake();

        Manager_Initative.OnInitiativeOrderUpdated += OnInitiativeOrderUpdated;
        Manager_Initative.OnInitiativeSelectionUpdated += OnInitiativeSelectionUpdated;
    }

    protected override void OnDestroy()
    {
        Manager_Initative.OnInitiativeOrderUpdated -= OnInitiativeOrderUpdated;
        Manager_Initative.OnInitiativeSelectionUpdated -= OnInitiativeSelectionUpdated;

        base.OnDestroy();
    }

    public override void Reveal()
    {
        base.Reveal();

        if (!isRevealed)
            return;

        Manager_Initative.Instance.InjectNewCharacter(character);
    }

    private void OnInitiativeOrderUpdated(List<character_Initative> initiativeOrder)
    {
        character_Initative myCharacter = initiativeOrder.Where(c => c.Character.Name == character.Name).FirstOrDefault();

        if (myCharacter == null)
            return;

        SetInitativeColor(myCharacter);
    }

    private void OnInitiativeSelectionUpdated(character_Initative character_Initative)
    {
        SetInitativeSelection(character_Initative.Character.Name == character.Name);
    }

    public override void Setup(Entity entity)
    {
        Character character = entity as Character;

        SetupCharacter(character);

        base.Setup(entity);
    }

    private void SetupCharacter(Character character)
    {
        if (character == null)
            return;

        this.character = character;

        SetInitativeSelection(false);

        SetInitativeColor(Manager_Initative.Instance.GetInitiativeCharacter(character.Name));

        SetHP(character.HealthPoints);
        text_CharacterHP.enabled = character.IsPlayer;
    }

    private void SetInitativeSelection(bool isActive)
    {
        gameObject_InitiativeSelection.SetActive(isActive);

        if (isActive)
        {
            StartTurn();
        }
    }

    private void StartTurn()
    {
        HandleBurn();
        HandleVenom();
        HandleBleed();
    }

    private void HandleBurn()
    {
        if (burnValue <= 0)
            return;

        Damage(burnValue);
    }

    private void HandleVenom()
    {
        if (venomValue <= 0)
            return;

        Damage(venomValue);
    }

    private void HandleBleed()
    {
        if (bleedValue <= 0)
            return;

        Damage(bleedValue);
        ApplyBleed(false);
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
        int trueHeal = heal;

        if (bleedValue > 0)
        {
            if (heal >= bleedValue)
            {
                trueHeal = heal - bleedValue;

                bleedValue = 0;

                ParticleSystem.EmissionModule emission = particleSystem_Bleed.emission;
                emission.enabled = false;
            }
            else
            {
                trueHeal = 0;
                bleedValue = bleedValue - heal;
            }
        }

        SetHP(health + trueHeal);
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
        Remove();
    }

    public void ApplyBurn(bool apply = true)
    {
        burnValue = apply ? burnValue + 1 : burnValue - 1;

        ParticleSystem.EmissionModule emission = particleSystem_Burn.emission;
        emission.enabled = burnValue > 0;
    }

    public void ApplyVenom(bool apply = true)
    {
        venomValue = apply ? venomValue + 1 : venomValue - 1;

        ParticleSystem.EmissionModule emission = particleSystem_Venom.emission;
        emission.enabled = venomValue > 0;
    }

    public void ApplyBleed(bool apply = true)
    {
        bleedValue = apply ? bleedValue + 1 : bleedValue - 1;

        ParticleSystem.EmissionModule emission = particleSystem_Bleed.emission;
        emission.enabled = bleedValue > 0;
    }

    public void CleanseBurn()
    {
        if (burnValue <= 0)
            return;

        ApplyBurn(false);
    }
    
    public void CleanseVenom()
    {
        if (venomValue <= 0)
            return;

        ApplyVenom(false);
    }
}