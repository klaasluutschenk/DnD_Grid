using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance;

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

    private Character_World movementSelection;

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

    private void HighlightTile()
    {
        highlightedTiles.ForEach(ht => ht.Highlight(false));
        highlightedTiles.Clear();

        Tile highlightedTile = Manager_Grid.Instance.GetTileByWorldPosition(mouseGridPosition);

        if (highlightedTile == null)
            return;

        highlightedTile.Highlight(true);

        highlightedTiles.Add(highlightedTile);
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

    #endregion

    #region Selection

    private void Selection()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile();
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
            HighlightTile();
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
            HighlightTile();
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
                    movementTile.Select(true);
                    selectedtiles.Add(movementTile);
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
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (movementSelection != null)
            {
                movementSelection = null;
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

    #region Heal

    private void Heal()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile();
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

    private bool blockSpawn = true;

    private void Spawn()
    {
        SetMousePosition();
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