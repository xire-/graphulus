using UnityEngine;

public class RTS : MonoBehaviour
{
    public Leap.Unity.PinchDetector PinchDetectorA;
    public Leap.Unity.PinchDetector PinchDetectorB;

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
        _anchor.position = (PinchDetectorA.Position + PinchDetectorB.Position) / 2.0f;

        Quaternion pp = Quaternion.Lerp(PinchDetectorA.Rotation, PinchDetectorB.Rotation, 0.5f);
        Vector3 u = pp * Vector3.up;
        _anchor.LookAt(PinchDetectorA.Position, u);

        if (_allowScale)
            _anchor.localScale = Vector3.one * Vector3.Distance(PinchDetectorA.Position, PinchDetectorB.Position);
    }

    private void transformSingleAnchor(Leap.Unity.PinchDetector singlePinch)
    {
        _anchor.position = singlePinch.Position;
        _anchor.localScale = Vector3.one;
    }

    private void Update()
    {
        bool didUpdate = false;
        if (PinchDetectorA != null)
            didUpdate |= PinchDetectorA.DidChangeFromLastFrame;
        if (PinchDetectorB != null)
            didUpdate |= PinchDetectorB.DidChangeFromLastFrame;

        if (didUpdate)
            transform.SetParent(null, true);

        if (PinchDetectorA != null && PinchDetectorA.IsPinching && PinchDetectorB != null && PinchDetectorB.IsPinching)
            transformDoubleAnchor();
        else if (PinchDetectorA != null && PinchDetectorA.IsPinching)
            transformSingleAnchor(PinchDetectorA);
        else if (PinchDetectorB != null && PinchDetectorB.IsPinching)
            transformSingleAnchor(PinchDetectorB);

        if (didUpdate)
            transform.SetParent(_anchor, true);
    }
}