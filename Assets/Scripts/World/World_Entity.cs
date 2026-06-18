using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

public class World_Entity : MonoBehaviour
{
    public static Action<World_Entity> OnSpawned;
    public static Action<World_Entity> OnDeSpawned;

    public bool IsRevealed => isRevealed;
    public List<Tile> Tiles => tiles;
    public Entity Entity => entity;

    [SerializeField] protected Image image_Sprite;
    [SerializeField] protected GameObject gameObject_Canvas = default;

    protected Entity entity;

    protected List<Tile> tiles;

    protected bool isRevealed;

    protected virtual void Awake()
    {
        OnSpawned?.Invoke(this);
    }

    protected virtual void OnDestroy()
    {
        OnDeSpawned?.Invoke(this);
    }

    public virtual void Setup(Entity entity)
    {
        this.entity = entity;

        image_Sprite.enabled = entity.Sprite != null;
        image_Sprite.sprite = entity.Sprite;

        tiles = new List<Tile>();
    }

    public void SetPosition(Tile targetTile)
    {
        ClearPosition();

        transform.position = targetTile.transform.position;

        tiles = Manager_Grid_2.Instance.GetTilesBySize(targetTile, entity.EntitySize);

        tiles.ForEach(t => t.SetWorldEntity(this));
    }
    
    public void ClearPosition()
    {
        if (tiles.Count == 0)
        {
            return;
        }

        tiles.ForEach(t => t.ClearEntity());
        tiles.Clear();
    }

    public virtual void Remove()
    {
        OnDeSpawned?.Invoke(this);
        ClearPosition();

        Destroy(this.gameObject);
    }
}
