using UnityEngine;

public class Billboard : MonoBehaviour {

    private void LateUpdate() {
        // http://wiki.unity3d.com/index.php?title=CameraFacingBillboard

        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
             Camera.main.transform.rotation * Vector3.up);
    }
}
