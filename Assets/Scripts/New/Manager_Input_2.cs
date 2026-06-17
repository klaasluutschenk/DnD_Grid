using UnityEngine;
using System.Collections;

public class Manager_Input_2 : MonoBehaviour
{
    public static Manager_Input_2 Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine RunInput()
    {
        return StartCoroutine(RunInputRoutine());
    }

    private IEnumerator RunInputRoutine()
    {
        yield return new WaitForSeconds(1);

        Debug.Log("Input Loaded");
    }
}
