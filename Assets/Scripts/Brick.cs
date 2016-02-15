using UnityEngine;


public class Brick : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Vector3.Distance(gameObject.transform.position, GameObject.Find("Main Camera").transform.position) > 0.5f)
        {
            Vector3 direction = GameObject.Find("Main Camera").transform.position - gameObject.transform.position;
            gameObject.transform.position += Time.fixedDeltaTime * (0.3f * direction);
        }
    }
}
