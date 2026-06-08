using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float horizontalSpeed = default;
    [SerializeField] private float verticalSpeed = default;

    [SerializeField] private RawImage rawImage = default;

    void Update()
    {
        Rect currentUvRect = rawImage.uvRect;

        currentUvRect.x += horizontalSpeed * Time.deltaTime;
        currentUvRect.y += verticalSpeed * Time.deltaTime;

        rawImage.uvRect = currentUvRect;

        if (Input.GetKey(KeyCode.N))
            horizontalSpeed = 0.05f;

        if (Input.GetKey(KeyCode.M))
            horizontalSpeed = 0.1f;

        if (Input.GetKey(KeyCode.B))
            horizontalSpeed = 0f;
    }
}
