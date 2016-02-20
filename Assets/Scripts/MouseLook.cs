using UnityEngine;


public class MouseLook : MonoBehaviour
{
    private float sensitivityX = 7f;
    private float sensitivityY = 7f;

    private float minimumY = -60f;
    private float maximumY = 60f;

    float rotationY = 0f;

    private GameObject curr;
    private Vector3? moveTo;


    void Update()
    {
        if (moveTo.HasValue)
        {
            GameObject.Find("Main Camera").transform.position = Vector3.MoveTowards(GameObject.Find("Main Camera").transform.position, moveTo.Value, 0.5f * Time.deltaTime);
        }

        if (!Input.GetKey(KeyCode.Space))
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0f);
        }

        RaycastHit hit;
        var cameratrans = GameObject.Find("Main Camera").transform;
        if (Physics.Raycast(cameratrans.position, cameratrans.forward, out hit))
        {
            curr = hit.transform.gameObject;
            if (Input.GetMouseButtonDown(0))
            {
                moveTo = curr.transform.position;
            }
            else
            {
                curr.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else {
            if (curr)
            {
                curr.GetComponent<Renderer>().material.color = Color.white;
                curr = null;
            }
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 2, 2), "");
    }
}
