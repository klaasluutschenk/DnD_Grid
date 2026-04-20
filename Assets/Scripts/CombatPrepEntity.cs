using UnityEngine;

public class CombatPrepEntity : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        if (entity == null)
            return;

        SetEntity(entity);
    }

    public void SetEntity(Entity entity)
    {
        this.entity = entity;

        name = entity.Name;

        if (entity.Sprite != null)
            spriteRenderer.sprite = entity.Sprite;
    }

    public CombatEncounter_Entity GetCombatPrepEntityData()
    {
        if (entity == null)
            return null;

        return new CombatEncounter_Entity(entity, new Vector3(transform.position.x, transform.position.y, -1));
    }
}
