using Leap.Unity;
using UnityEngine;

public class PinchManager : MonoBehaviour
{
    // to be set in editor
    public GameObject graphObject;

    // to be set in editor
    public PinchDetector PinchDetectorL, PinchDetectorR;

    private ClosestNodeParams _closestNodeParamsL, _closestNodeParamsR;
    private GameObject _pinchControllerObject;
    private SinglePinchParams _singlePinchParams;

    private void Awake()
    {
        _pinchControllerObject = new GameObject("PinchController");
        _pinchControllerObject.transform.parent = graphObject.transform.parent;
        graphObject.transform.parent = _pinchControllerObject.transform;

        _closestNodeParamsL = new ClosestNodeParams { pinchDetector = PinchDetectorL };
        _closestNodeParamsR = new ClosestNodeParams { pinchDetector = PinchDetectorR };
    }

    private void transformDoubleAnchor()
    {
        _pinchControllerObject.transform.position = (PinchDetectorR.Position + PinchDetectorL.Position) / 2f;

        Quaternion pp = Quaternion.Lerp(PinchDetectorR.Rotation, PinchDetectorL.Rotation, .5f);
        Vector3 u = pp * Vector3.up;
        _pinchControllerObject.transform.LookAt(PinchDetectorR.Position, u);

        const bool allowScale = true;
        if (allowScale)
            _pinchControllerObject.transform.localScale = Vector3.one * Vector3.Distance(PinchDetectorR.Position, PinchDetectorL.Position);
    }

    private void transformSingleAnchor(PinchDetector pinchDetector)
    {
        var pinchPositionDelta = _singlePinchParams.pinchDetectorInitialPosition - pinchDetector.Position;
        _singlePinchParams.node.transform.position = _singlePinchParams.nodeInitialPosition - pinchPositionDelta;
    }

    private void Update()
    {
        bool didUpdate = PinchDetectorR.DidChangeFromLastFrame | PinchDetectorL.DidChangeFromLastFrame;

        if (didUpdate)
            graphObject.transform.SetParent(null, true);

        if (!PinchDetectorR.IsPinching && !PinchDetectorL.IsPinching)
        {
            UpdateClosestNode(ref _closestNodeParamsR);
            UpdateClosestNode(ref _closestNodeParamsL);
        }
        else if (PinchDetectorR.IsPinching && PinchDetectorL.IsPinching)
            UpdateDoublePinch();
        else if (PinchDetectorR.IsPinching)
            UpdateSinglePinch(PinchDetectorR, ref _closestNodeParamsR);
        else if (PinchDetectorL.IsPinching)
            UpdateSinglePinch(PinchDetectorL, ref _closestNodeParamsL);

        if (didUpdate)
            graphObject.transform.SetParent(_pinchControllerObject.transform, true);
    }

    private void UpdateClosestNode(ref ClosestNodeParams closestNodeParams)
    {
        closestNodeParams.nodePrev = closestNodeParams.node;

        // get closest node to index tip
        var handModel = closestNodeParams.pinchDetector.GetComponentInParent<IHandModel>();
        if (handModel != null)
        {
            var index = handModel.GetLeapHand().Fingers[1];
            var indexTipPosition = index.TipPosition.ToVector3();

            const float maxClosestNodeDistance = 0.08f;
            var closestNodeObject = graphObject.GetComponent<Graph>().FindClosestNodeObject(indexTipPosition, maxClosestNodeDistance);
            closestNodeParams.node = closestNodeObject != null ? closestNodeObject.GetComponent<Node>() : null;
        }

        UpdateClosestNodeSelection(ref closestNodeParams);
    }

    private void UpdateClosestNodeSelection(ref ClosestNodeParams closestNodeParams)
    {
        if (closestNodeParams.nodePrev != closestNodeParams.node)
        {
            if (closestNodeParams.nodePrev != null)
                closestNodeParams.nodePrev.GetComponent<Node>().Selected = false;
            if (closestNodeParams.node != null)
                closestNodeParams.node.GetComponent<Node>().Selected = true;
        }
    }

    private void UpdateDoublePinch()
    {
        // finalize single pinch (if any)
        if (PinchDetectorR.DidStartPinch || PinchDetectorL.DidStartPinch)
        {
            if (_closestNodeParamsR.node != null && _closestNodeParamsR.node.GetComponent<Node>().Pinched)
                _closestNodeParamsR.node.GetComponent<Node>().Pinched = false;
            if (_closestNodeParamsL.node != null && _closestNodeParamsL.node.GetComponent<Node>().Pinched)
                _closestNodeParamsL.node.GetComponent<Node>().Pinched = false;
        }

        transformDoubleAnchor();
    }

    private void UpdateSinglePinch(PinchDetector pinchDetector, ref ClosestNodeParams closestNodeParams)
    {
        // early exit if not pinching near a node
        if (closestNodeParams.node == null)
            return;

        if (pinchDetector.DidStartPinch)
        {
            _singlePinchParams = new SinglePinchParams
            {
                node = closestNodeParams.node,
                nodeInitialPosition = closestNodeParams.node.transform.position,
                pinchDetectorInitialPosition = pinchDetector.Position,
            };

            _singlePinchParams.node.GetComponent<Node>().Pinched = true;
        }

        if (pinchDetector.IsPinching && _singlePinchParams.node != null)
            transformSingleAnchor(pinchDetector);

        if (pinchDetector.DidEndPinch && _singlePinchParams.node != null)
        {
            _singlePinchParams.node.GetComponent<Node>().Pinched = false;
            _singlePinchParams.node = null;
        }
    }

    private struct ClosestNodeParams
    {
        public Node node;
        public Node nodePrev;
        public PinchDetector pinchDetector;
    }

    private struct SinglePinchParams
    {
        public Node node;
        public Vector3 nodeInitialPosition;
        public Vector3 pinchDetectorInitialPosition;
    }
}