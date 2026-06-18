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
        yield return Manager_Encounter.Instance.Initialize();

        yield return Manager_Environment.Instance.Initialize();

        yield return Manager_Grid_2.Instance.Initialize();

        yield return Manager_Initative.Instance.Initialize();

        yield return Manager_Players.Instance.Initialize();

        yield return Manager_Characters_2.Instance.Initialize();

        yield return Manager_Input_2.Instance.Initialize();

        yield return Manager_Cursor.Instance.Initialize();

        yield return Manager_Camera.Instance.Initialize();

        yield return Manager_Tooltip.Instance.Initialize();

        Debug.Log("Loading Completed!");
    }
}
