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

    private void InjectCharcter(character_Initative character_Initative)
    {
        characters.Add(character_Initative);
        UpdateInitiativeOrder();

        if (character_Initative.IsPlayer)
        {
            OnCombatEncounterStarted();
        }
    }

    private void OnCombatEncounterLoaded(CombatEncounter combatEncounter)
    {
        LoadColors();

        foreach (CombatEncounter_Character character in combatEncounter.Characters)
        {
            if (IsCharacterInPlay(character.Character.name))
                continue;

            character_Initative newCharacter =
                new character_Initative(
                    character.Character.IsPlayer,
                    character.Character.name,
                    character.Character.initiative,
                    character.Character.Sprite); ;

            if (character.Character.InitativeColor == Color.black)
            {
                Color asignedColor = GetRandomAvailableColor();

                newCharacter.InitativeColor = asignedColor;

                LockColor(asignedColor);
            }
            else
            {
                newCharacter.InitativeColor = character.Character.InitativeColor;

                LockColor(character.Character.InitativeColor);
            }

            if (character.Character.CustomInitiative)
            {
                customInitatives.Add(newCharacter);
                continue;
            }

            characters.Add(newCharacter);
        }

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
        return characters.Where(c => c.Name == characterName).FirstOrDefault();
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

    private bool IsCharacterInPlay(string characterName)
    {
        foreach (character_Initative character_Initative in characters)
        {
            if (character_Initative.Name == characterName)
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
    public bool IsPlayer;
    public string Name;
    public int Initiative;
    public Color InitativeColor;
    public Sprite Sprite;

    public character_Initative(bool IsPlayer, string Name, int Initiative, Sprite Sprite)
    {
        this.IsPlayer = IsPlayer;
        this.Name = Name;
        this.Initiative = Initiative;
        this.Sprite = Sprite;
    }
}
