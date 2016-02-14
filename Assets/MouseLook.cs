using UnityEngine;


public class MouseLook : MonoBehaviour
{
    private float sensitivityX = 7f;
    private float sensitivityY = 7f;

    private float minimumY = -60f;
    private float maximumY = 60f;

    float rotationY = 0f;


    void Update()
    {
        if (!Input.GetKey(KeyCode.Space))
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0f);
        }
    }
}
