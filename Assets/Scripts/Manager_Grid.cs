using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class Manager_Grid : MonoBehaviour
{
    public static Manager_Grid Instance;

    public static Action OnGridGenerated;

    public bool IsGridGenerated => isGridGenerated;

    [SerializeField] private Tile tilePrefab = default;

    [SerializeField] private Transform physicsContainer = default;
    [SerializeField] private Transform gridContainer = default;

    [SerializeField] private LayerMask layerMask = default;

    private bool isGridGenerated;

    private List<Tile> tiles = new List<Tile>();

    private GameObject physicsObject;

    private void Awake()
    {
        Instance = this;

        Manager_Combat.OnCombatEncounterEnded += OnCombatEncounterEnded;
        Manager_Combat.OnGridRequest += OnGridRequest;
    }

    private void OnCombatEncounterEnded()
    {
        ClearPhysics();
        ClearGrid();
    }

    private void OnGridRequest(CombatEncounter combatEncounter)
    {
        SpawnPhysics(combatEncounter.PhysicsObject);
        GenerateGrid(combatEncounter.Background);
    }

    #region Grid

    private void ClearGrid()
    {
        tiles.ForEach(t => Destroy(t));
        tiles.Clear();

        isGridGenerated = false;
    }

    private void GenerateGrid(Sprite sprite)
    {
        int gridWidth = (int)sprite.rect.width / 100;
        int gridHeight = (int)sprite.rect.height / 100;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile newTile = Instantiate(tilePrefab, gridContainer);

                float xPos = (-gridWidth / 2) + x + 0.5f;
                float yPos = (-gridHeight / 2) + y + 0.5f;

                newTile.Setup(new Vector2(x, y));

                newTile.transform.position = new Vector3(xPos, yPos, -1);

                newTile.gameObject.name = $"Tile {x} {y}";

                tiles.Add(newTile);
            }
        }

        isGridGenerated = true;
    }

    #endregion

    #region Physics

    private void ClearPhysics()
    {
        Destroy(physicsObject);
        physicsObject = null;
    }
    
    private void SpawnPhysics(GameObject physicsObjectToSpawn)
    {
        physicsObject = Instantiate(physicsObjectToSpawn, physicsContainer);
    }

    #endregion

    #region Functions

    public void RemoveTile(Tile tile)
    {
        if (!tiles.Contains(tile))
            return;

        tiles.Remove(tile);
        Destroy(tile.gameObject);
    }

    public List<Tile> GetTilesByRoomIndex(int roomIndex)
    {
        return tiles.Where(t => t.RoomIndex == roomIndex).ToList();
    }

    public Tile GetTileByWorldPosition(Vector3 position)
    {
        return tiles.Where(w => w.transform.position == position).FirstOrDefault();
    }

    public Tile GetTileByGridPosition(Vector2 gridPosition)
    {
        return tiles.Where(w => w.GridPosition == gridPosition).FirstOrDefault();
    }

    public Tile GetSelectedTiles()
    {
        return tiles.Where(w => w.IsSelected).FirstOrDefault();
    }

    public Tile GetHighlightedTiles()
    {
        return tiles.Where(w => w.IsHiglighted).FirstOrDefault();
    }

    public List<Tile> GetSurroundingAlliedTile(Tile tile, bool lineOfSight, bool IsPlayerTeam)
    {
        List<Tile> surroundingTiles = GetSurroundingTiles(tile, lineOfSight);
        List<Tile> alliedTiles = new List<Tile>();

        foreach (Tile surroundingTile in surroundingTiles)
        {
            if (surroundingTile.WorldCharacter == null)
            {
                alliedTiles.Add(surroundingTile);
            }
            else if (surroundingTile.WorldCharacter.Character.IsPlayerTeam == IsPlayerTeam)
            {
                alliedTiles.Add(surroundingTile);
            }
        }

        return alliedTiles;
    }

    public List<Tile> GetSurroundingTiles(Tile tile, bool lineOfSight = false)
    {
        List<Tile> surroundingTiles = new List<Tile>();

        Tile UpTile = GetTileByGridPosition(new Vector2(tile.GridPosition.x, tile.GridPosition.y + 1));
        
        if (UpTile != null)
        {
            if (lineOfSight)
            {
                if (HasLineOfSight(tile, UpTile, transform.up))
                {
                    surroundingTiles.Add(UpTile);
                }
            }
            else
                surroundingTiles.Add(UpTile);
        }

        Tile downTile = GetTileByGridPosition(new Vector2(tile.GridPosition.x, tile.GridPosition.y - 1));

        if (downTile != null)
        {
            if (lineOfSight)
            {
                if (HasLineOfSight(tile, downTile, -transform.up))
                {
                    surroundingTiles.Add(downTile);
                }
            }
            else
                surroundingTiles.Add(downTile);
        }

        Tile LeftTile = GetTileByGridPosition(new Vector2(tile.GridPosition.x - 1, tile.GridPosition.y));

        if (LeftTile != null)
        {
            if (lineOfSight)
            {
                if (HasLineOfSight(tile, LeftTile, -transform.right))
                {
                    surroundingTiles.Add(LeftTile);
                }
            }
            else
                surroundingTiles.Add(LeftTile);
        }

        Tile rightTile = GetTileByGridPosition(new Vector2(tile.GridPosition.x + 1, tile.GridPosition.y));

        if (rightTile != null)
        {
            if (lineOfSight)
            {
                if (HasLineOfSight(tile, rightTile, transform.right))
                {
                    surroundingTiles.Add(rightTile);
                }
            }
            else
                surroundingTiles.Add(rightTile);
        }

        return surroundingTiles;
    }

    public bool HasLineOfSight(Tile originTile, Tile targetTile, Vector3 direction)
    {
        float distance = Vector3.Distance(originTile.transform.position, targetTile.transform.position);

        if (Physics.Raycast(originTile.DetectionCube.position, direction, distance, layerMask))
            return false;

        return true;
    }

    public bool HasLineOfSight(Tile originTile, Tile targetTile)
    {
        float distance = Vector3.Distance(originTile.transform.position, targetTile.transform.position);
        Vector3 direction = targetTile.transform.position - originTile.transform.position;

        if (Physics.Raycast(originTile.DetectionCube.position, direction, distance, layerMask))
            return false;

        return true;
    }

    #endregion
}
