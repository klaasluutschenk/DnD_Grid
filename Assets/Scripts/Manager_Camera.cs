using UnityEngine;

public class Manager_Camera : MonoBehaviour
{
    public static Manager_Camera Instance;

    [SerializeField] private Transform cameraObject = default;

    [SerializeField] private float minZoom = default;
    [SerializeField] private float maxZoom = default;
    [SerializeField] private float sensitivity = default;

    [SerializeField] private float MoveSensitivity = default;

    private float currentZoomPercentage = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        CameraInput();
    }

    private void CameraInput()
    {
        Moving();
        Zooming();
    }

    private void Moving()
    {
        Vector3 targetPos = cameraObject.transform.position;
        float trueMoveSensitvity = MoveSensitivity * currentZoomPercentage;

        if (Input.GetKey(KeyCode.A))
            targetPos.x -= trueMoveSensitvity * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            targetPos.x += trueMoveSensitvity * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
            targetPos.y += trueMoveSensitvity * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            targetPos.y -= trueMoveSensitvity * Time.deltaTime;

        cameraObject.transform.position = targetPos;
    }

    private void Zooming()
    {
        float currentZoom = Camera.main.orthographicSize;
        currentZoom += Input.GetAxis("Mouse ScrollWheel") * -sensitivity;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        Camera.main.orthographicSize = currentZoom;

        currentZoomPercentage = currentZoom / maxZoom;
    }
}