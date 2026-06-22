using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Manager_Characters_2 : MonoBehaviour
{
    public static Manager_Characters_2 Instance;

    public List<Character_World> Characters => characters;

    [SerializeField] private Transform characterContainer = default;
    [SerializeField] private List<CharacterPrefabData> characterPrefabData;

    private List<Character_World> characters = new List<Character_World>();

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine Initialize()
    {
        return StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        CombatEncounter combatEncounter = Manager_Encounter.Instance.CombatEncounter;

        SpawnAllCharacters(combatEncounter.Characters);

        Character_World.OnDeSpawned += OnDespawned;

        Debug.Log("Characters Loaded");

        yield return null;
    }

    private void SpawnAllCharacters(List<CombatEncounter_Character> combatEncounter_Characters)
    {
        foreach (CombatEncounter_Character cc in combatEncounter_Characters)
        {
            SpawnCharacter(cc.Character, cc.Position);
        }
    }

    public void SpawnCharacter(Character character, Vector3 position)
    {
        Tile targetTile = Manager_Grid_2.Instance.GetTileByWorldPosition(position);

        if (targetTile == null)
        {
            return;
        }

        if (!Manager_Grid_2.Instance.RoomAvailable(targetTile, character.EntitySize))
        {
            return;
        }

        Character_World newWorldCharacter = Instantiate(GetCharacterPrefab(character.EntitySize), characterContainer);

        newWorldCharacter.gameObject.name = character.Name;

        newWorldCharacter.Setup(character);

        newWorldCharacter.SetPosition(targetTile);

        characters.Add(newWorldCharacter);

        Manager_Initative.Instance.AddToInitiative(character);
    }

    private void OnDespawned(World_Entity world_Entity)
    {
        Character_World character_World = world_Entity as Character_World;

        if (character_World == null)
        {
            return;
        }

        if (!characters.Contains(character_World))
        {
            return;
        }

        characters.Remove(character_World);
    }

    private Character_World GetCharacterPrefab(EntitySize characterType)
    {
        foreach (CharacterPrefabData prefabData in characterPrefabData)
        {
            if (prefabData.CharacterType == characterType)
            {
                return prefabData.Prefab;
            }
        }

        Debug.LogError("There is no Character Prefab for this entry!");
        return null;
    }

    public List<Character_World> GetPlayerCharacters()
    {
        return characters.Where(c => c.Character.IsPlayer).ToList();
    }
}

[System.Serializable]
public class CharacterPrefabData
{
    public EntitySize CharacterType;
    public Character_World Prefab;
}

public enum EntitySize
{
    Default = 0,
    Small = 1,
    Big = 2,
    Giant = 3,
    Player
}