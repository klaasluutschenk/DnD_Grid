using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Manager_Initative : MonoBehaviour
{
    public static Manager_Initative Instance;

    public static Action OnInitativeSetup;
    public static Action<character_Initative> OnInitiativeSelectionUpdated;
    public static Action<List<character_Initative>> OnInitiativeOrderUpdated;

    public static Action<List<character_Initative>> OnCustomInitiativeRequest;

    [SerializeField] private List<Color> initiativeColors = new List<Color>();

    private List<character_Initative> characters = new List<character_Initative>();
    private List<Color> availableColors = new List<Color>();

    private List<character_Initative> customInitatives = new List<character_Initative>();

    private character_Initative activeCharacter;

    private void Awake()
    {
        Instance = this;

        Manager_Combat.OnCombatEncounterEnded += OnCombatEncounterEnded;
        Manager_Combat.OnCombatEncounterLoaded += OnCombatEncounterLoaded;
        Manager_Combat.OnCombatEncounterStarted += OnCombatEncounterStarted;

        CustomInitiativeUI.OnCharacterSet += OnCharacterSet;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            NextCharacter();
    }

    private void OnCombatEncounterEnded()
    {
        characters.Clear();
    }

    public void InjectNewCharacter(Character character)
    {
        if (IsCharacterInPlay(character.Name))
            return;

        character_Initative newCharacter =
                new character_Initative(
                    character,
                    character.initiative);

        Color asignedColor = GetRandomAvailableColor();
        if (character.InitativeColor == Color.black)
        {
            newCharacter.InitativeColor = asignedColor;

            LockColor(asignedColor);
        }
        else
        {
            newCharacter.InitativeColor = character.InitativeColor;

            LockColor(character.InitativeColor);
        }

        if (character.CustomInitiative)
        {
            customInitatives.Add(newCharacter);
            CheckCustomInitiative();
            return;
        }

        characters.Add(newCharacter);

        if (character.AdditionalInitiatives.Count > 0)
        {
            foreach (int initiative in character.AdditionalInitiatives)
            {
                character_Initative clone = new character_Initative(
                    newCharacter.Character,
                    initiative);

                clone.InitativeColor = asignedColor;

                characters.Add(clone);
            }
        }

        UpdateInitiativeOrder();
    }

    private void InjectCharcter(character_Initative character_Initative)
    {
        characters.Add(character_Initative);
        UpdateInitiativeOrder();

        if (character_Initative.Character.IsPlayer)
        {
            OnCombatEncounterStarted();
        }
    }

    private void OnCombatEncounterLoaded(CombatEncounter combatEncounter)
    {
        LoadColors();

        CheckCustomInitiative();
        UpdateInitiativeOrder();
    }

    private void OnCombatEncounterStarted()
    {
        activeCharacter = characters[0];
        UpdateInitiativeSelection();
    }

    private void UpdateInitiativeOrder()
    {
        characters = characters.OrderByDescending(c => c.Initiative).ToList();

        OnInitiativeOrderUpdated?.Invoke(characters);
    }

    public character_Initative GetInitiativeCharacter(string characterName)
    {
        return characters.Where(c => c.Character.Name == characterName).FirstOrDefault();
    }

    private void UpdateInitiativeSelection()
    {
        OnInitiativeSelectionUpdated?.Invoke(activeCharacter);
    }

    private void NextCharacter()
    {
        int index = 0;

        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i] == activeCharacter)
                index = i;
        }

        if (index == characters.Count - 1)
            index = 0;
        else
            index++;

        SelectCharacter(characters[index]);
    }

    private void SelectCharacter(character_Initative character_Initative)
    {
        activeCharacter = character_Initative;
        UpdateInitiativeSelection();
    }

    private void CheckCustomInitiative()
    {
        if (customInitatives.Count == 0)
        {
            OnInitativeSetup?.Invoke();
            return;
        }

        OnCustomInitiativeRequest?.Invoke(customInitatives);
    }

    private void UpdateCustomInitiatives(character_Initative character_Initative)
    {
        if (!customInitatives.Contains(character_Initative))
            return;

        customInitatives.Remove(character_Initative);

        CheckCustomInitiative();
    }

    public bool IsCharacterInPlay(string characterName)
    {
        foreach (character_Initative character_Initative in characters)
        {
            if (character_Initative.Character.Name == characterName)
                return true;
        }

        return false;
    }

    private void LoadColors()
    {
        initiativeColors.ForEach(c => availableColors.Add(c));
    }

    private void LockColor(Color color)
    {
        if (!availableColors.Contains(color))
            return;

        availableColors.Remove(color);
    }

    private Color GetRandomAvailableColor()
    {
        int randomIndex = UnityEngine.Random.Range(0, availableColors.Count);

        Color randomColor = availableColors[randomIndex];

        return randomColor;
    }

    private void OnCharacterSet(character_Initative character_Initative)
    {
        if (!customInitatives.Contains(character_Initative))
            return;

        InjectCharcter(character_Initative);
        customInitatives.Remove(character_Initative);
        CheckCustomInitiative();
    }
}

[Serializable]
public class character_Initative
{
    public Character Character;

    public int Initiative;
    public Color InitativeColor;

    public character_Initative(Character Character, int Initiative)
    {
        this.Character = Character;
        this.Initiative = Initiative;
    }
}
