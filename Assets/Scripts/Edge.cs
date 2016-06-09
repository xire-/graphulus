using UnityEngine;
using UnityEngine.Assertions;

public class Edge : MonoBehaviour
{
    public int length;
    public GameObject source, target;

    private void Awake()
    {
        // draw edges before nodes
        GetComponent<Renderer>().material.renderQueue = 0;
    }

    private void LateUpdate()
    {
        Assert.IsNotNull(source);
        GetComponent<LineRenderer>().SetPosition(0, source.transform.position);
        Assert.IsNotNull(target);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);

        float val = Random.value;
        if (val < 0.0008f)
        {
            var light = (GameObject)Instantiate(Resources.Load("EdgeLight"));
         
//            var animation = transform.parent.GetComponent<World>().animationManager.Add(t => MoveLight(light, t), 1f, Easing.EaseOutQuart);

            var animation = new Animation
            {
                duration = 1f,
                Update = t => MoveLight(light, t),
                Ease = Easing.EaseOutQuart
                        
            };
            


            animation.OnEnd = delegate
            {
                Destroy(light);
            };

            transform.parent.GetComponent<World>().animationManager.Add(animation);
        }
    }

    private void MoveLight(GameObject light, float t)
    {
        light.transform.position = Vector3.Lerp(source.transform.position, target.transform.position, t);
    }
}