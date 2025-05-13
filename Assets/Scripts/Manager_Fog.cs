using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager_Fog : MonoBehaviour
{
    public static Manager_Fog Instance;

    [SerializeField] private List<FogRoom> fogRooms = new List<FogRoom>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddFogRoom(FogRoom fogRoom)
    {
        foreach (FogRoom myFogRoom in fogRooms)
        {
            if (myFogRoom.Index == fogRoom.Index)
                return;
        }

        fogRooms.Add(fogRoom);
    }

    private IEnumerator Start()
    {
        // Wait a few frame to give the grid time to generate.
        yield return null;
        yield return null;
        yield return null;

        foreach (FogRoom fogRoom in fogRooms)
        {
            if (fogRoom.RevealAtStart)
            {
                List<Tile> tile = Manager_Grid.Instance.GetTilesByRoomIndex(fogRoom.Index);

                tile.ForEach(t => t.Reveal(true));
            }
        }
    }
}

[System.Serializable]
public class FogRoom
{
    public int Index;
    public bool RevealAtStart;

    public FogRoom(int index, bool revealAtStart)
    {
        Index = index;
        RevealAtStart = revealAtStart;
    }
}