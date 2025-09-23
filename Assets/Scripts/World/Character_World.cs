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

    public Stat Health = new Stat(StatType.Health, 0);
    public Stat Armor = new Stat(StatType.Armor, 0);
    public Stat Barrier = new Stat(StatType.Barrier, 0);

    public Stat Burning = new Stat(StatType.Burning, 0);
    public Stat Poisoned = new Stat(StatType.Poisoned, 0);
    public Stat Bleeding = new Stat(StatType.Bleeding, 0);

    public Stat Slowed = new Stat(StatType.Slowed, 0);
    public Stat Stunned = new Stat(StatType.Stunned, 0);
    public Stat Blinded = new Stat(StatType.Blinded, 0);

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
        if (!Burning.StatActive)
            return;

        Damage(Burning.StatValue);
    }

    private void HandleVenom()
    {
        if (!Poisoned.StatActive)
            return;

        Damage(Poisoned.StatValue);
    }

    private void HandleBleed()
    {
        if (!Bleeding.StatActive)
            return;

        Damage(Bleeding.StatValue);
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
        SetHP(Health.StatValue - damage);
    }

    public void Heal(int heal)
    {
        int trueHeal = heal;

        if (Bleeding.StatValue > 0)
        {
            if (heal >= Bleeding.StatValue)
            {
                trueHeal = heal - Bleeding.StatValue;

                Bleeding.StatValue = 0;

                ParticleSystem.EmissionModule emission = particleSystem_Bleed.emission;
                emission.enabled = false;
            }
            else
            {
                trueHeal = 0;
                Bleeding.StatValue = Bleeding.StatValue - heal;
            }
        }

        SetHP(Health.StatValue + trueHeal);
    }

    private void SetHP(int value)
    {
        if (value < 0)
            value = 0;

        if (value > character.HealthPoints)
            value = character.HealthPoints;

        if (value == Health.StatValue)
            return;

        Health.StatValue = value;

        if (Health.StatValue == 0)
        {
            Kill();
            return;
        }

        float healthPercentage = (float)Health.StatValue / (float)character.HealthPoints;

        image_HP.fillAmount = healthPercentage;
        SetHealthColor(healthPercentage);

        text_CharacterHP.text = Health.StatValue.ToString();
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
        Burning.StatValue = apply ? Burning.StatValue + 1 : Burning.StatValue - 1;

        ParticleSystem.EmissionModule emission = particleSystem_Burn.emission;
        emission.enabled = Burning.StatActive;
    }

    public void ApplyVenom(bool apply = true)
    {
        Poisoned.StatValue = apply ? Poisoned.StatValue + 1 : Poisoned.StatValue - 1;

        ParticleSystem.EmissionModule emission = particleSystem_Venom.emission;
        emission.enabled = Poisoned.StatActive;
    }

    public void ApplyBleed(bool apply = true)
    {
        Bleeding.StatValue = apply ? Bleeding.StatValue + 1 : Bleeding.StatValue - 1;

        ParticleSystem.EmissionModule emission = particleSystem_Bleed.emission;
        emission.enabled = Bleeding.StatActive;
    }

    public void CleanseBurn()
    {
        if (!Burning.StatActive)
            return;

        ApplyBurn(false);
    }
    
    public void CleanseVenom()
    {
        if (!Poisoned.StatActive)
            return;

        ApplyVenom(false);
    }
}

public class Stat
{
    public StatType StatType;
    public int StatValue;

    public bool StatActive => StatValue > 0;

    public Stat (StatType statType, int statValue)
    {
        StatType = statType;
        StatValue = statValue;
    }
}

public enum StatType
{
    Health = 0,
    Armor = 1,
    Barrier = 2,
    Burning = 3,
    Poisoned = 4,
    Bleeding = 5,
    Slowed = 6,
    Stunned = 7,
    Blinded = 8
}