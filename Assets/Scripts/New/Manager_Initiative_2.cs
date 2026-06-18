using UnityEngine;
using System.Collections;

public class Manager_Initiative_2 : MonoBehaviour
{
    public static Manager_Initiative_2 Instance;

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
        Debug.Log("Initiative Loaded");

        yield return null;
    }
}
