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

        // Initiative

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Initiative();
            inputState = InputState.Initiative;
            return;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            inputState = InputState.Default;
            return;
        }

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

    private void Initiative()
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (Input.GetMouseButtonDown(0))
        {
            character_World.SetInitiative(false);
        }

        if (Input.GetMouseButtonDown(1))
            character_World.SetInitiative(true);
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

public enum InputState
{
    Default = -1,
    Movement = 0,
    Armor = 1,
    Barrier = 2,
    Bleed = 3,
    Blinded = 4,
    Burn = 5,
    Reload = 6,
    Root = 7,
    Slowed = 8,
    Stun = 9,
    Venom = 10,
    Initiative = 11,
    Spawning = 12,
    TrueDamage = 13
}