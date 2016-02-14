using UnityEngine;


public class World : MonoBehaviour
{
    void Start()
    {
        for (float x = -1; x < 2; x += 0.5f)
        {
            for (float y = -1; y < 2; y += 0.5f)
            {
                GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                brick.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                brick.transform.position = new Vector3(x, y, 0f);
                brick.transform.parent = gameObject.transform;
                brick.AddComponent(System.Type.GetType("Brick"));
            }
        }

        for (float z = -3; z < 0; z += 0.5f)
        {
            for (float y = -1; y < 2; y += 0.5f)
            {
                GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                brick.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                brick.transform.position = new Vector3(-1f, y, z);
                brick.transform.parent = gameObject.transform;
                brick.GetComponent<Renderer>().material.color = Color.red;
                brick.AddComponent(System.Type.GetType("Brick"));
            }
        }
    }
}
