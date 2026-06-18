using UnityEngine;

public class CombatPrepCharacter : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer colorSpriteRenderer;
    [SerializeField] private GameObject playerObject;

    private void OnValidate()
    {
        if (character == null)
            return;

        SetCharacter(character);
    }

    public void SetCharacter(Character character)
    {
        this.character = character;

        string displayName = character.IsPlayer ? $"Player - {character.Name}" : character.Name;

        name = displayName;

        colorSpriteRenderer.color = character.CombatPrepColor;

        spriteRenderer.gameObject.SetActive(!character.IsPlayer);
        playerObject.SetActive(character.IsPlayer);

        if (character.Sprite != null)
            spriteRenderer.sprite = character.Sprite;

        float size = GetSize(character.EntitySize);
        transform.localScale = new Vector3(size, size, size);
    }

    public CombatEncounter_Character GetCombatPrepCharacterData()
    {
        if (character == null)
            return null;

        return new CombatEncounter_Character(character, new Vector3(transform.position.x, transform.position.y, -1));
    }

    private float GetSize(EntitySize entitySize)
    {
        switch (entitySize)
        {
            case EntitySize.Default:
                return 0.8f;
            case EntitySize.Small:
                return 0.8f;
            case EntitySize.Big:
                return 1.8f;
            case EntitySize.Giant:
                return 2.8f;
        }

        return 0.8f;
    }
}