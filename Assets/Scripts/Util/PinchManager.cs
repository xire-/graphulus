using UnityEngine;

public class PinchManager : MonoBehaviour
{
    // to be set in editor
    public GameObject graphObject;

    // to be set in editor
    public Leap.Unity.PinchDetector PinchDetectorL, PinchDetectorR;

    private bool _allowScale = true;
    private GameObject _pinchControlObject;
    private SinglePinchParams _singlePinchParams;

    private void Awake()
    {
        _pinchControlObject = new GameObject("Pinch Control");
        _pinchControlObject.transform.parent = graphObject.transform.parent;
        graphObject.transform.parent = _pinchControlObject.transform;
    }

    private void transformDoubleAnchor()
    {
        _pinchControlObject.transform.position = (PinchDetectorR.Position + PinchDetectorL.Position) / 2f;

        Quaternion pp = Quaternion.Lerp(PinchDetectorR.Rotation, PinchDetectorL.Rotation, .5f);
        Vector3 u = pp * Vector3.up;
        _pinchControlObject.transform.LookAt(PinchDetectorR.Position, u);

        if (_allowScale)
            _pinchControlObject.transform.localScale = Vector3.one * Vector3.Distance(PinchDetectorR.Position, PinchDetectorL.Position);
    }

    private void transformSingleAnchor(Leap.Unity.PinchDetector singlePinch)
    {
        var diff = _singlePinchParams.pinchDetectorInitialPosition - singlePinch.Position;
        _singlePinchParams.nodeObject.transform.position = _singlePinchParams.nodeObjectInitialPosition - diff;
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

        if (PinchDetectorR.DidStartPinch)
        {
            var pinchDetector = PinchDetectorR.DidStartPinch ? PinchDetectorR : PinchDetectorL;

            var closestNodeObject = graphObject.GetComponent<Graph>().FindClosestNodeObject(pinchDetector.Position);
            closestNodeObject.GetComponent<Node>().IsPinched = true;
            closestNodeObject.GetComponent<Node>().Select();

            _singlePinchParams = new SinglePinchParams
            {
                nodeObject = closestNodeObject,
                nodeObjectInitialPosition = closestNodeObject.transform.position,
                pinchDetectorInitialPosition = PinchDetectorR.Position,
            };
        }

        if (PinchDetectorR != null && PinchDetectorR.IsPinching && PinchDetectorL != null && PinchDetectorL.IsPinching)
            transformDoubleAnchor();
        else if (PinchDetectorR != null && PinchDetectorR.IsPinching)
            transformSingleAnchor(PinchDetectorR);
        //else if (PinchDetectorL != null && PinchDetectorL.IsPinching)
        //    transformSingleAnchor(PinchDetectorL);

        if (didUpdate)
            graphObject.transform.SetParent(_pinchControlObject.transform, true);
    }

    private struct SinglePinchParams
    {
        public GameObject nodeObject;
        public Vector3 nodeObjectInitialPosition;
        public Vector3 pinchDetectorInitialPosition;
    }
}