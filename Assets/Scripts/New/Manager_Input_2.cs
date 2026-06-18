using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager_Input_2 : MonoBehaviour
{
    public static Manager_Input_2 Instance;

    private List<Tile> movementTiles = new List<Tile>();
    private Character_World movementCharacter;

    private InputState inputState = InputState.Default;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine RunInput()
    {
        return StartCoroutine(RunInputRoutine());
    }

    private IEnumerator RunInputRoutine()
    {
        Debug.Log("Input Loaded");

        yield return null;
    }

    private void Update()
    {
        Controlls();
    }

    private void ToggleInputState(InputState newInputState)
    {
        if (inputState != newInputState)
            inputState = newInputState;
        else
            inputState = InputState.Default;

        Manager_Cursor.Instance.SetCursor(inputState);
    }

    private void ResetInputState()
    {
        inputState = InputState.Default;
        Manager_Cursor.Instance.ResetCursor();
    }

    private void Controlls()
    {
        // Movement

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleInputState(InputState.Movement);

            Tile.OnTileClicked += OnTileClicked_Movement;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Tile.OnTileClicked -= OnTileClicked_Movement;
            ResetInputState();
            ClearMovementTiles();
        }

        if (Input.GetMouseButtonDown(1) && inputState == InputState.Movement)
        {
            ClearMovementTiles();
        }
    }

    private void OnTileClicked_Movement(Tile tile)
    {
        if (movementCharacter == null)
        {
            SetNewMovement(tile);
            return;
        }

        ActivateMovement(tile);
    }

    private void SetNewMovement(Tile tile)
    {
        if (tile.World_Entity == null)
        {
            return;
        }

        Character_World character_World = (Character_World)tile.World_Entity;

        if (character_World == null)
        {
            return;
        }

        ClearMovementTiles();

        movementCharacter = character_World;
        movementCharacter.Tiles.ForEach(t => t.HighlightSelection(true));

        List<Tile> totalTiles = new List<Tile>();

        for (int tileIndex = 0; tileIndex < character_World.Tiles.Count; tileIndex++)
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(character_World.Tiles[tileIndex]);

            int movement = character_World.GetMovement();

            int currentRadiusCheck = 0;
            while (currentRadiusCheck < movement && currentRadiusCheck < 50)
            {
                List<Tile> tiles2 = new List<Tile>();

                for (int i = 0; i < tiles.Count; i++)
                {
                    List<Tile> surroundingTiles = Manager_Grid_2.Instance.GetSurroundingAlliedTile(tiles[i], character_World.Character.IsPlayerTeam);

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

            totalTiles.AddRange(tiles);
        }

        for (int i = totalTiles.Count - 1; i > 0; i--)
        {
            if (totalTiles[i].World_Entity != null)
            {
                totalTiles.RemoveAt(i);
            }
        }

        totalTiles.ForEach(st => st.Movement(true));

        movementTiles.AddRange(totalTiles);
    }

    private void ActivateMovement(Tile tile)
    {
        if (!Manager_Grid_2.Instance.RoomAvailable(tile, movementCharacter.Character.EntitySize, movementCharacter))
        {
            return;
        }

        movementCharacter.Tiles.ForEach(t => t.HighlightSelection(false));
        movementCharacter.SetPosition(tile);

        ClearMovementTiles();
        ClearMovementCharacter();
    }

    private void ClearMovementTiles()
    {
        movementTiles.ForEach(st => st.Movement(false));
        movementTiles.Clear();
        ClearMovementCharacter();
    }

    private void ClearMovementCharacter()
    {
        if (movementCharacter == null)
        {
            return;
        }

        movementCharacter.Tiles.ForEach(t => t.HighlightSelection(false));
        movementCharacter = null;
    }
}