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
    }

    private void OnCustomInitiativeRequest(List<Character> characters)
    {
        activePanels.ForEach(ap => Destroy(ap.gameObject));
        activePanels.Clear();

        container.gameObject.SetActive(true);

        foreach (Character character in characters)
        {
            CustomInitiativeUI newUi = Instantiate(CustomInitiativeUI_Prefab, container);

            newUi.Setup(character);

            newUi.OnRemove += Remove;

            activePanels.Add(newUi);
        }
    }

    private void Remove(CustomInitiativeUI customInitiativeUI)
    {
        customInitiativeUI.OnRemove -= Remove;

        activePanels.Remove(customInitiativeUI);
        Destroy(customInitiativeUI.gameObject);

        if (activePanels.Count == 0)
        {
            container.gameObject.SetActive(false);
        }
    }
}
