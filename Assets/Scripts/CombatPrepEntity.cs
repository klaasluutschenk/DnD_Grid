using UnityEngine;

public class CombatPrepEntity : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private SpriteRenderer spriteRenderer;
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

        spriteRenderer.gameObject.SetActive(!character.IsPlayer);
        playerObject.SetActive(character.IsPlayer);

        if (character.Sprite != null)
            spriteRenderer.sprite = character.Sprite;

        float size = character.IsMinion ? 0.6f : 0.8f;
        transform.localScale = new Vector3(size, size, size);
    }

    public CombatEncounter_Character GetCombatPrepEntityData()
    {
        if (character == null)
            return null;

        return new CombatEncounter_Character(character, new Vector3(transform.position.x, transform.position.y, -1));
    }
}