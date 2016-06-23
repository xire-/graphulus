using System;

public struct Test
{
    public Func<float, float> Ease;
    public Action OnEnd;
    public Action OnStart;
    public Func<float, float, bool> Update;
    public float duration;
}