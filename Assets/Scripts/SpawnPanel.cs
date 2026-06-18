using UnityEngine;
using System;
using System.Collections.Generic;

public class SpawnPanel : MonoBehaviour
{
    [SerializeField] private SpawnUI spawnUI_Prefab = default;
    [SerializeField] private Transform prefabContainer = default;
    [SerializeField] private Transform container = default;

    private List<SpawnUI> activeSpawnUIs = new List<SpawnUI>();

    private void Awake()
    {
        Manager_Input_2.OnSpawnRequest += OnSpawnRequest;
        Manager_Input_2.OnStopSpawnRequest += OnStopSpawnRequest;
    }

    private void OnSpawnRequest()
    {
        container.gameObject.SetActive(true);

        foreach (Character character in Manager_Encounter.Instance.CombatEncounter.SpawnableCharacters)
        {
            SpawnUI spawnUI = Instantiate(spawnUI_Prefab, prefabContainer);

            spawnUI.Setup(character);
            spawnUI.OnClicked += OnClicked;

            activeSpawnUIs.Add(spawnUI);
        }
    }

    private void OnStopSpawnRequest()
    {
        container.gameObject.SetActive(false);

        foreach (SpawnUI spawnUI in activeSpawnUIs)
        {
            spawnUI.OnClicked -= OnClicked;
            Destroy(spawnUI.gameObject);
        }

        activeSpawnUIs.Clear();
    }

    private void OnClicked(Character character)
    {
        Manager_Input_2.Instance.SetCharacterToSpawn(character);
    }
}
