using UnityEngine;
using System.Collections;

public class Manager_Characters_2 : MonoBehaviour
{
    public static Manager_Characters_2 Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Coroutine RunCharacters()
    {
        return StartCoroutine(RunCharactersRoutine());
    }

    private IEnumerator RunCharactersRoutine()
    {
        yield return new WaitForSeconds(1);

        Debug.Log("Characters Loaded");
    }
}
