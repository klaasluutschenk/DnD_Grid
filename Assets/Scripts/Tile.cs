using UnityEngine;

public class Tile : MonoBehaviour
{
    public Character Character => character;

    private Character character;

    public void SetCharacter(Character character)
    {
        this.character = character;
    }

    public void ClearCharacter()
    {
        character = null;
    }
}
