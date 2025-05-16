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
    [SerializeField] private ParticleSystem particleSystem_Poison = default;
    [SerializeField] private ParticleSystem particleSystem_Bleed = default;
    [SerializeField] private ParticleSystem particleSystem_Heal = default;

    private Character character;

    private int health;

    private List<StatusEffect> statusEffects = new List<StatusEffect>();

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
        HandleStatusEffects();
    }

    private void HandleStatusEffects()
    {
        StatusEffect heal = GetStatusEffect(StatusEffectType.Heal);
        if (heal != null)
        {
            Heal(heal.Value);

            if (!heal.indefinite)
            {
                heal.Duration--;

                if (heal.Duration <= 0)
                    CleanseStatusEffect(StatusEffectType.Heal);
            }
        }

        StatusEffect burn = GetStatusEffect(StatusEffectType.Burn);
        if (burn != null)
        {
            Damage(burn.Value);

            if (!burn.indefinite)
            {
                burn.Duration--;

                if (burn.Duration <= 0)
                    CleanseStatusEffect(StatusEffectType.Burn);
            }
        }

        StatusEffect poison = GetStatusEffect(StatusEffectType.Poison);
        if (poison != null)
        {
            Damage(poison.Value);

            if (!poison.indefinite)
            {
                poison.Duration--;

                if (poison.Duration <= 0)
                    CleanseStatusEffect(StatusEffectType.Poison);
            }
        }

        StatusEffect bleed = GetStatusEffect(StatusEffectType.Bleed);
        if (bleed != null)
        {
            Damage(bleed.Value);

            if (!bleed.indefinite)
            {
                bleed.Duration--;

                if (bleed.Duration <= 0)
                    CleanseStatusEffect(StatusEffectType.Bleed);
            }
        }
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
        Remove();
    }

    public void ApplyStatusEffect(int value, int duration, StatusEffectType statusEffectType)
    {
        CleanseStatusEffect(statusEffectType);

        statusEffects.Add(new StatusEffect(value, duration, statusEffectType));

        ToggleStatusEffectParticles(statusEffectType, true);
    }

    private void CleanseStatusEffect(StatusEffectType statusEffectType)
    {
        if (!HasStatusEffect(statusEffectType))
            return;

        StatusEffect currentStatusEffect = GetStatusEffect(statusEffectType);

        statusEffects.Remove(currentStatusEffect);

        ToggleStatusEffectParticles(statusEffectType, false);
    }

    private bool HasStatusEffect(StatusEffectType statusEffectType)
    {
        return statusEffects.Where(se => se.StatusEffectType == statusEffectType).ToList().Count > 0;
    }

    private StatusEffect GetStatusEffect(StatusEffectType statusEffectType)
    {
        return statusEffects.Where(se => se.StatusEffectType == statusEffectType).FirstOrDefault();
    }

    private void ToggleStatusEffectParticles(StatusEffectType statusEffectType, bool enable)
    {
        ParticleSystem.EmissionModule emission;

        switch (statusEffectType)
        {
            case StatusEffectType.None:
                return;
            case StatusEffectType.Burn:
                emission = particleSystem_Burn.emission;
                break;
            case StatusEffectType.Poison:
                emission = particleSystem_Poison.emission;
                break;
            case StatusEffectType.Bleed:
                emission = particleSystem_Bleed.emission;
                break;
            case StatusEffectType.Heal:
                emission = particleSystem_Heal.emission;
                break;
            default:
                break;
        }

        emission.enabled = enable;
    }
}

public class StatusEffect
{
    public int Value;
    public int Duration;
    public StatusEffectType StatusEffectType;

    public bool indefinite;

    public StatusEffect(int value, int duration, StatusEffectType statusEffectType)
    {
        Value = value;
        Duration = duration;
        StatusEffectType = statusEffectType;

        indefinite = duration == 0 ? true : false;
    }
}

public enum StatusEffectType
{
    None = 0,
    Burn = 1,
    Poison = 2,
    Bleed = 3,
    Heal = 4
}