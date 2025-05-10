using UnityEngine;

public class Tile : MonoBehaviour
{
    public Character_World WorldCharacter => worldCharacter;
    public bool IsSelected => isSelected;
    public bool IsHiglighted => isHighlighted;
    public Vector2 GridPosition => gridPosition;

    [SerializeField] private GameObject gameObject_Selected = default;
    [SerializeField] private GameObject gameObject_Highlighted = default;

    private Character_World worldCharacter;

    private bool isSelected = false;
    private bool isHighlighted = false;

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
}
