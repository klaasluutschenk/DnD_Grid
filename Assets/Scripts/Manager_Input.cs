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

    private int selectionRadius = 1;
    private int damageValue = 1;

    public Vector3 mouseGridPosition;
    public Vector3 oldMouseGridPosition;

    private List<Tile> highlightedTiles = new List<Tile>();
    private List<Tile> selectedtiles = new List<Tile>();
    private List<Tile> movementTiles = new List<Tile>();
    private List<Tile> fogTiles = new List<Tile>();

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
        if (highlightedTiles.Count == 0)
        {
            Manager_Tooltip.Instance.ClearTooltip();
            return;
        }

        mainTile = highlightedTiles[0];

        if (mainTile == null)
            return;

        // Tooltip

        if (Input.GetKey(KeyCode.Space))
            Manager_Tooltip.Instance.SetToolTip(mainTile);
        else
            Manager_Tooltip.Instance.ClearTooltip();

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