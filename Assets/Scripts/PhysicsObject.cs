using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField] private bool removeTiles = default;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 6)
            return;

        if (!removeTiles)
            return;

        Tile tile = collision.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        Manager_Grid.Instance.RemoveTile(tile);
    }
}
