using UnityEngine;

public class Edge : MonoBehaviour
{
    public GameObject source, target;
    public Springy.Edge springyEdge;

    private void Awake()
    {
        // draw edges before nodes
        GetComponent<Renderer>().material.renderQueue = 0;
    }

    private void LateUpdate()
    {
        // update edge position
        GetComponent<LineRenderer>().SetPosition(0, source.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);

        // randomly spawn light from source node to target
        if (Random.value < 0.0008f)
            SpawnLight();
    }

    private void SpawnLight()
    {
        var light = (GameObject)Instantiate(Resources.Load("Light"));
        light.transform.parent = transform;

        // random duration and easing
        var duration = Random.Range(0.5f, 4f);
        System.Func<float, float>[] easings = { Easing.EaseOutCubic, Easing.EaseOutQuad, Easing.EaseOutQuart, Easing.EaseOutQuint };
        var easing = easings[Random.Range(0, easings.Length)];

        // add animation
        GameSystem.Instance.GetComponent<GameSystem>().Animate(new Animation
        {
            OnStart = () =>
            {
                light.transform.position = source.transform.position;
                light.SetActive(true);
            },
            Update = t =>
            {
                light.transform.position = Vector3.Lerp(source.transform.position, target.transform.position, t);
            },
            OnEnd = () =>
            {
                Destroy(light);
            },
            duration = duration,
            Ease = easing,
        });
    }
}