using UnityEngine;

public class Tile : MonoBehaviour
{
    public Character_World WorldCharacter => worldCharacter;
    public bool IsSelected => isSelected;
    public bool IsHiglighted => isHighlighted;
    public bool IsMovement => isMovement;
    public Vector2 GridPosition => gridPosition;
    public Transform DetectionCube => transform_detectionCube;

    public int RoomIndex;

    [SerializeField] private GameObject gameObject_Selected = default;
    [SerializeField] private GameObject gameObject_Highlighted = default;
    [SerializeField] private GameObject gameObject_Movement = default;
    [SerializeField] private Transform transform_detectionCube = default;

    private Character_World worldCharacter;

    private bool isSelected = false;
    private bool isHighlighted = false;
    private bool isMovement = false;

    private Vector2 gridPosition;

    public void Setup(Vector2 gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public void SetWorldCharacter(Character_World worldCharacter)
    {
        this.worldCharacter = worldCharacter;
    }

    public void ClearCharacter()
    {
        worldCharacter = null;
    }

    public void Select(bool isSelected)
    {
        if (this.isSelected == isSelected)
            return;

        this.isSelected = isSelected;

        gameObject_Selected.SetActive(isSelected);
    }

    public void Highlight(bool isHighlighted)
    {
        if (this.isHighlighted == isHighlighted)
            return;

        this.isHighlighted = isHighlighted;

        gameObject_Highlighted.SetActive(isHighlighted);
    }

    public void Movement(bool isMovement)
    {
        if (this.isMovement == isMovement)
            return;

        this.isMovement = isMovement;

        gameObject_Movement.SetActive(isMovement);
    }
}
