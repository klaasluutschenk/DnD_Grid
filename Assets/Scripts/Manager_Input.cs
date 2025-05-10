using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    private int selectionRadius = 1;
    private int damageValue = 1;
    private int healValue = 1;

    public Vector3 mouseGridPosition;
    public Vector3 oldMouseGridPosition;
    private int display;

    private List<Tile> highlightedTiles = new List<Tile>();
    private List<Tile> selectedtiles = new List<Tile>();

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
        inputState = InputState.Selection;
    }

    private void OnSelectionChanged(string value)
    {
        selectionRadius = int.Parse(value);
    }

    private void OnDamageClicked()
    {
        inputState = InputState.Damage;
    }

    private void OnDamageChanged(string value)
    {
        damageValue = int.Parse(value);
    }

    private void OnHealClicked()
    {
        inputState = InputState.Heal;
    }

    private void OnHealChanged(string value)
    {
        healValue = int.Parse(value);
    }

    private void OnMovementClicked()
    {
        inputState = InputState.Movement;
    }

    private void OnSpawnedClicked()
    {
        inputState = InputState.Spawn;
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
                break;
            case InputState.Heal:
                break;
            case InputState.Movement:
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

    #endregion

    #region Selection

    private void Selection()
    {
        SetMousePosition();

        if (HasMouseChanged())
        {
            highlightedTiles.ForEach(ht => ht.Highlight(false));
            highlightedTiles.Clear();

            Tile highlightedTile = Manager_Grid.Instance.GetTileByWorldPosition(mouseGridPosition);

            if (highlightedTile == null)
                return;

            highlightedTile.Highlight(true);

            highlightedTiles.Add(highlightedTile);
        }

        if (Input.GetMouseButtonDown(0))
        {
            selectedtiles.ForEach(st => st.Select(false));
            selectedtiles.Clear();

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
            highlightedTiles.ForEach(ht => ht.Highlight(false));
            highlightedTiles.Clear();

            selectedtiles.ForEach(st => st.Select(false));
            selectedtiles.Clear();

            inputState = InputState.None;
        }
    }

    #endregion

    #region Spawning

    private void Spawn()
    {
        //SetMousePosition();
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