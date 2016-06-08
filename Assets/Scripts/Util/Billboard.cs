using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.Rotate(0, 180, 0);
    }
}