using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Manager_Grid_2 : MonoBehaviour
{
    public static Manager_Grid_2 Instance;

    [SerializeField] private Tile tilePrefab = default;

    [SerializeField] private Transform physicsContainer = default;
    [SerializeField] private Transform gridContainer = default;
    [SerializeField] private GameObject tempGrid = default;

    [SerializeField] private LayerMask layerMask = default;

    private List<Tile> tiles = new List<Tile>();

    private GameObject physicsObject;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine RunGrid()
    {
        return StartCoroutine(RunGridRoutine());
    }

    private IEnumerator RunGridRoutine()
    {
        tempGrid.SetActive(false);

        CombatEncounter combatEncounter = Manager_Encounter.Instance.CombatEncounter;

        SpawnPhysics(combatEncounter.PhysicsObject);

        GenerateGrid(combatEncounter.Background, combatEncounter.BackgroundSizeModifier);

        SetNeighbours();

        yield return null;

        ClearPhysicsObject();

        Debug.Log("Grid Loaded");
    }

    private void SpawnPhysics(GameObject encounterPhysicsObject)
    {
        physicsObject = Instantiate(encounterPhysicsObject, physicsContainer);
    }

    private void ClearPhysicsObject()
    {
        Destroy(physicsObject);
        physicsContainer.gameObject.SetActive(false);
        physicsObject = null;
    }

    private void GenerateGrid(Sprite background, int backgroundSizeModifier)
    {
        int gridWidth = (int)background.rect.width * backgroundSizeModifier / 100;
        int gridHeight = (int)background.rect.height * backgroundSizeModifier / 100;

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

                Debug.LogError("Remove this auto Revealing!");
                newTile.Reveal(true);

                tiles.Add(newTile);
            }
        }
    }

    public void RemoveTile(Tile tile)
    {
        if (!tiles.Contains(tile))
            return;

        tiles.Remove(tile);
        Destroy(tile.gameObject);
    }

    private void SetNeighbours()
    {
        foreach (Tile tile in tiles)
        {
            tile.SetNeighbours();
        }
    }

    public Tile GetTileByWorldPosition(Vector3 position)
    {
        return tiles.Where(w => w.transform.position == position).ToList().FirstOrDefault();
    }

    public Tile GetTileByGridPosition(Vector2 gridPosition)
    {
        return tiles.Where(w => w.GridPosition == gridPosition).ToList().FirstOrDefault();
    }

    public Tile GetNeighbour(Tile tile, Vector2 direction)
    {
        Tile neighbour = GetTileByGridPosition(new Vector2(tile.GridPosition.x + direction.x, tile.GridPosition.y + direction.y));

        if (neighbour != null)
        {
            if (!HasLineOfSight(tile, neighbour, direction))
            {
                return null;
            }
        }

        return neighbour;
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
}
