using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject world;

    private void Start()
    {
        world = transform.parent.gameObject;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdateLook();

        if (world.GetComponent<World>().debugModeEnabled)
        {
            UpdateMovement();
        }
    }

    private void UpdateLook()
    {
        // http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/

        Vector2 sensitivity = new Vector2(6, 6);
        Vector2 smoothing = new Vector2(3, 3);

        var targetOrientation = Quaternion.Euler(transform.localRotation.eulerAngles);

        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        Vector2 smoothMouse = new Vector2();
        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        Vector2 mouseAbsolute = new Vector2();
        mouseAbsolute += smoothMouse;

        Vector2 clampInDegrees = new Vector2(360, 180);
        if (clampInDegrees.x < 360)
            mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        var xRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right);
        transform.localRotation = xRotation;

        if (clampInDegrees.y < 360)
            mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation *= targetOrientation;

        var yRotation = Quaternion.AngleAxis(mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
        transform.localRotation *= yRotation;
    }

    private void UpdateMovement()
    {
        const float speed = 10f;

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }
    }
}