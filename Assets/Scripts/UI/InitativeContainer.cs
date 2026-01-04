using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InitativeContainer : MonoBehaviour
{
    [SerializeField] private Initiative_UI Initiative_UI_Prefab = default;
    [SerializeField] private Transform container = default;

    private List<Initiative_UI> activeUIs = new List<Initiative_UI>();
    private List<Character_Initiative> activeCharacters = new List<Character_Initiative>();

    private void Awake()
    {
        Manager_Initative.OnInitiativeUpdated += OnInitiativeUpdated;
        Manager_Initative.OnCharacterTurnStart += OnCharacterTurnStart;
        Manager_Initative.OnCharacterTurnEnd += OnCharacterTurnEnd;

        //Manager_Initative.OnInitiativeOrderUpdated += OnInitiativeOrderUpdated;
        //Manager_Initative.OnInitiativeSelectionUpdated += OnInitiativeSelectionUpdated;
    }

    private void OnInitiativeUpdated(List<Character_Initiative> initiativeOrder)
    {
        Setup(initiativeOrder);
        OrderUI();
    }

    private void OnCharacterTurnStart(Character_Initiative character_Initative)
    {
        Initiative_UI ui = GetUIByCharacter(character_Initative);

        if (ui == null)
            return;

        ui.SetInitative(true);
    }

    private void OnCharacterTurnEnd(Character_Initiative character_Initative)
    {
        Initiative_UI ui = GetUIByCharacter(character_Initative);

        if (ui == null)
            return;

        ui.SetInitative(false);
    }

    private Initiative_UI GetUIByCharacter(Character_Initiative character_Initativer)
    {
        foreach (Initiative_UI ui in activeUIs)
        {
            if (ui.Character == character_Initativer)
                return ui;
        }

        return null;
    }

    private void OnInitiativeSelectionUpdated(Character_Initiative character_Initative)
    {
        activeUIs.ForEach(a => a.SetInitative(a.Character == character_Initative));
    }

    private void Setup(List<Character_Initiative> initiativeOrder)
    {
        List<Character_Initiative> charactersToAdd = new List<Character_Initiative>();
        
        foreach (Character_Initiative character_Initative in initiativeOrder)
        {
            if (!activeCharacters.Contains(character_Initative))
            {
                charactersToAdd.Add(character_Initative);
            }
        }

        List<Character_Initiative> charactersToRemove = new List<Character_Initiative>();

        foreach (Character_Initiative characterToRemove in activeCharacters)
        {
            if (!initiativeOrder.Contains(characterToRemove))
            {
                charactersToRemove.Add(characterToRemove);
            }
        }

        foreach (Character_Initiative character in charactersToAdd)
        {
            SpawnInitativeUI(character);
        }

        foreach (Character_Initiative character in charactersToRemove)
        {
            DestroyInitiativeUI(character);
        }
    }

    private void SpawnInitativeUI(Character_Initiative character_Initative)
    {
        Initiative_UI newUI = Instantiate(Initiative_UI_Prefab, container);

        newUI.Setup(character_Initative);

        activeUIs.Add(newUI);
        activeCharacters.Add(character_Initative);
    }

    private void DestroyInitiativeUI(Character_Initiative character_Initative)
    {
        Initiative_UI targetUI = null;

        foreach (Initiative_UI initiative_UI in activeUIs)
        {
            if (initiative_UI.Character == character_Initative)
            {
                targetUI = initiative_UI;
                break;
            }
        }

        activeUIs.Remove(targetUI);
        activeCharacters.Remove(character_Initative);

        Destroy(targetUI.gameObject);
    }
    
    private void OrderUI()
    {
        activeUIs = activeUIs.OrderByDescending(a => a.Character.Initiative).ToList();

        foreach(Initiative_UI initiative_UI in activeUIs)
        {
            initiative_UI.transform.SetAsLastSibling();
        }
    }
}
