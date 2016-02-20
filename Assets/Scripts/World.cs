using UnityEngine;


public class World : MonoBehaviour
{
    void Start()
    {
        // draw grid of nodes to experiments on (will be removed)
        for (float x = -1; x < 2; x += 0.5f)
        {
            for (float y = -1; y < 2; y += 0.5f)
            {
                GameObject node = (GameObject)Instantiate(Resources.Load("Node"), new Vector3(x, y, 0f), Quaternion.identity);
                node.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                node.transform.parent = gameObject.transform;
            }
        }
        for (float z = -3; z < 0; z += 0.5f)
        {
            for (float y = -1; y < 2; y += 0.5f)
            {
                GameObject node = (GameObject)Instantiate(Resources.Load("Node"), new Vector3(-1f, y, z), Quaternion.identity);
                node.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                node.transform.parent = gameObject.transform;
            }
        }
    }
}
