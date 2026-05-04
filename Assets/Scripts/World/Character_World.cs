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
    [SerializeField] private GameObject gameObject_Dead;
    [SerializeField] private Image image_HP;
    [SerializeField] private Image image_Armor;
    [SerializeField] private Image image_Barrier;
    [SerializeField] private Image image_Initiative;
    [SerializeField] private TextMeshProUGUI text_CharacterHP;

    [SerializeField] private Color color_Healthy = default;
    [SerializeField] private Color color_Wounded = default;
    [SerializeField] private Color color_Dying = default;

    private Character character;

    public Stat Health;
    public Stat Armor;
    public Stat Barrier;

    public Stat Burning;
    public Stat Poisoned;
    public Stat Bleeding;

    public Stat Slowed;
    public Stat Stunned;
    public Stat Blinded;
    public Stat Reloading;
    public Stat Rooted;

    private bool isAlive;

    protected override void Awake()
    {
        base.Awake();

        Manager_Initative.OnInitiativeUpdated += OnInitiativeUpdated;

        Manager_Initative.OnCharacterTurnStart += OnTurnStarted;
        Manager_Initative.OnCharacterTurnEnd += OnTurnEnd;
    }

    protected override void OnDestroy()
    {
        Manager_Initative.OnInitiativeUpdated -= OnInitiativeUpdated;

        Manager_Initative.OnCharacterTurnStart -= OnTurnStarted;
        Manager_Initative.OnCharacterTurnEnd -= OnTurnEnd;

        base.OnDestroy();
    }

    public override void Reveal()
    {
        base.Reveal();

        if (!isRevealed)
            return;

        Manager_Characters.Instance.RevealCharacter(this);
    }

    public void SetInitiative(bool active)
    {
        gameObject_InitiativeSelection.SetActive(active);
    }

    private void OnInitiativeUpdated(List<Character_Initiative> initiativeOrder)
    {
        Character_Initiative myCharacter = initiativeOrder.Where(c => c.Character.Name == character.Name).FirstOrDefault();

        if (myCharacter == null)
            return;

        SetInitativeColor(myCharacter);
    }

    private void OnTurnStarted(Character_Initiative character_Initiative)
    {
        if (character != character_Initiative.Character)
            return;

        SetInitiative(true);
        StartTurn();
    }

    private void OnTurnEnd(Character_Initiative character_Initiative)
    {
        if (character != character_Initiative.Character)
            return;

        SetInitiative(false);
        EndTurn();
    }

    // Testing
    public GameObject Ship;
    public override void Setup(Entity entity)
    {
        Character character = entity as Character;

        SetupCharacter(character);

        base.Setup(entity);

        // Testing
        if (Ship != null)
            Ship.SetActive(character.HasShip);
    }

    private void SetupStats()
    {
        isAlive = true;

        Armor.SetValue(0);
        Barrier.SetValue(0);
        Burning.SetValue(0);
        Poisoned.SetValue(0);
        Bleeding.SetValue(0);
        Slowed.SetValue(0);
        Stunned.SetValue(0);
        Blinded.SetValue(0);
        Reloading.SetValue(0);
        Rooted.SetValue(0);
    }

    private void SetupCharacter(Character character)
    {
        if (character == null)
            return;

        this.character = character;

        SetInitativeColor(Manager_Initative.Instance.GetInitiativeCharacter(character.Name));

        SetupStats();

        SetHP(character.HealthPoints);
        SetArmor(character.Armor);
        SetBarrier(character.Barier);

        ApplyBaseEffects();

        text_CharacterHP.enabled = character.IsPlayer;
    }

    private void ApplyBaseEffects()
    {
        for (int bleedValue = 0; bleedValue < character.Bleed; bleedValue++)
        {
            ApplyBleed();
        }

        for (int BlindedValue = 0; BlindedValue < character.Blinded; BlindedValue++)
        {
            ApplyBlinded();
        }

        for (int BurnValue = 0; BurnValue < character.Burn; BurnValue++)
        {
            ApplyBurn();
        }

        for (int ReloadValue = 0; ReloadValue < character.Reload; ReloadValue++)
        {
            ApplyReloading();
        }

        for (int RootValue = 0; RootValue < character.Root; RootValue++)
        {
            ApplyRooted();
        }

        for (int SlowedValue = 0; SlowedValue < character.Slowed; SlowedValue++)
        {
            ApplySlowed();
        }

        for (int StunValue = 0; StunValue < character.Stun; StunValue++)
        {
            ApplyStunned();
        }

        for (int VenomValue = 0; VenomValue < character.Venom; VenomValue++)
        {
            ApplyVenom();
        }
    }

    private void StartTurn()
    {
        HandleBurn();
        HandleVenom();
        HandleBleed();
    }

    private void EndTurn()
    {
        CleanseBlinded();
        CleanseRoot();
        CleanseSlowed();
        CleanseStunned();
        CleanseReload();
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

    private void SetInitativeColor(Character_Initiative character_Initative)
    {
        if (character_Initative == null)
            return;

        image_Initiative.color = character_Initative.InitativeColor;
    }

    public void Damage(float damage, bool ignoreBarrier = false, bool ignoreArmor = false)
    {
        if (Barrier.StatActive && !ignoreBarrier)
        {
            float barrierDamage = damage;
            damage -= Barrier.StatValue;

            SetBarrier(Barrier.StatValue - barrierDamage);

            if (damage <= 0)
                return;
        }

        if (Armor.StatActive && !ignoreArmor)
        {
            float armorDamage = damage;
            damage -= Armor.StatValue;

            SetArmor(Armor.StatValue - armorDamage);

            if (damage <= 0)
                return;
        }

        SetHP(Health.StatValue - damage);
    }

    public void Heal(float heal)
    {
        float trueHeal = heal;

        if (Bleeding.StatValue > 0)
        {
            if (heal >= Bleeding.StatValue)
            {
                trueHeal = heal - Bleeding.StatValue;

                Bleeding.SetValue(0);
            }
            else
            {
                trueHeal = 0;
                Bleeding.SetValue(Bleeding.StatValue - heal);
            }
        }

        SetHP(Health.StatValue + trueHeal);
    }

    private void SetBarrier(float value)
    {
        Barrier.SetValue(value);
        SetBars();
    }

    private void SetArmor(float value)
    {
        Armor.SetValue(value);
        SetBars();
    }

    private void SetHP(float value)
    {
        if (value < 0)
            value = 0;

        if (value > character.HealthPoints)
            value = character.HealthPoints;

        Health.SetValue(value);
        
        if (!isAlive)
        {
            if (Health.StatValue > 0)
            {
                SetLifeStatus(true);
                Manager_Initative.Instance.AddToInitiative(character);
            }
        }

        float healthPercentage = (float)Health.StatValue / (float)character.HealthPoints;

        SetBars();
        SetHealthColor(healthPercentage);

        text_CharacterHP.text = Health.StatValue.ToString();

        if (Health.StatValue == 0)
        {
            Kill();
        }
    }

    private void SetBars()
    {
        float total = Health.StatValue + Armor.StatValue + Barrier.StatValue;

        // Max Health
        if (Health.StatValue == character.HealthPoints)
        {
            image_HP.fillAmount = Health.StatValue / total;
            image_Armor.fillAmount = (Health.StatValue + Armor.StatValue) / total;
            image_Barrier.fillAmount = 1;
        }
        else
        {
            // Total is under max health
            if (total < character.HealthPoints)
            {
                image_HP.fillAmount = Health.StatValue / character.HealthPoints;
                image_Armor.fillAmount = (Health.StatValue + Armor.StatValue) / character.HealthPoints;
                image_Barrier.fillAmount = (Health.StatValue + Armor.StatValue + Barrier.StatValue) / character.HealthPoints;
            }
            // Total is above max health, but hp is not at max
            else
            {
                // Only Armor is Active
                if (Armor.StatActive && !Barrier.StatActive)
                {
                    image_HP.fillAmount = Health.StatValue / total;
                    image_Armor.fillAmount = 1;
                    image_Barrier.fillAmount = 0;
                }
                // Only Barrier is Active
                else if (!Armor.StatActive && Barrier.StatActive)
                {
                    image_HP.fillAmount = Health.StatValue / total;
                    image_Armor.fillAmount = 0;
                    image_Barrier.fillAmount = 1;
                }
                else
                {
                    image_HP.fillAmount = Health.StatValue / total;
                    image_Armor.fillAmount = (Health.StatValue + Armor.StatValue) / total;
                    image_Barrier.fillAmount = 1;
                }
            }
        }
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
        if (isAlive)
        {
            SetLifeStatus(false);
            Manager_Initative.Instance.RemoveCharacter(character);
            return;
        }

        Remove();
    }

    private void SetLifeStatus(bool alive)
    {
        isAlive = alive;
        gameObject_Dead.SetActive(!alive);
    }

    #region Apply Effects

    public void ApplyArmor(bool apply = true)
    {
        Armor.SetValue(apply ? Armor.StatValue + 1 : Armor.StatValue - 1);
        SetBars();
    }

    public void ApplyBarrier(bool apply = true)
    {
        Barrier.SetValue(apply ? Barrier.StatValue + 1 : Barrier.StatValue - 1);
        SetBars();
    }

    public void ApplyBleed(bool apply = true)
    {
        Bleeding.SetValue(apply ? Bleeding.StatValue + 1 : Bleeding.StatValue - 1);
    }

    public void ApplyBlinded(bool apply = true)
    {
        Blinded.SetValue(apply ? Blinded.StatValue + 1 : Blinded.StatValue - 1);
    }

    public void ApplyBurn(bool apply = true)
    {
        Burning.SetValue(apply ? Burning.StatValue + 1 : Burning.StatValue - 1);
    }

    public void ApplyReloading(bool apply = true)
    {
        Reloading.SetValue(apply ? Reloading.StatValue + 1 : Reloading.StatValue - 1);
    }

    public void ApplyRooted(bool apply = true)
    {
        Rooted.SetValue(apply ? Rooted.StatValue + 1 : Rooted.StatValue - 1);
    }

    public void ApplySlowed(bool apply = true)
    {
        Slowed.SetValue(apply ? Slowed.StatValue + 1 : Slowed.StatValue - 1);
    }

    public void ApplyStunned(bool apply = true)
    {
        Stunned.SetValue(apply ? Stunned.StatValue + 1 : Stunned.StatValue - 1);
    }

    public void ApplyVenom(bool apply = true)
    {
        Poisoned.SetValue(apply ? Poisoned.StatValue + 1 : Poisoned.StatValue - 1);
    }

    #endregion

    #region Cleanse Effects

    public void CleanseBleed()
    {
        if (!Bleeding.StatActive)
            return;

        ApplyBleed(false);
    }

    public void CleanseBlinded()
    {
        if (!Blinded.StatActive)
            return;

        ApplyBlinded(false);
    }

    public void CleanseBurn()
    {
        if (!Burning.StatActive)
            return;

        ApplyBurn(false);
    }

    public void CleanseReload()
    {
        if (!Reloading.StatActive)
            return;

        ApplyReloading(false);
    }

    public void CleanseRoot()
    {
        if (!Rooted.StatActive)
            return;

        ApplyRooted(false);
    }

    public void CleanseSlowed()
    {
        if (!Slowed.StatActive)
            return;

        ApplySlowed(false);
    }

    public void CleanseStunned()
    {
        if (!Stunned.StatActive)
            return;

        ApplyStunned(false);
    }

    public void CleanseVenom()
    {
        if (!Poisoned.StatActive)
            return;

        ApplyVenom(false);
    }

    #endregion

    public int GetMovement()
    {
        int movement = character.Movement;

        if (Rooted.StatActive)
            return 0;

        if (Slowed.StatActive)
            return (int)(character.IsPlayerTeam ? Mathf.Ceil(movement / 2f) : Mathf.Floor(movement / 2f));

        return movement;
    }
}

[Serializable]
public class Stat
{
    public float StatValue => statValue;

    private float statValue;

    public GameObject Icon;

    public void SetValue(float value)
    {
        if (value < 0)
            value = 0;

        statValue = value;

        if (Icon != null)
            Icon.SetActive(StatActive);
    }

    public bool StatActive => StatValue > 0;
}