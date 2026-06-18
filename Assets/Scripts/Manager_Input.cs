using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance;

    public static Action OnSpawnRequest;
    public static Action OnStopSpawnRequest;

    public InputState InputState => inputState;
    public float FogTimer => fogTimer;

    [SerializeField] private Button button_Selection = default;
    [SerializeField] private TMP_InputField inputField_Selection = default;

    [SerializeField] private Button button_Spawn = default;

    [SerializeField] private Button button_Impact = default;

    [SerializeField] private Camera playerCamera;

    private InputState inputState = InputState.Default;

    public Vector3 mouseGridPosition;
    public Vector3 oldMouseGridPosition;

    private List<Tile> highlightedTiles = new List<Tile>();

    private Tile mainTile;

    private int currentRoomSelection;
    private int display;

    private Character_World movementSelection;

    private Character characterToSpawn;

    private float fogTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        mainTile = highlightedTiles[0];

        if (mainTile == null)
            return;

        // Spawning

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (inputState == InputState.Default)
            {
                ActivateSpawning();
                return;
            }

            if (inputState == InputState.Spawning)
            {
                DeactivateSpawing();
                return;
            }
        }

        if (inputState == InputState.Spawning)
        {
            Spawning();
            return;
        }
    }

    #region Spawning

    private void ActivateSpawning()
    {
        //ToggleInputState(InputState.Spawning);

        OnSpawnRequest?.Invoke();
    }

    private void DeactivateSpawing()
    {
        //ToggleInputState(InputState.Default);
        characterToSpawn = null;

        OnStopSpawnRequest?.Invoke();
    }

    public void SetCharacterToSpawn(Character character)
    {
        characterToSpawn = character;
    }

    private void Spawning()
    {
        if (characterToSpawn == null)
            return;

        if (mainTile.HasWorldEntity)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Manager_Characters.Instance.SpawnCharacter(characterToSpawn, mainTile.transform.position);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            DeactivateSpawing();
        }
    }

    #endregion
}