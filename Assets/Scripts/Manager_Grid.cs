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

    public Tile GetTileByWorldPosition(Vector3 position)
    {
        return tiles.Where(w => w.transform.position == position).FirstOrDefault();
    }

    #endregion
}
