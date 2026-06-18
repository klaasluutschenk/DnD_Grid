using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager_Cursor : MonoBehaviour
{
    public static Manager_Cursor Instance;

    [SerializeField] private List<CursorData> cursorData = default;

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
        Debug.Log("Cursor Loaded");

        yield return null;
    }

    public void ResetCursor()
    {
        SetCursor(InputState.Default);
    }

    public void SetCursor(InputState inputState)
    {
        if (!HasCursorChange(inputState))
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            return;
        }

        Texture2D cursorTexture = GetCursorTexture(inputState);

        Cursor.SetCursor(
            cursorTexture,
            new Vector2(cursorTexture.width / 2, cursorTexture.height / 2),
            CursorMode.Auto);
    }

    private Texture2D GetCursorTexture(InputState inputState)
    {
        foreach (CursorData cursorDataEntry in cursorData)
        {
            if (cursorDataEntry.InputState == inputState)
            {
                return cursorDataEntry.CursorTexture;
            }
        }

        return null;
    }

    private bool HasCursorChange(InputState inputState)
    {
        switch (inputState)
        {
            case InputState.Default:
                return false;
            case InputState.Initiative:
                return false;
            case InputState.Spawning:
                return false;
        }

        return true;
    }
}

[System.Serializable]
public class CursorData
{
    public InputState InputState;
    public Texture2D CursorTexture;
}
