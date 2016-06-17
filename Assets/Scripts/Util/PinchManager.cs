using UnityEngine;

public class PinchManager : MonoBehaviour
{
    public Leap.Unity.PinchDetector PinchDetectorR;
    public Leap.Unity.PinchDetector PinchDetectorL;

    private bool _allowScale = true;

    private Transform _anchor;

    private void Awake()
    {
        GameObject pinchControl = new GameObject("PinchControl");
        _anchor = pinchControl.transform;
        _anchor.transform.parent = transform.parent;
        transform.parent = _anchor;
    }

    private void transformDoubleAnchor()
    {
        _anchor.position = (PinchDetectorR.Position + PinchDetectorL.Position) / 2.0f;

        Quaternion pp = Quaternion.Lerp(PinchDetectorR.Rotation, PinchDetectorL.Rotation, 0.5f);
        Vector3 u = pp * Vector3.up;
        _anchor.LookAt(PinchDetectorR.Position, u);

        if (_allowScale)
            _anchor.localScale = Vector3.one * Vector3.Distance(PinchDetectorR.Position, PinchDetectorL.Position);
    }

    private void transformSingleAnchor(Leap.Unity.PinchDetector singlePinch)
    {
        _anchor.position = singlePinch.Position;
        _anchor.localScale = Vector3.one;
    }

    private void Update()
    {
        bool didUpdate = false;
        if (PinchDetectorR != null)
            didUpdate |= PinchDetectorR.DidChangeFromLastFrame;
        if (PinchDetectorL != null)
            didUpdate |= PinchDetectorL.DidChangeFromLastFrame;

        if (didUpdate)
            transform.SetParent(null, true);

        if (PinchDetectorR != null && PinchDetectorR.IsPinching && PinchDetectorL != null && PinchDetectorL.IsPinching)
            transformDoubleAnchor();
        else if (PinchDetectorR != null && PinchDetectorR.IsPinching)
            transformSingleAnchor(PinchDetectorR);
        else if (PinchDetectorL != null && PinchDetectorL.IsPinching)
            transformSingleAnchor(PinchDetectorL);

        if (didUpdate)
            transform.SetParent(_anchor, true);
    }
}