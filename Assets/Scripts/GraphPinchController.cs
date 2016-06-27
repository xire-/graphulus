using Leap.Unity;
using UnityEngine;

public class GraphPinchController : MonoBehaviour {

    [HideInInspector]
    public GameObject pinchControllerObject;

    // to be set in editor
    public PinchDetector pinchDetectorR, pinchDetectorL;

    private ClosestNode _closestNodeR = new ClosestNode(), _closestNodeL = new ClosestNode();
    private PinchInfo _pinchInfoR, _pinchInfoL;

    private PinchInfo GetPinchInfo(PinchDetector pinchDetector, ClosestNode closestNode) {
        return new PinchInfo {
            node = closestNode.curr,
            nodeInitialPosition = (closestNode.curr == null) ? Vector3.zero : closestNode.curr.gameObject.transform.position,
            pinchDetectorInitialPosition = pinchDetector.Position,
        };
    }

    private void Start() {
        pinchControllerObject = new GameObject("GraphPinchController");
        pinchControllerObject.transform.parent = transform.parent;
        transform.parent = pinchControllerObject.transform;
    }

    private void transformDoubleAnchor() {
        pinchControllerObject.transform.position = (pinchDetectorR.Position + pinchDetectorL.Position) / 2f;

        Quaternion pp = Quaternion.Lerp(pinchDetectorR.Rotation, pinchDetectorL.Rotation, .5f);
        Vector3 u = pp * Vector3.up;
        pinchControllerObject.transform.LookAt(pinchDetectorR.Position, u);

        const bool allowScale = true;
        if (allowScale) {
            pinchControllerObject.transform.localScale = Vector3.one * Vector3.Distance(pinchDetectorR.Position, pinchDetectorL.Position);
        }
    }

    private void transformSingleAnchor(PinchDetector pinchDetector, PinchInfo pinchInfo) {
        var pinchPositionDelta = pinchInfo.pinchDetectorInitialPosition - pinchDetector.Position;
        pinchInfo.node.transform.position = pinchInfo.nodeInitialPosition - pinchPositionDelta;
    }

    private void Update() {
        // update closest node only when not pinching
        if (!pinchDetectorR.IsPinching) {
            UpdateClosestNode(pinchDetectorR, _closestNodeR);
        }
        if (!pinchDetectorL.IsPinching) {
            UpdateClosestNode(pinchDetectorL, _closestNodeL);
        }

        // update anchors
        bool didUpdate = pinchDetectorR.DidChangeFromLastFrame | pinchDetectorL.DidChangeFromLastFrame;
        if (didUpdate) {
            transform.SetParent(null, true);
        }

        UpdatePinchSingle(pinchDetectorR, _closestNodeR, ref _pinchInfoR);
        UpdatePinchSingle(pinchDetectorL, _closestNodeL, ref _pinchInfoL);
        UpdatePinchDouble();

        if (didUpdate) {
            transform.SetParent(pinchControllerObject.transform, true);
        }
    }

    private void UpdateClosestNode(PinchDetector pinchDetector, ClosestNode closestNode) {
        closestNode.prev = closestNode.curr;

        // get closest node to index tip
        var handModel = pinchDetector.GetComponentInParent<IHandModel>();
        if (handModel != null) {
            var index = handModel.GetLeapHand().Fingers[1];
            var indexTipPosition = index.TipPosition.ToVector3();

            var closestNodeToIndexTip = gameObject.GetComponent<Graph>().GetClosestNodeOrNull(indexTipPosition);
            const float maxDistance = 0.03f;
            closestNode.curr = Vector3.Distance(indexTipPosition, closestNodeToIndexTip.transform.position) <= maxDistance ? closestNodeToIndexTip : null;
        }
        else {
            closestNode.curr = null;
        }

        UpdateClosestNodeSelection(ref closestNode);
    }

    private void UpdateClosestNodeSelection(ref ClosestNode closestNodeParams) {
        if (closestNodeParams.prev != closestNodeParams.curr) {
            if (closestNodeParams.prev != null) {
                closestNodeParams.prev.Selected = false;
            }
            if (closestNodeParams.curr != null) {
                closestNodeParams.curr.Selected = true;
            }
        }
    }

    private void UpdatePinchDouble() {
        if (pinchDetectorR.IsPinching && pinchDetectorL.IsPinching) {
            // zoom and scale only when both pinch were not close to any node
            bool noNodePinch = _pinchInfoR.node == null && _pinchInfoL.node == null;
            if (noNodePinch) {
                // finalize single pinch (if any)
                if (pinchDetectorR.DidStartPinch || pinchDetectorL.DidStartPinch) {
                    if (_closestNodeR.curr != null && _closestNodeR.curr.Pinched) {
                        _closestNodeR.curr.Pinched = false;
                    }
                    if (_closestNodeL.curr != null && _closestNodeL.curr.Pinched) {
                        _closestNodeL.curr.Pinched = false;
                    }
                }

                transformDoubleAnchor();
            }
        }
    }

    private void UpdatePinchSingle(PinchDetector pinchDetector, ClosestNode closestNode, ref PinchInfo pinchInfo) {
        // if pinch detected, update pinch info
        if (pinchDetector.DidStartPinch) {
            pinchInfo = GetPinchInfo(pinchDetector, closestNode);
        }

        if (pinchInfo.node != null) {
            // set node as pinched when pinch starts
            if (pinchDetector.DidStartPinch) {
                pinchInfo.node.Pinched = true;
            }

            // update node position to match pinch movement
            if (pinchDetector.IsPinching) {
                transformSingleAnchor(pinchDetector, pinchInfo);
            }

            // release node when pinch ends
            if (pinchDetector.DidEndPinch) {
                pinchInfo.node.Pinched = false;
            }
        }
    }

    private struct PinchInfo {
        public Node node;
        public Vector3 nodeInitialPosition;
        public Vector3 pinchDetectorInitialPosition;
    }

    private class ClosestNode {
        public Node curr;
        public Node prev;
    }
}
