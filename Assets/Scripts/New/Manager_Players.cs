using UnityEngine;
using System.Collections;

public class Manager_Players : MonoBehaviour
{
    public static Manager_Players Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine RunPlayers()
    {
        return StartCoroutine(RunPlayersRoutine());
    }

    private IEnumerator RunPlayersRoutine()
    {
        Debug.Log("Players Loaded");

        yield return null;
    }
}
