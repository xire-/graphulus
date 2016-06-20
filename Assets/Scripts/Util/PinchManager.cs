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
    private ClosestNodeObjectParams _closestNodeParamsL, _closestNodeParamsR;
    private GameObject _pinchControllerObject;
    private SinglePinchParams _singlePinchParams;

    private void Awake()
    {
        _pinchControllerObject = new GameObject("PinchController");
        _pinchControllerObject.transform.parent = graphObject.transform.parent;
        graphObject.transform.parent = _pinchControllerObject.transform;

        _closestNodeParamsL = new ClosestNodeObjectParams { pinchDetector = PinchDetectorL };
        _closestNodeParamsR = new ClosestNodeObjectParams { pinchDetector = PinchDetectorR };
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
            UpdateClosestNodeObject(ref _closestNodeParamsR);
        if (!PinchDetectorL.IsPinching)
            UpdateClosestNodeObject(ref _closestNodeParamsL);

        UpdatePinch();
    }

    private void UpdateClosestNodeObject(ref ClosestNodeObjectParams closestNodeObjectParams)
    {
        closestNodeObjectParams.closestNodeObjectPrev = closestNodeObjectParams.closestNodeObject;

        // get closest object to index tip
        var handModel = closestNodeObjectParams.pinchDetector.GetComponentInParent<IHandModel>();
        if (handModel != null)
        {
            var index = handModel.GetLeapHand().Fingers[1];
            var indexTipPosition = index.TipPosition.ToVector3();
            closestNodeObjectParams.closestNodeObject = graphObject.GetComponent<Graph>().FindClosestNodeObject(indexTipPosition, _maxClosestNodeDistance);
        }

        UpdateClosestNodeObjectSelection(ref closestNodeObjectParams);
    }

    private void UpdateClosestNodeObjectSelection(ref ClosestNodeObjectParams closestNodeObjectParams)
    {
        if (closestNodeObjectParams.closestNodeObjectPrev != closestNodeObjectParams.closestNodeObject)
        {
            if (closestNodeObjectParams.closestNodeObjectPrev != null)
                closestNodeObjectParams.closestNodeObjectPrev.GetComponent<Node>().Selected = false;
            if (closestNodeObjectParams.closestNodeObject != null)
                closestNodeObjectParams.closestNodeObject.GetComponent<Node>().Selected = true;
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
            if (_closestNodeParamsR.closestNodeObject != null && _closestNodeParamsR.closestNodeObject.GetComponent<Node>().Pinched)
                _closestNodeParamsR.closestNodeObject.GetComponent<Node>().Pinched = false;
            if (_closestNodeParamsL.closestNodeObject != null && _closestNodeParamsL.closestNodeObject.GetComponent<Node>().Pinched)
                _closestNodeParamsL.closestNodeObject.GetComponent<Node>().Pinched = false;

            transformDoubleAnchor();
        }
        else
        {
            UpdateSinglePinch(PinchDetectorR, ref _closestNodeParamsR);
            UpdateSinglePinch(PinchDetectorL, ref _closestNodeParamsL);
        }

        if (didUpdate)
            graphObject.transform.SetParent(_pinchControllerObject.transform, true);
    }

    private void UpdateSinglePinch(PinchDetector pinchDetector, ref ClosestNodeObjectParams closestNodeObjectParams)
    {
        // early exit if not pinching near a node
        if (closestNodeObjectParams.closestNodeObject == null)
            return;

        if (pinchDetector.DidStartPinch)
        {
            _singlePinchParams = new SinglePinchParams
            {
                nodeObject = closestNodeObjectParams.closestNodeObject,
                nodeObjectInitialPosition = closestNodeObjectParams.closestNodeObject.transform.position,
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

    private struct ClosestNodeObjectParams
    {
        public GameObject closestNodeObject;
        public GameObject closestNodeObjectPrev;
        public PinchDetector pinchDetector;
    }

    private struct SinglePinchParams
    {
        public GameObject nodeObject;
        public Vector3 nodeObjectInitialPosition;
        public Vector3 pinchDetectorInitialPosition;
    }
}