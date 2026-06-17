using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    #region Old

    public World_Entity World_Entity => worldEntity;
    public bool IsSelected => isSelected;
    public bool IsHiglighted => isHighlighted;
    public bool IsMovement => isMovement;
    public Vector2 GridPosition => gridPosition;
    public Transform DetectionCube => transform_detectionCube;
    public int RoomIndex => roomIndex;
    public bool IsRevealed => isRevealed;
    public bool HasWorldEntity => worldEntity != null;

    [SerializeField] private GameObject gameObject_Selected = default;
    [SerializeField] private GameObject gameObject_Movement = default;
    [SerializeField] private Transform transform_detectionCube = default;

    [SerializeField] private GameObject gameObject_Fog = default;
    [SerializeField] private GameObject gameObject_Fog_Highlight = default;

    private World_Entity worldEntity;

    private bool isSelected = false;
    private bool isHighlighted = false;
    private bool isMovement = false;

    private bool isRevealed = false;
    private bool isFogHighlighted = false;

    private int roomIndex;

    private Vector2 gridPosition;

    public void Setup(Vector2 gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public void SetWorldEntity(World_Entity worldEntity)
    {
        this.worldEntity = worldEntity;

        RevealWorldEntity(isRevealed);
    }

    public void ClearEntity()
    {
        worldEntity = null;
    }

    public void SetFogRoomIndex(int fogRoomIndex)
    {
        this.roomIndex = fogRoomIndex;
    }

    public void Select(bool temp)
    {
        OnTileClicked?.Invoke(this);
    }

    public void Movement(bool isMovement)
    {
        if (this.isMovement == isMovement)
            return;

        this.isMovement = isMovement;

        gameObject_Movement.SetActive(isMovement);
    }

    public void Reveal(bool isRevealed)
    {
        if (isRevealed == this.isRevealed)
            return;

        this.isRevealed = isRevealed;

        gameObject_Fog.SetActive(!isRevealed);

        RevealWorldEntity(isRevealed);
    }

    public void FogHighlight(bool isFogHighlighted)
    {
        if (isFogHighlighted == this.isFogHighlighted)
            return;

        this.isFogHighlighted = isFogHighlighted;

        gameObject_Fog_Highlight.SetActive(isFogHighlighted);
    }

    private void RevealWorldEntity(bool reveal)
    {
        if (worldEntity == null)
            return;

        if (reveal)
            worldEntity.Reveal();
        else
            worldEntity.Hide();
    }

    #endregion

    #region new

    public static Action<Tile> OnTileHighlighted;
    public static Action<Tile> OnTileClicked;

    [SerializeField] private GameObject gameObject_Highlighted = default;

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseDown()
    {
        Select();
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    public void Select()
    {
        OnTileClicked?.Invoke(this);
    }

    public void Highlight(bool isHighlighted)
    {
        gameObject_Highlighted.SetActive(isHighlighted);

        OnTileHighlighted?.Invoke(this);
    }

    #endregion
}
