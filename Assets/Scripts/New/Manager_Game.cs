using UnityEngine;
using System.Collections;

public class Manager_Game : MonoBehaviour
{
    public static Manager_Game Instance;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return Manager_Encounter.Instance.RunEncounter();

        yield return Manager_Environment.Instance.RunEnvironment();

        yield return Manager_Grid_2.Instance.RunGrid();

        yield return Manager_Initiative_2.Instance.RunInitiative();

        yield return Manager_Players.Instance.RunPlayers();

        yield return Manager_Characters_2.Instance.RunCharacters();

        yield return Manager_Input_2.Instance.RunInput();

        yield return Manager_Cursor.Instance.RunCursor();

        Debug.Log("Loading Completed!");
    }
}
