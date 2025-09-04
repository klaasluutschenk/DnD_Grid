using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    protected virtual bool RemoveTiles => removeTiles;
    protected virtual bool ShowVisual => false;

    [SerializeField] private bool removeTiles = default;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = ShowVisual;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 6)
            return;

        if (!RemoveTiles)
            return;

        Tile tile = collision.gameObject.GetComponent<Tile>();

        if (tile == null)
            return;

        Manager_Grid.Instance.RemoveTile(tile);
    }
}
