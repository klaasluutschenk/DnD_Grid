using UnityEngine;
using System.Collections.Generic;

public class CustomInitativePanel : MonoBehaviour
{
    [SerializeField] private CustomInitiativeUI CustomInitiativeUI_Prefab = default;
    [SerializeField] private Transform container;

    private List<CustomInitiativeUI> activePanels = new List<CustomInitiativeUI>();

    private void Awake()
    {
        Manager_Initative.OnCustomInitiativeRequest += OnCustomInitiativeRequest;
        Manager_Initative.OnInitativeSetup += OnInitativeSetup;
    }

    private void OnCustomInitiativeRequest(List<character_Initative> characters)
    {
        activePanels.ForEach(ap => Destroy(ap.gameObject));
        activePanels.Clear();

        container.gameObject.SetActive(true);

        foreach (character_Initative character in characters)
        {
            CustomInitiativeUI newUi = Instantiate(CustomInitiativeUI_Prefab, container);

            newUi.Setup(character);

            activePanels.Add(newUi);
        }
    }

    private void OnInitativeSetup()
    {
        container.gameObject.SetActive(false);
    }
}
