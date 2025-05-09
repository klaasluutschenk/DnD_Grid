using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InitativeContainer : MonoBehaviour
{
    [SerializeField] private Initiative_UI Initiative_UI_Prefab = default;
    [SerializeField] private Transform container = default;

    private List<Initiative_UI> activeUIs = new List<Initiative_UI>();
    private List<character_Initative> activeCharacters = new List<character_Initative>();

    private void Awake()
    {
       Manager_Initative.OnInitiativeOrderUpdated += OnInitiativeOrderUpdated;
       Manager_Initative.OnInitiativeSelectionUpdated += OnInitiativeSelectionUpdated;
    }

    private void OnInitiativeOrderUpdated(List<character_Initative> initiativeOrder)
    {
        Setup(initiativeOrder);
        OrderUI();
    }

    private void OnInitiativeSelectionUpdated(character_Initative character_Initative)
    {
        activeUIs.ForEach(a => a.SetInitative(a.Character == character_Initative));
    }

    private void Setup(List<character_Initative> initiativeOrder)
    {
        List<character_Initative> charactersToAdd = new List<character_Initative>();
        
        foreach (character_Initative character_Initative in initiativeOrder)
        {
            if (!activeCharacters.Contains(character_Initative))
            {
                charactersToAdd.Add(character_Initative);
            }
        }

        foreach (character_Initative character in charactersToAdd)
        {
            SpawnInitativeUI(character);
        }
    }

    private void SpawnInitativeUI(character_Initative character_Initative)
    {
        Initiative_UI newUI = Instantiate(Initiative_UI_Prefab, container);

        newUI.Setup(character_Initative);

        activeUIs.Add(newUI);
        activeCharacters.Add(character_Initative);
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
