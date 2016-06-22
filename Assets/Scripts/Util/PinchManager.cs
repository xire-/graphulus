using Leap.Unity;
using UnityEngine;

public class PinchManager : MonoBehaviour {

    // to be set in editor
    public PinchDetector PinchDetectorR, PinchDetectorL;

    private ClosestNode _closestNodeR = new ClosestNode(), _closestNodeL = new ClosestNode();
    private Graph _graph;
    private GameObject _pinchControllerObject;
    private PinchInfo _pinchInfoR, _pinchInfoL;

    private PinchInfo GetPinchInfo(PinchDetector pinchDetector, ClosestNode closestNode) {
        return new PinchInfo {
            node = closestNode.curr,
            nodeInitialPosition = (closestNode.curr == null) ? Vector3.zero : closestNode.curr.gameObject.transform.position,
            pinchDetectorInitialPosition = pinchDetector.Position,
        };
    }

    private void Start() {
        _graph = GameSystem.Instance.graph;

        _pinchControllerObject = new GameObject("PinchController");
        _pinchControllerObject.transform.parent = _graph.transform.parent;
        _graph.transform.parent = _pinchControllerObject.transform;
    }

    private void transformDoubleAnchor() {
        _pinchControllerObject.transform.position = (PinchDetectorR.Position + PinchDetectorL.Position) / 2f;

        Quaternion pp = Quaternion.Lerp(PinchDetectorR.Rotation, PinchDetectorL.Rotation, .5f);
        Vector3 u = pp * Vector3.up;
        _pinchControllerObject.transform.LookAt(PinchDetectorR.Position, u);

        const bool allowScale = true;
        if (allowScale) {
            _pinchControllerObject.transform.localScale = Vector3.one * Vector3.Distance(PinchDetectorR.Position, PinchDetectorL.Position);
        }
    }

    private void transformSingleAnchor(PinchDetector pinchDetector, PinchInfo pinchInfo) {
        var pinchPositionDelta = pinchInfo.pinchDetectorInitialPosition - pinchDetector.Position;
        pinchInfo.node.transform.position = pinchInfo.nodeInitialPosition - pinchPositionDelta;
    }

    private void Update() {
        // update closest node only when not pinching
        if (!PinchDetectorR.IsPinching) {
            UpdateClosestNode(PinchDetectorR, _closestNodeR);
        }
        if (!PinchDetectorL.IsPinching) {
            UpdateClosestNode(PinchDetectorL, _closestNodeL);
        }

        // update anchors
        bool didUpdate = PinchDetectorR.DidChangeFromLastFrame | PinchDetectorL.DidChangeFromLastFrame;
        if (didUpdate) {
            _graph.transform.SetParent(null, true);
        }

        UpdatePinchSingle(PinchDetectorR, _closestNodeR, ref _pinchInfoR);
        UpdatePinchSingle(PinchDetectorL, _closestNodeL, ref _pinchInfoL);
        UpdatePinchDouble();

        if (didUpdate) {
            _graph.transform.SetParent(_pinchControllerObject.transform, true);
        }
    }

    private void UpdateClosestNode(PinchDetector pinchDetector, ClosestNode closestNode) {
        closestNode.prev = closestNode.curr;

        // get closest node to index tip
        var handModel = pinchDetector.GetComponentInParent<IHandModel>();
        if (handModel != null) {
            var index = handModel.GetLeapHand().Fingers[1];
            var indexTipPosition = index.TipPosition.ToVector3();

            const float maxClosestNodeDistance = 0.08f;
            closestNode.curr = _graph.GetClosestNodeOrNull(indexTipPosition, maxClosestNodeDistance);
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
        if (PinchDetectorR.IsPinching && PinchDetectorL.IsPinching) {
            // zoom and scale only when both pinch were not close to any node
            bool noNodePinch = _pinchInfoR.node == null && _pinchInfoL.node == null;
            if (noNodePinch) {
                // finalize single pinch (if any)
                if (PinchDetectorR.DidStartPinch || PinchDetectorL.DidStartPinch) {
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
