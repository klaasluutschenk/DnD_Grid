using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Manager_Initative : MonoBehaviour
{
    public static Action<List<Character_Initative>> OnInitiativeUpdated;
    public static Action<Character> OnCharacterTurnStart;
    public static Action<Character> OnCharacterTurnEnd;

    public static Manager_Initative Instance;

    private List<Character_Initative> activeCharacters = new List<Character_Initative>();
    private List<Inactive_Character_Initative> inactiveCharacters = new List<Inactive_Character_Initative>();
    private List<Character> customInitiativesToSetup = new List<Character>();


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

    public void AddToInitiative(Character character)
    {
        if (IsCharacterInPlay(character))
        {
            IncreaseCharacter(character);
            return;
        }

        if (IsCharacterInactive(character))
        {
            ReaddCharacter(character);
            return;
        }

        AddNewCharacter(character);
    }

    private void AddNewCharacter(Character character)
    {
        if (character.CustomInitiative)
        {
            customInitiativesToSetup.Add(character);
            CheckCustomInitiative();

            return;
        }

        Color asignedColor = GetRandomAvailableColor();
        LockColor(asignedColor);

        Character_Initative newCharacter =
                new Character_Initative(
                    character,
                    character.initiative,
                    asignedColor);

        newCharacter.unitCount = 1;
        activeCharacters.Add(newCharacter);

        if (character.AdditionalInitiatives.Count > 0)
        {
            foreach (int initiative in character.AdditionalInitiatives)
            {
                Character_Initative clone = new Character_Initative(
                    newCharacter.Character,
                    initiative,
                    asignedColor);

                clone.unitCount = 1;
                activeCharacters.Add(clone);
            }
        }

        UpdateInitiativeOrder();
        OnInitiativeUpdated?.Invoke(activeCharacters);
    }

    private void ReaddCharacter(Character character)
    {
        Inactive_Character_Initative characterToReadd = GetInactiveCharacter(character);

        inactiveCharacters.Remove(characterToReadd);

        Character_Initative newCharacter =
                new Character_Initative(
                    character,
                    character.initiative,
                    characterToReadd.InitativeColor);

        newCharacter.unitCount = 1;
        activeCharacters.Add(newCharacter);

        if (character.AdditionalInitiatives.Count > 0)
        {
            foreach (int initiative in character.AdditionalInitiatives)
            {
                Character_Initative clone = new Character_Initative(
                    newCharacter.Character,
                    initiative,
                    characterToReadd.InitativeColor);

                clone.unitCount = 1;
                activeCharacters.Add(clone);
            }
        }

        UpdateInitiativeOrder();
        OnInitiativeUpdated?.Invoke(activeCharacters);
    }

    private void IncreaseCharacter(Character character)
    {
        List<Character_Initative> characterAndClones = GetCharactersInInitiative(character);

        characterAndClones.ForEach(chs => chs.unitCount += 1);

        OnInitiativeUpdated?.Invoke(activeCharacters);
    }

    public void RemoveCharacter(Character character)
    {
        List<Character_Initative> characterAndClones = GetCharactersInInitiative(character);

        bool hasBeenAddedToInactiveList = false;

        foreach (Character_Initative character_Initative in characterAndClones)
        {
            character_Initative.unitCount -= 1;

            if (character_Initative.unitCount == 0)
            {
                if (!hasBeenAddedToInactiveList)
                {
                    hasBeenAddedToInactiveList = true;
                    inactiveCharacters.Add(new Inactive_Character_Initative(character_Initative.Character, character_Initative.InitativeColor));
                }

                activeCharacters.Remove(character_Initative);
            }
        }

        UpdateInitiativeOrder();
        OnInitiativeUpdated?.Invoke(activeCharacters);
    }

    private void UpdateInitiativeOrder()
    {
        characters = characters.OrderByDescending(c => c.Initiative).ToList();

        OnInitiativeOrderUpdated?.Invoke(activeCharacters);
    }

    // Single Character
    public Character_Initative GetCharacterInInitative(Character character)
    {
        foreach (Character_Initative character_Initative in activeCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                return character_Initative;
        }

        return null;
    }

    // Character and their clones
    public List<Character_Initative> GetCharactersInInitiative(Character character)
    {
        List<Character_Initative> characterAndClones = new List<Character_Initative>();

        foreach (Character_Initative character_Initative in activeCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                characterAndClones.Add(character_Initative);
        }

        return characterAndClones;
    }

    public bool IsCharacterInPlay(Character character)
    {
        foreach (Character_Initative character_Initative in activeCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                return true;
        }

        return false;
    }

    public bool IsCharacterInactive(Character character)
    {
        foreach (Inactive_Character_Initative character_Initative in inactiveCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                return true;
        }

        return false;
    }

    public Inactive_Character_Initative GetInactiveCharacter(Character character)
    {
        foreach (Inactive_Character_Initative character_Initative in inactiveCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                return character_Initative;
        }

        return null;
    }

    #region Colors

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

    #endregion



    public static Action OnInitativeSetup;
    public static Action<Character_Initative> OnInitiativeSelectionUpdated;
    public static Action<List<Character_Initative>> OnInitiativeOrderUpdated;

    public static Action<List<Character_Initative>> OnCustomInitiativeRequest;

    [SerializeField] private List<Color> initiativeColors = new List<Color>();

    private List<Character_Initative> characters = new List<Character_Initative>();
    private List<Color> availableColors = new List<Color>();

    private List<Character_Initative> customInitatives = new List<Character_Initative>();

    private Character_Initative activeCharacter;

    public void InjectNewCharacter(Character character)
    {
        
        UpdateInitiativeOrder();
    }

    private void InjectCharcter(Character_Initative character_Initative)
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
        //activeCharacter = characters[0];
        //UpdateInitiativeSelection();
    }

    public Character_Initative GetInitiativeCharacter(string characterName)
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

    private void SelectCharacter(Character_Initative character_Initative)
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

    private void UpdateCustomInitiatives(Character_Initative character_Initative)
    {
        if (!customInitatives.Contains(character_Initative))
            return;

        customInitatives.Remove(character_Initative);

        CheckCustomInitiative();
    }

    private void OnCharacterSet(Character_Initative character_Initative)
    {
        if (!customInitatives.Contains(character_Initative))
            return;

        InjectCharcter(character_Initative);
        customInitatives.Remove(character_Initative);
        CheckCustomInitiative();
    }
}

[Serializable]
public class Character_Initative
{
    public Character Character;

    public int Initiative;
    public Color InitativeColor;

    public int unitCount;

    public Character_Initative(Character character, int initiative, Color initiativeColor)
    {
        Character = character;
        Initiative = initiative;
        InitativeColor = initiativeColor;
    }
}

[Serializable]
public class Inactive_Character_Initative
{
    public Character Character;
    public Color InitativeColor;

    public Inactive_Character_Initative(Character character, Color initiativeColor)
    {
        Character = character;
        InitativeColor = initiativeColor;
    }
}