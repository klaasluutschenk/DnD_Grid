using UnityEngine;

public class FogMaker : MonoBehaviour
{
    [SerializeField] private FogRoom fogRoom = default;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Start()
    {
        Manager_Fog.Instance.AddFogRoom(fogRoom);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 6)
            return;

        Tile tile = collision.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        tile.SetFogRoomIndex(fogRoom.Index);
    }
}
