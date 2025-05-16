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
    public Tile Tile => tile;
    public Entity Entity => entity;

    [SerializeField] protected Image image_Sprite;
    [SerializeField] protected GameObject gameObject_Canvas = default;

    protected Entity entity;

    protected Tile tile;

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

        image_Sprite.enabled = entity.Sprite;

        tile = Manager_Grid.Instance.GetTileByWorldPosition(transform.position);
        tile.SetWorldEntity(this);
    }

    public virtual void Remove()
    {
        OnDeSpawned?.Invoke(this);
        tile.ClearEntity();

        Destroy(this.gameObject);
    }

    #region Revealing & Hiding

    public virtual void Reveal()
    {
        if (isRevealed)
            return;

        gameObject_Canvas.SetActive(true);
        isRevealed = true;
    }

    public void Hide()
    {
        gameObject_Canvas.SetActive(false);
        isRevealed = false;
    }

    #endregion

    #region Movement

    public void Move(Tile targetTile)
    {
        if (tile != null)
            tile.SetWorldEntity(null);

        tile = targetTile;

        transform.position = tile.transform.position;

        tile.SetWorldEntity(this);
    }

    #endregion
}
