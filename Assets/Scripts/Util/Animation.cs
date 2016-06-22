using System;

public struct Animation {
    public float duration;
    public Func<float, float> Ease;
    public Action OnEnd;
    public Action OnStart;
    public Action<float> Update;
}
