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

    [SerializeField] private Button button_Selection = default;
    [SerializeField] private TMP_InputField inputField_Selection = default;

    [SerializeField] private Button button_Damage = default;
    [SerializeField] private TMP_InputField inputField_Damage = default;

    [SerializeField] private Button button_Heal= default;
    [SerializeField] private TMP_InputField inputField_Heal = default;

    [SerializeField] private Button button_Movement = default;

    [SerializeField] private Button button_Spawn = default;

    [SerializeField] private Camera playerCamera;

    private InputState inputState = InputState.None;
    private bool isLocked = false;

    private int selectionRadius = 1;
    private int damageValue = 1;
    private int healValue = 1;

    public Vector3 mouseGridPosition;
    public Vector3 oldMouseGridPosition;
    private int display;

    private List<Tile> highlightedTiles = new List<Tile>();
    private List<Tile> selectedtiles = new List<Tile>();
    private List<Tile> movementTiles = new List<Tile>();

    private Character_World movementSelection;

    private Character characterToSpawn;

    private void Awake()
    {
        Instance = this;

        button_Selection.onClick.AddListener(OnSelectionClicked);
        inputField_Selection.onValueChanged.AddListener(OnSelectionChanged);

        button_Damage.onClick.AddListener(OnDamageClicked);
        inputField_Damage.onValueChanged.AddListener(OnDamageChanged);

        button_Heal.onClick.AddListener(OnHealClicked);
        inputField_Heal.onValueChanged.AddListener(OnHealChanged);

        button_Movement.onClick.AddListener(OnMovementClicked);

        button_Spawn.onClick.AddListener(OnSpawnedClicked);
    }

    #region UI Interaction

    private void OnSelectionClicked()
    {
        if (isLocked)
            return;

        inputState = InputState.Selection;
        isLocked = true;
    }

    private void OnSelectionChanged(string value)
    {
        selectionRadius = int.Parse(value);
    }

    private void OnDamageClicked()
    {
        if (isLocked)
            return;

        selectionRadius = 1;
        inputState = InputState.Damage;
        isLocked = true;
    }

    private void OnDamageChanged(string value)
    {
        damageValue = int.Parse(value);
    }

    private void OnHealClicked()
    {
        if (isLocked)
            return;

        selectionRadius = 1;
        inputState = InputState.Heal;
        isLocked = true;
    }

    private void OnHealChanged(string value)
    {
        healValue = int.Parse(value);
    }

    private void OnMovementClicked()
    {
        if (isLocked)
            return;

        selectionRadius = 1;
        inputState = InputState.Movement;
        isLocked = true;
    }

    private void OnSpawnedClicked()
    {
        if (isLocked)
            return;

        selectionRadius = 1;
        inputState = InputState.Spawn;
        isLocked = true;

        OnSpawnRequest?.Invoke();
    }

    #endregion

    private void Update()
    {
        switch (inputState)
        {
            case InputState.None:
                break;
            case InputState.Selection:
                Selection();
                break;
            case InputState.Damage:
                Damage();
                break;
            case InputState.Heal:
                Heal();
                break;
            case InputState.Movement:
                Movement();
                break;
            case InputState.Spawn:
                Spawn();
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectionRadius = 1;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            selectionRadius = 2;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            selectionRadius = 3;

        if (Input.GetKeyDown(KeyCode.Alpha4))
            selectionRadius = 4;

        if (Input.GetKeyDown(KeyCode.Alpha5))
            selectionRadius = 5;

        if (Input.GetKeyDown(KeyCode.Alpha6))
            selectionRadius = 6;

        if (Input.GetKeyDown(KeyCode.Alpha7))
            selectionRadius = 7;

        if (Input.GetKeyDown(KeyCode.Alpha8))
            selectionRadius = 8;
    }

    #region Mouse

    private void SetMousePosition()
    {
        Vector3 mouseWorldPosition;

#if UNITY_EDITOR
        mouseWorldPosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
#else
        Vector3 relativeMousePosition = Display.RelativeMouseAt(Input.mousePosition);
        display = (int)relativeMousePosition.z;

        if (display != 9)
            return;

        mouseWorldPosition = playerCamera.ScreenToWorldPoint(relativeMousePosition);
#endif

        float relativeXPos = Mathf.Round(mouseWorldPosition.x + 0.5f) - 0.5f;
        float relativeYPos = Mathf.Round(mouseWorldPosition.y + 0.5f) - 0.5f;

        mouseGridPosition = new Vector3(relativeXPos, relativeYPos, -1);
    }

    private bool HasMouseChanged()
    {
        bool hasChanged = oldMouseGridPosition != mouseGridPosition;

        oldMouseGridPosition = mouseGridPosition;

        return hasChanged;
    }

    private void HighlightTile(bool checkLineOfSight, bool checkAlliedTile)
    {
        highlightedTiles.ForEach(ht => ht.Highlight(false));
        highlightedTiles.Clear();
        
        List<Tile> tiles = new List<Tile>();

        // Get Main Tile
        Tile highlightedTile = Manager_Grid.Instance.GetTileByWorldPosition(mouseGridPosition);

        if (highlightedTile == null)
            return;

        tiles.Add(highlightedTile);

        //Get Radius tiles

        int currentRadiusCheck = 0;
        while(currentRadiusCheck < selectionRadius - 1 && currentRadiusCheck < 20)
        {
            List<Tile> tiles2 = new List<Tile>();

            for (int i = 0; i < tiles.Count; i++)
            {
                List<Tile> surroundingTiles = new List<Tile>();

                surroundingTiles = Manager_Grid.Instance.GetSurroundingTiles(tiles[i], true);

                foreach (Tile tile in surroundingTiles)
                {
                    if (!tiles2.Contains(tile))
                    {
                        tiles2.Add(tile);
                    }
                }
            }

            foreach (Tile tile in tiles2)
            {
                if (!tiles.Contains(tile))
                {
                    tiles.Add(tile);
                }
            }

            currentRadiusCheck++;
        }

        if (checkLineOfSight)
        {
            List<Tile> tilesInLineOfSight = new List<Tile>();

            foreach (Tile tile in tiles)
            {
                if (Manager_Grid.Instance.HasLineOfSight(highlightedTile, tile))
                    tilesInLineOfSight.Add(tile);
            }

            tiles = tilesInLineOfSight;
        }        

        tiles.ForEach(st => st.Highlight(true));

        highlightedTiles.AddRange(tiles);
    }

    private void MovementHighlight(Tile tile)
    {
        List<Tile> tiles = new List<Tile>();

        tiles.Add(tile);

        int movement = tile.WorldCharacter.Character.Movement;

        int currentRadiusCheck = 0;
        while (currentRadiusCheck < movement && currentRadiusCheck < 50)
        {
            List<Tile> tiles2 = new List<Tile>();

            for (int i = 0; i < tiles.Count; i++)
            {
                List<Tile> surroundingTiles = new List<Tile>();

                surroundingTiles = Manager_Grid.Instance.GetSurroundingAlliedTile(tiles[i], true, tile.WorldCharacter.Character.IsPlayerTeam);

                foreach (Tile surroundingTile in surroundingTiles)
                {
                    if (!tiles2.Contains(surroundingTile))
                    {
                        tiles2.Add(surroundingTile);
                    }
                }
            }

            foreach (Tile tile2 in tiles2)
            {
                if (!tiles.Contains(tile2))
                {
                    tiles.Add(tile2);
                }
            }

            currentRadiusCheck++;
        }

        tiles.ForEach(st => st.Movement(true));

        movementTiles.AddRange(tiles);
    }

    private void ClearHighlights()
    {
        highlightedTiles.ForEach(ht => ht.Highlight(false));
        highlightedTiles.Clear();
    }

    private void ClearSelection()
    {
        selectedtiles.ForEach(st => st.Select(false));
        selectedtiles.Clear();
    }

    private void ClearMovementTiles()
    {
        movementTiles.ForEach(st => st.Movement(false));
        movementTiles.Clear();
    }

    #endregion

    #region Selection

    private void Selection()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile(true, false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            ClearSelection();

            foreach (Tile tile in highlightedTiles)
            {
                tile.Highlight(false);
                tile.Select(true);
                selectedtiles.Add(tile);
            }

            highlightedTiles.Clear();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (selectedtiles.Count > 0)
            {
                ClearSelection();
                return;
            }

            ClearHighlights();
            ClearSelection();

            inputState = InputState.None;
            isLocked = false;
        }
    }

    #endregion

    #region Damage

    private void Damage()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile(true, false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            foreach (Tile tile in highlightedTiles)
            {
                if (tile.WorldCharacter == null)
                    continue;

                tile.WorldCharacter.Damage(damageValue);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearHighlights();
            ClearSelection();

            inputState = InputState.None;
            isLocked = false;
        }
    }

    #endregion

    #region Movement

    private void Movement()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile(false, false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (movementSelection == null)
            {
                Tile movementTile = highlightedTiles.FirstOrDefault();
                Character_World selectedCharacter = movementTile.WorldCharacter;

                if (selectedCharacter != null)
                {
                    movementSelection = selectedCharacter;

                    MovementHighlight(movementTile);
                }                
            }
            else
            {
                Tile movementTile = highlightedTiles.FirstOrDefault();
                if (movementTile.WorldCharacter == null)
                {
                    movementSelection.Move(movementTile);
                    movementSelection = null;
                    ClearSelection();
                    ClearMovementTiles();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (movementSelection != null)
            {
                movementSelection = null;
                ClearSelection();
                ClearMovementTiles();
                return;
            }

            ClearHighlights();
            ClearSelection();
            ClearMovementTiles();

            inputState = InputState.None;
            isLocked = false;
        }
    }

    #endregion

    #region Heal

    private void Heal()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile(true, false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            foreach (Tile tile in highlightedTiles)
            {
                if (tile.WorldCharacter == null)
                    continue;

                tile.WorldCharacter.Heal(healValue);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearHighlights();
            ClearSelection();

            inputState = InputState.None;
            isLocked = false;
        }
    }

    #endregion

    #region Spawning

    public void SetCharacterToSpawn(Character character)
    {
        characterToSpawn = character;
    }

    private void Spawn()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile(false, false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            foreach (Tile tile in highlightedTiles)
            {
                if (tile.WorldCharacter != null)
                    continue;

                Manager_Characters.Instance.SpawnCharacter(characterToSpawn, tile.transform.position);

                if (!Manager_Initative.Instance.IsCharacterInPlay(characterToSpawn.Name) && characterToSpawn.CustomInitiative)
                {
                    StopSpawn();
                    return;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            StopSpawn();
        }
    }

    private void StopSpawn()
    {
        OnStopSpawnRequest?.Invoke();
        characterToSpawn = null;

        ClearHighlights();
        ClearSelection();

        inputState = InputState.None;
        isLocked = false;
    }

#endregion
}

public enum InputState
{
    None = 0,
    Selection = 1,
    Damage = 2,
    Heal = 3,
    Movement = 4,
    Spawn = 5
}