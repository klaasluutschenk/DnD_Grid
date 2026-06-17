using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager_Input_2 : MonoBehaviour
{
    public static Manager_Input_2 Instance;

    private List<Tile> movementTiles = new List<Tile>();

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
        Tile.OnTileClicked += OnTileClicked;
        Debug.Log("Input Loaded");

        yield return null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClearMovementTiles();
        }
    }

    private void OnTileClicked(Tile tile)
    {
        ClearMovementTiles();

        List<Tile> tiles = new List<Tile>();
        tiles.Add(tile);

        int movement = 6;

        int currentRadiusCheck = 0;
        while (currentRadiusCheck < movement && currentRadiusCheck < 50)
        {
            List<Tile> tiles2 = new List<Tile>();

            for (int i = 0; i < tiles.Count; i++)
            {
                List<Tile> surroundingTiles = new List<Tile>();

                surroundingTiles = tiles[i].Neigbours;

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

    private void ClearMovementTiles()
    {
        movementTiles.ForEach(st => st.Movement(false));
        movementTiles.Clear();
    }
}