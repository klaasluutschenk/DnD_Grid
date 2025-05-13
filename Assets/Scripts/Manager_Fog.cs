using UnityEngine;
using System.Linq;
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

    public void RevealRoom(int index)
    {
        FogRoom fogRoom = GetFogRoomByIndex(index);

        if (fogRoom == null)
            return;

        fogRoom.isRevealed = true;
    }

    public bool IsRoomRevealed(int index)
    {
        FogRoom fogRoom = GetFogRoomByIndex(index);

        if (fogRoom == null)
            return true;

        return fogRoom.isRevealed;
    }

    private FogRoom GetFogRoomByIndex(int index)
    {
        return fogRooms.Where(fr => fr.Index == index).FirstOrDefault();
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

                fogRoom.isRevealed = true;
            }
        }
    }
}

[System.Serializable]
public class FogRoom
{
    public int Index;
    public bool RevealAtStart;
    public bool isRevealed;
}