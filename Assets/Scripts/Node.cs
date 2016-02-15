using UnityEngine;


public class Node : MonoBehaviour
{
    void FixedUpdate()
    {
		// simulate node movement to camera (will be removed)
		var camera = GameObject.Find("Main Camera");
        if (Vector3.Distance(gameObject.transform.position, camera.transform.position) > 0.5f)
        {
            Vector3 direction = camera.transform.position - gameObject.transform.position;
            gameObject.transform.position += Time.fixedDeltaTime * (0.3f * direction);
        }
    }
}
