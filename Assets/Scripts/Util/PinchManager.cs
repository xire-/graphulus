using Leap.Unity;
using UnityEngine;

public class PinchManager : MonoBehaviour
{
    // to be set in editor
    public GameObject graphObject;

    // to be set in editor
    public PinchDetector PinchDetectorL, PinchDetectorR;

    private readonly float _maxClosestNodeDistance = 0.08f;
    private bool _allowScale = true;
    private GameObject _closestNodeObject, _closestNodeObjectPrev;
    private GameObject _pinchControllerObject;
    private SinglePinchParams _singlePinchParams;

    private void Awake()
    {
        _pinchControllerObject = new GameObject("PinchController");
        _pinchControllerObject.transform.parent = graphObject.transform.parent;
        graphObject.transform.parent = _pinchControllerObject.transform;
    }

    private void transformDoubleAnchor()
    {
        _pinchControllerObject.transform.position = (PinchDetectorR.Position + PinchDetectorL.Position) / 2f;

        Quaternion pp = Quaternion.Lerp(PinchDetectorR.Rotation, PinchDetectorL.Rotation, .5f);
        Vector3 u = pp * Vector3.up;
        _pinchControllerObject.transform.LookAt(PinchDetectorR.Position, u);

        if (_allowScale)
            _pinchControllerObject.transform.localScale = Vector3.one * Vector3.Distance(PinchDetectorR.Position, PinchDetectorL.Position);
    }

    private void transformSingleAnchor(PinchDetector singlePinch)
    {
        var diff = _singlePinchParams.pinchDetectorInitialPosition - singlePinch.Position;
        _singlePinchParams.nodeObject.transform.position = _singlePinchParams.nodeObjectInitialPosition - diff;
    }

    private void Update()
    {
        if (!PinchDetectorR.IsPinching)
            UpdateClosestNodeObject();
        UpdatePinch();
    }

    private void UpdateClosestNodeObject()
    {
        _closestNodeObjectPrev = _closestNodeObject;

        // get closest object to index tip
        var handModel = PinchDetectorR.GetComponentInParent<IHandModel>();
        if (handModel != null)
        {
            var index = handModel.GetLeapHand().Fingers[1];
            var indexTipPosition = index.TipPosition.ToVector3();
            _closestNodeObject = graphObject.GetComponent<Graph>().FindClosestNodeObject(indexTipPosition, _maxClosestNodeDistance);
        }

        UpdateClosestNodeObjectSelection();
    }

    private void UpdateClosestNodeObjectSelection()
    {
        if (_closestNodeObjectPrev != _closestNodeObject)
        {
            if (_closestNodeObjectPrev != null)
                _closestNodeObjectPrev.GetComponent<Node>().Selected = false;
            if (_closestNodeObject != null)
                _closestNodeObject.GetComponent<Node>().Selected = true;
        }
    }

    private void UpdatePinch()
    {
        bool didUpdate = false;
        didUpdate |= PinchDetectorR.DidChangeFromLastFrame;
        didUpdate |= PinchDetectorL.DidChangeFromLastFrame;

        if (didUpdate)
            graphObject.transform.SetParent(null, true);

        if (PinchDetectorR.IsPinching && PinchDetectorL.IsPinching)
        {
            if (_closestNodeObject != null && _closestNodeObject.GetComponent<Node>().Pinched)
                _closestNodeObject.GetComponent<Node>().Pinched = false;

            transformDoubleAnchor();
        }
        else
        {
            UpdateSinglePinch(PinchDetectorR);
            UpdateSinglePinch(PinchDetectorL);
        }

        if (didUpdate)
            graphObject.transform.SetParent(_pinchControllerObject.transform, true);
    }

    private void UpdateSinglePinch(PinchDetector pinchDetector)
    {
        // early exit if not pinching near a node
        if (_closestNodeObject == null)
            return;

        if (pinchDetector.DidStartPinch)
        {
            _singlePinchParams = new SinglePinchParams
            {
                nodeObject = _closestNodeObject,
                nodeObjectInitialPosition = _closestNodeObject.transform.position,
                pinchDetectorInitialPosition = pinchDetector.Position,
            };

            _singlePinchParams.nodeObject.GetComponent<Node>().Pinched = true;
        }

        if (pinchDetector.IsPinching && _singlePinchParams.nodeObject != null)
            transformSingleAnchor(pinchDetector);

        if (pinchDetector.DidEndPinch && _singlePinchParams.nodeObject != null)
        {
            _singlePinchParams.nodeObject.GetComponent<Node>().Pinched = false;
            _singlePinchParams.nodeObject = null;
        }
    }

    private struct SinglePinchParams
    {
        public GameObject nodeObject;
        public Vector3 nodeObjectInitialPosition;
        public Vector3 pinchDetectorInitialPosition;
    }
}