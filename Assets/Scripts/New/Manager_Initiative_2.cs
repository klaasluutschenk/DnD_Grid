using UnityEngine;
using System.Collections;

public class Manager_Initiative_2 : MonoBehaviour
{
    public static Manager_Initiative_2 Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine RunInitiative()
    {
        return StartCoroutine(RunInitiativeRoutine());
    }

    private IEnumerator RunInitiativeRoutine()
    {
        Debug.Log("Initiative Loaded");

        yield return null;
    }
}
