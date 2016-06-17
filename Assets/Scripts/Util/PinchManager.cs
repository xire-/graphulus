using UnityEngine;

public class PinchManager : MonoBehaviour
{
    public GameObject graphObject;
    public Leap.Unity.PinchDetector PinchDetectorL, PinchDetectorR;
    private bool _allowScale = true;
    private Transform _anchor;
    private Vector3 initial;
    private Vector3 initialPos;
    private GameObject nearestNode;

    private void Awake()
    {
        GameObject pinchControl = new GameObject("PinchControl");
        _anchor = pinchControl.transform;
        _anchor.transform.parent = graphObject.transform.parent;
        graphObject.transform.parent = _anchor;
    }

    private GameObject GetClosestObject(Vector3 point)
    {
        GameObject closestObject = null;
        foreach (var nodeObject in graphObject.GetComponent<Graph>().nodes)
        {
            if (closestObject == null)
                closestObject = nodeObject;
            if (Vector3.Distance(point, nodeObject.transform.position) < Vector3.Distance(point, closestObject.transform.position))
                closestObject = nodeObject;
        }
        return closestObject;
    }

    private void transformDoubleAnchor()
    {
        _anchor.position = (PinchDetectorR.Position + PinchDetectorL.Position) / 2f;

        Quaternion pp = Quaternion.Lerp(PinchDetectorR.Rotation, PinchDetectorL.Rotation, .5f);
        Vector3 u = pp * Vector3.up;
        _anchor.LookAt(PinchDetectorR.Position, u);

        if (_allowScale)
            _anchor.localScale = Vector3.one * Vector3.Distance(PinchDetectorR.Position, PinchDetectorL.Position);
    }

    private void transformSingleAnchor(Leap.Unity.PinchDetector singlePinch)
    {
        var diff = initial - singlePinch.Position;
        nearestNode.transform.position = initialPos - diff;
    }

    private void Update()
    {
        bool didUpdate = false;
        if (PinchDetectorR != null)
            didUpdate |= PinchDetectorR.DidChangeFromLastFrame;
        if (PinchDetectorL != null)
            didUpdate |= PinchDetectorL.DidChangeFromLastFrame;

        if (didUpdate)
            graphObject.transform.SetParent(null, true);

        if (PinchDetectorR.DidStartPinch || PinchDetectorL.DidStartPinch)
        {
            nearestNode = GetClosestObject(PinchDetectorR.Position);
            nearestNode.GetComponent<Node>().Select();
            nearestNode.GetComponent<Node>().IsPinched = true;

            initial = PinchDetectorR.Position;
            initialPos = nearestNode.transform.position;
        }

        if (PinchDetectorR != null && PinchDetectorR.IsPinching && PinchDetectorL != null && PinchDetectorL.IsPinching)
            transformDoubleAnchor();
        else if (PinchDetectorR != null && PinchDetectorR.IsPinching)
            transformSingleAnchor(PinchDetectorR);
        else if (PinchDetectorL != null && PinchDetectorL.IsPinching)
            transformSingleAnchor(PinchDetectorL);

        if (didUpdate)
            graphObject.transform.SetParent(_anchor, true);
    }
}