using UnityEngine;
using System.Collections;

public class Manager_Players : MonoBehaviour
{
    public static Manager_Players Instance;

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
        Debug.Log("Players Loaded");

        yield return null;
    }
}
