using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private readonly Vector2 _sensitivity = new Vector2(3, 3);
    private readonly Vector2 _smoothing = new Vector2(3, 3);

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private Node FindLookedNodeObject()
    {
        RaycastHit hit;
        const float radius = 0.02f;
        if (Physics.SphereCast(Camera.main.transform.position, radius, Camera.main.transform.forward, out hit))
        {
            var lookedObject = hit.transform.gameObject;
            if (lookedObject.tag == "Node")
                return lookedObject.GetComponent<Node>();
            else
                return null;
        }
        else
            return null;
    }

    private void Start()
    {
        StartCoroutine("UpdateLookedNodeObject");
    }

    private void Update()
    {
        UpdateLook();
        UpdateMovement();

        UpdateLookedNodeObject();
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

    private IEnumerator UpdateLookedNodeObject()
    {
        Node lookedNode = null, lookedNodePrev = null;
        while (true)
        {
            lookedNodePrev = lookedNode;
            lookedNode = FindLookedNodeObject();

            if (lookedNodePrev != lookedNode)
            {
                if (lookedNodePrev != null)
                    lookedNodePrev.Selected = false;
                if (lookedNode != null)
                    lookedNode.Selected = true;
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    private void UpdateMovement()
    {
        const float movementSpeed = 0.5f;

        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * movementSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            transform.position += Vector3.up * movementSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.Q))
            transform.position -= Vector3.up * movementSpeed * Time.deltaTime;
    }
}