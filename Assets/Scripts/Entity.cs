using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Scriptable Objects/Entity")]
public class Entity : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;

    public Sprite Sprite;
}
