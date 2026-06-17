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
        yield return new WaitForSeconds(1);

        Debug.Log("Initiative Loaded");
    }
}
