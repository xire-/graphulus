using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const float _movementSpeed = 0.5f;
    private readonly Vector2 _sensitivity = new Vector2(3, 3);
    private readonly Vector2 _smoothing = new Vector2(3, 3);

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdateLook();
        UpdateMovement();
    }

    private void UpdateLook()
    {
        // http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/

        var targetOrientation = Quaternion.Euler(transform.localRotation.eulerAngles);

        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(_sensitivity.x * _smoothing.x, _sensitivity.y * _smoothing.y));

        Vector2 smoothMouse = new Vector2();
        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / _smoothing.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / _smoothing.y);

        Vector2 mouseAbsolute = new Vector2();
        mouseAbsolute += smoothMouse;

        Vector2 clampInDegrees = new Vector2(360f, 180f);
        if (clampInDegrees.x < 360f)
            mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        var xRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right);
        transform.localRotation = xRotation;

        if (clampInDegrees.y < 360f)
            mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation *= targetOrientation;

        var yRotation = Quaternion.AngleAxis(mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
        transform.localRotation *= yRotation;
    }

    private void UpdateMovement()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * _movementSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * _movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * _movementSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * _movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            transform.position += Vector3.up * _movementSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.Q))
            transform.position -= Vector3.up * _movementSpeed * Time.deltaTime;
    }
}