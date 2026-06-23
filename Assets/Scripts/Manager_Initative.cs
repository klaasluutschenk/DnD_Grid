using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Manager_Initative : MonoBehaviour
{
    public static Manager_Initative Instance;

    public static Action<List<Character_Initiative>> OnInitiativeUpdated;
    public static Action<Character_Initiative> OnCharacterTurnStart;
    public static Action<Character_Initiative> OnCharacterTurnEnd;

    public static Action<List<Character>> OnCustomInitiativeRequest;

    [SerializeField] private List<Color> initiativeColors = new List<Color>();
    [SerializeField] private List<Color> initiativeColorsPlayers = new List<Color>();

    private List<Character_Initiative> activeCharacters = new List<Character_Initiative>();
    private List<Inactive_Character_Initiative> inactiveCharacters = new List<Inactive_Character_Initiative>();
    private List<Character> customInitiativesToSetup = new List<Character>();
    private List<Color> availableColors = new List<Color>();
    private List<Color> availableColorsPlayers = new List<Color>();

    private Character_Initiative activeCharacter;

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
        LoadColors();

        UpdateInitiativeOrder();

        CustomInitiativeUI.OnCharacterSet += OnCharacterSet;

        Debug.Log("Initiative Loaded");

        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            NextCharacter();

        if (Input.GetKeyDown(KeyCode.Return))
            StartEncounter();
    }

    public void AddToInitiative(Character character, int customInitative = -1)
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

        AddNewCharacter(character, customInitative);
    }

    private void AddNewCharacter(Character character, int customInitative = -1)
    {
        if (character.HasNoInitiative)
            return;

        if (character.CustomInitiative && customInitative == -1)
        {
            customInitiativesToSetup.Add(character);
            OnCustomInitiativeRequest?.Invoke(customInitiativesToSetup);
            return;
        }

        Color asignedColor = Color.white;

        if (character.IsPlayer)
        {
            asignedColor = GetRandomAvailableColorPlayer();
            LockColorPlayer(asignedColor);
        }
        else
        {
            asignedColor = GetRandomAvailableColor();
            LockColor(asignedColor);
        }

        Character_Initiative newCharacter =
                new Character_Initiative(
                    character,
                    customInitative == -1 ? character.initiative : customInitative,
                    asignedColor);

        newCharacter.unitCount = 1;
        activeCharacters.Add(newCharacter);

        if (character.AdditionalInitiatives.Count > 0)
        {
            foreach (int initiative in character.AdditionalInitiatives)
            {
                Character_Initiative clone = new Character_Initiative(
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
        Inactive_Character_Initiative characterToReadd = GetInactiveCharacter(character);

        inactiveCharacters.Remove(characterToReadd);

        Character_Initiative newCharacter =
                new Character_Initiative(
                    character,
                    characterToReadd.Initiative,
                    characterToReadd.InitativeColor);

        newCharacter.unitCount = 1;
        activeCharacters.Add(newCharacter);

        if (character.AdditionalInitiatives.Count > 0)
        {
            foreach (int initiative in character.AdditionalInitiatives)
            {
                Character_Initiative clone = new Character_Initiative(
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
        List<Character_Initiative> characterAndClones = GetCharactersInInitiative(character);

        characterAndClones.ForEach(chs => chs.unitCount += 1);

        OnInitiativeUpdated?.Invoke(activeCharacters);
    }

    public void RemoveCharacter(Character character)
    {
        if (character.HasNoInitiative)
            return;

        List<Character_Initiative> characterAndClones = GetCharactersInInitiative(character);

        bool hasBeenAddedToInactiveList = false;

        foreach (Character_Initiative character_Initative in characterAndClones)
        {
            character_Initative.unitCount -= 1;

            if (character_Initative.unitCount == 0)
            {
                if (!hasBeenAddedToInactiveList)
                {
                    hasBeenAddedToInactiveList = true;
                    inactiveCharacters.Add(new Inactive_Character_Initiative(character_Initative.Character, character_Initative.InitativeColor, character_Initative.Initiative));
                }

                activeCharacters.Remove(character_Initative);
            }
        }

        UpdateInitiativeOrder();
        OnInitiativeUpdated?.Invoke(activeCharacters);
    }

    private void UpdateInitiativeOrder()
    {
        activeCharacters = activeCharacters.OrderByDescending(c => c.Initiative).ToList();
        OnInitiativeUpdated?.Invoke(activeCharacters);
    }

    // Single Character
    public Character_Initiative GetCharacterInInitative(Character character)
    {
        foreach (Character_Initiative character_Initative in activeCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                return character_Initative;
        }

        return null;
    }

    // Character and their clones
    public List<Character_Initiative> GetCharactersInInitiative(Character character)
    {
        List<Character_Initiative> characterAndClones = new List<Character_Initiative>();

        foreach (Character_Initiative character_Initative in activeCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                characterAndClones.Add(character_Initative);
        }

        return characterAndClones;
    }

    public bool IsCharacterInPlay(Character character)
    {
        foreach (Character_Initiative character_Initative in activeCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                return true;
        }

        return false;
    }

    public bool IsCharacterInactive(Character character)
    {
        foreach (Inactive_Character_Initiative character_Initative in inactiveCharacters)
        {
            if (character_Initative.Character.Name == character.Name)
                return true;
        }

        return false;
    }

    public Inactive_Character_Initiative GetInactiveCharacter(Character character)
    {
        foreach (Inactive_Character_Initiative character_Initative in inactiveCharacters)
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
        initiativeColorsPlayers.ForEach(c => availableColorsPlayers.Add(c));
    }

    private void LockColor(Color color)
    {
        if (!availableColors.Contains(color))
            return;

        availableColors.Remove(color);
    }

    private void LockColorPlayer(Color color)
    {
        if (!availableColorsPlayers.Contains(color))
            return;

        availableColorsPlayers.Remove(color);
    }

    private Color GetRandomAvailableColor()
    {
        int randomIndex = UnityEngine.Random.Range(0, availableColors.Count);

        Color randomColor = availableColors[randomIndex];

        return randomColor;
    }

    private Color GetRandomAvailableColorPlayer()
    {
        int randomIndex = UnityEngine.Random.Range(0, availableColorsPlayers.Count);

        Color randomColor = availableColorsPlayers[randomIndex];

        return randomColor;
    }

    public Character_Initiative GetInitiativeCharacter(string characterName)
    {
        return activeCharacters.Where(c => c.Character.Name == characterName).FirstOrDefault();
    }

    #endregion

    private void StartEncounter()
    {
        if (activeCharacters.Count == 0)
        {
            Debug.LogWarning("Please add at least one character to the initiative!");
        }

        SelectCharacter(activeCharacters.FirstOrDefault());
    }

    private void SelectCharacter(Character_Initiative character_Initative)
    {
        if (activeCharacter != null)
            OnCharacterTurnEnd?.Invoke(activeCharacter);

        activeCharacter = character_Initative;
        OnCharacterTurnStart?.Invoke(activeCharacter);
    }

    private void NextCharacter()
    {
        int index = 0;

        for (int i = 0; i < activeCharacters.Count; i++)
        {
            if (activeCharacters[i] == activeCharacter)
                index = i;
        }

        if (index == activeCharacters.Count - 1)
            index = 0;
        else
            index++;

        SelectCharacter(activeCharacters[index]);
    }

    private void OnCharacterSet(Character_Initiative_Custom character_Initiative_Custom)
    {
        AddToInitiative(character_Initiative_Custom.Character, character_Initiative_Custom.Initiative);

        customInitiativesToSetup.Remove(character_Initiative_Custom.Character);
    }
}

[Serializable]
public class Character_Initiative
{
    public Character Character;

    public int Initiative;
    public Color InitativeColor;

    public int unitCount;

    public Character_Initiative(Character character, int initiative, Color initiativeColor)
    {
        Character = character;
        Initiative = initiative;
        InitativeColor = initiativeColor;
    }
}

[Serializable]
public class Inactive_Character_Initiative
{
    public Character Character;
    public Color InitativeColor;
    public int Initiative;

    public Inactive_Character_Initiative(Character character, Color initiativeColor, int initiative)
    {
        Character = character;
        InitativeColor = initiativeColor;
        Initiative = initiative;
    }
}

[Serializable]
public class Character_Initiative_Custom
{
    public Character Character;
    public int Initiative;

    public Character_Initiative_Custom(Character character, int initiative)
    {
        Character = character;
        Initiative = initiative;
    }
}