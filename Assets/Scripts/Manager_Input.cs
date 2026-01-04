using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using F10.StreamDeckIntegration;
using F10.StreamDeckIntegration.Attributes;

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

    [SerializeField] private Texture2D cursor_Movement = default;
    [SerializeField] private Texture2D cursor_Burn = default;
    [SerializeField] private Texture2D cursor_Venom = default;
    [SerializeField] private Texture2D cursor_Bleed = default;

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

    private void OnEnable()
    {
        StreamDeck.Add(this);
    }

    private void OnDisable()
    {
        StreamDeck.Remove(this);
    }

    #region UI Interaction

    //private void OnSelectionClicked()
    //{
    //    if (isLocked)
    //        return;

    //    inputState = InputState.Selection;
    //    isLocked = true;
    //}

    //private void OnSelectionChanged(string value)
    //{
    //    selectionRadius = int.Parse(value);
    //}


    //private void OnSpawnedClicked()
    //{
    //    if (isLocked)
    //        return;

    //    selectionRadius = 1;
    //    inputState = InputState.Spawn;
    //    isLocked = true;

    //    OnSpawnRequest?.Invoke();
    //}

    //private void OnImpactClicked()
    //{
    //    PhysicsObject_Destructable.OnHighlighted += HighLightDestructable;
    //    PhysicsObject_Destructable.OnStopHighlighted += StopHighLightDestructable;
    //    inputState = InputState.Impact;
    //    isLocked = true;
    //}

    #endregion

    private void Update()
    {
        SetCursor();

        SetMousePosition();

        if (HasMouseChanged())
        {
            HighlightTile(true, false);
        }

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

        // Fog

        if (!mainTile.IsRevealed)
        {
            Fog();
            return;
        }
        else
            ClearFogtiles();

        // Movement

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Movement();
            inputState = InputState.Movement;
            return;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ClearMovement(true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
            ToggleBurn();

        if (Input.GetKeyDown(KeyCode.V))
            ToggleVenom();

        if (Input.GetKeyDown(KeyCode.B))
            ToggleBleed();

        // Negative Effect

        if (Input.GetMouseButtonDown(0))
        {
            switch (InputState)
            {
                case InputState.Default:
                    Damage();
                    break;
                case InputState.Burn:
                    ApplyBurn();
                    break;
                case InputState.Venom:
                    ApplyVenom();
                    break;
                case InputState.Bleed:
                    ApplyBleed();
                    break;
                case InputState.Slow:
                    break;
                case InputState.Stun:
                    break;
                case InputState.Blind:
                    break;
            }
            return;
        }

        // Positive Effect

        if (Input.GetMouseButtonDown(1))
        {
            switch (InputState)
            {
                case InputState.Default:
                    Heal();
                    break;
                case InputState.Burn:
                    CleanseBurn();
                    break;
                case InputState.Venom:
                    CleanseVenom();
                    break;
                case InputState.Bleed:
                    break;
                case InputState.Slow:
                    break;
                case InputState.Stun:
                    break;
                case InputState.Blind:
                    break;
            }
            return;
        }
    }

    #region StreamDeck Commands

    [StreamDeckButton("ToggleBurn")]
    public void ToggleBurn()
    {
        if (inputState != InputState.Burn)
            inputState = InputState.Burn;
        else
            inputState = InputState.Default;
    }

    [StreamDeckButton("ToggleVenom")]
    public void ToggleVenom()
    {
        if (inputState != InputState.Venom)
            inputState = InputState.Venom;
        else
            inputState = InputState.Default;
    }

    [StreamDeckButton("ToggleBleed")]
    public void ToggleBleed()
    {
        if (inputState != InputState.Bleed)
            inputState = InputState.Bleed;
        else
            inputState = InputState.Default;
    }

    #endregion

    private void SetCursor()
    {
        // cursor
        switch (InputState)
        {
            case InputState.Default:
                Cursor.SetCursor(
                    null, Vector2.zero, CursorMode.Auto);
                break;
            case InputState.Movement:
                Cursor.SetCursor(
                    cursor_Movement,
                    new Vector2(cursor_Movement.width / 2, cursor_Movement.height / 2),
                    CursorMode.Auto);
                break;
            case InputState.Burn:
                Cursor.SetCursor(
                    cursor_Burn,
                    new Vector2(cursor_Burn.width / 2, cursor_Burn.height / 2),
                    CursorMode.Auto);
                break;
            case InputState.Venom:
                Cursor.SetCursor(
                    cursor_Venom,
                    new Vector2(cursor_Venom.width / 2, cursor_Venom.height / 2),
                    CursorMode.Auto);
                break;
            case InputState.Bleed:
                Cursor.SetCursor(
                    cursor_Bleed,
                    new Vector2(cursor_Bleed.width / 2, cursor_Bleed.height / 2),
                    CursorMode.Auto);
                break;
            case InputState.Slow:
                break;
            case InputState.Stun:
                break;
            case InputState.Blind:
                break;
            case InputState.Armor:
                break;
            case InputState.Barrier:
                break;
            default:
                break;
        }
    }

    #region Mouse

    private void SetMousePosition()
    {
        Vector3 mouseWorldPosition;
        Vector3 relativeMousePosition = Vector3.zero;

#if UNITY_EDITOR
        mouseWorldPosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
#else
        relativeMousePosition = Display.RelativeMouseAt(Input.mousePosition);
        display = (int)relativeMousePosition.z;

        if (display == 0)
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

    private void MovementHighlight(Tile tile, Character_World character_World)
    {
        List<Tile> tiles = new List<Tile>();

        tiles.Add(tile);

        int movement = character_World.Character.Movement;

        int currentRadiusCheck = 0;
        while (currentRadiusCheck < movement && currentRadiusCheck < 50)
        {
            List<Tile> tiles2 = new List<Tile>();

            for (int i = 0; i < tiles.Count; i++)
            {
                List<Tile> surroundingTiles = new List<Tile>();

                surroundingTiles = Manager_Grid.Instance.GetSurroundingAlliedTile(tiles[i], true, character_World.Character.IsPlayerTeam);

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

            inputState = InputState.Default;
        }
    }

    #endregion

    #region Effects

    private void Damage()
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (character_World == null)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
            character_World.Damage(5);
        else
            character_World.Damage(1);
    }

    private void Heal(int healValue = 1)
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (character_World == null)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
            character_World.Heal(5);
        else
            character_World.Heal(1);
    }

    private void ApplyBurn()
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (character_World == null)
            return;

        character_World.ApplyBurn();
    }

    private void CleanseBurn()
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (character_World == null)
            return;

        character_World.CleanseBurn();
    }

    private void ApplyVenom()
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (character_World == null)
            return;

        character_World.ApplyVenom();
    }

    private void CleanseVenom()
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (character_World == null)
            return;

        character_World.CleanseVenom();
    }

    private void ApplyBleed()
    {
        Character_World character_World = mainTile.World_Entity as Character_World;

        if (character_World == null)
            return;

        character_World.ApplyBleed();
    }

    #endregion

    #region Movement

    private void Movement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (movementSelection == null)
            {
                Tile movementTile = highlightedTiles.FirstOrDefault();
                Character_World selectedCharacter = movementTile.World_Entity as Character_World;

                if (selectedCharacter != null)
                {
                    movementSelection = selectedCharacter;

                    MovementHighlight(movementTile, selectedCharacter);
                }                
            }
            else
            {
                Tile movementTile = highlightedTiles.FirstOrDefault();
                if (movementTile.World_Entity == null)
                {
                    movementSelection.Move(movementTile);
                    movementSelection = null;
                    ClearSelection();
                    ClearMovementTiles();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
            ClearMovement();
    }

    private void ClearMovement(bool forceClear = false)
    {
        if (movementSelection != null)
        {
            movementSelection = null;
            ClearSelection();
            ClearMovementTiles();

            if (!forceClear)
                return;
        }

        ClearSelection();
        ClearMovementTiles();

        inputState = InputState.Default;
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
                if (tile.World_Entity != null)
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

        inputState = InputState.Default;
    }

    #endregion

    #region Fog

    private void Fog()
    {
        HighLightFogTiles();

        if (Input.GetMouseButton(0))
        {
            fogTimer += Time.deltaTime;

            if (fogTimer >= 1)
            {
                fogTiles.ForEach(ft => ft.Reveal(true));
                Manager_Fog.Instance.RevealRoom(currentRoomSelection);

                ClearFogtiles();

                fogTimer = 0;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            fogTimer = 0;
        }
    }

    private void HighLightFogTiles()
    {
        ClearFogtiles();

        // Get Main Tile
        Tile highlightedTile = Manager_Grid.Instance.GetTileByWorldPosition(mouseGridPosition);

        if (highlightedTile == null)
            return;

        if (Manager_Fog.Instance.IsRoomRevealed(highlightedTile.RoomIndex))
            return;

        currentRoomSelection = highlightedTile.RoomIndex;

        fogTiles.AddRange(Manager_Grid.Instance.GetTilesByRoomIndex(highlightedTile.RoomIndex));

        fogTiles.ForEach(t => t.FogHighlight(true));
    }

    private void ClearFogtiles()
    {
        fogTiles.ForEach(ht => ht.FogHighlight(false));
        fogTiles.Clear();

        currentRoomSelection = -1;
    }

    #endregion

    #region Impact

    private PhysicsObject_Destructable target_physicsObject_Destructable;
    private void Impact()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (target_physicsObject_Destructable == null)
                return;

            bool isDestroyed = target_physicsObject_Destructable.Damage(damageValue);

            if (isDestroyed)
                ClearHighlightDestructable();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearHighlightDestructable();

            PhysicsObject_Destructable.OnHighlighted -= HighLightDestructable;
            PhysicsObject_Destructable.OnStopHighlighted -= StopHighLightDestructable;

            inputState = InputState.Default;
        }
    }

    private void HighLightDestructable(PhysicsObject_Destructable target_physicsObject_Destructable)
    {
        this.target_physicsObject_Destructable = target_physicsObject_Destructable;
        this.target_physicsObject_Destructable.HighLight();
    }

    private void StopHighLightDestructable(PhysicsObject_Destructable target_physicsObject_Destructable)
    {
        if (this.target_physicsObject_Destructable == target_physicsObject_Destructable)
        {
            ClearHighlightDestructable();
        }
    }

    private void ClearHighlightDestructable()
    {
        if (target_physicsObject_Destructable == null)
            return;

        target_physicsObject_Destructable.SetDefault();
        target_physicsObject_Destructable = null;
    }

    #endregion
}

public enum InputState
{
    Default = 0,
    Movement = 1,
    Burn = 2,
    Venom = 3,
    Bleed = 4,
    Slow = 5,
    Stun = 6,
    Blind = 7,
    Armor = 8,
    Barrier = 9
}

//Selection = 1,
//Damage = 2,
//Heal = 3,
//Movement = 4,
//Spawn = 5,
//Fog = 6,
//Impact = 7,
//Burn = 8,
//Venom = 9,
//Bleed = 10,
//CleanseBurn = 11,
//CleanseVenom = 12,
//Tooltip = 13