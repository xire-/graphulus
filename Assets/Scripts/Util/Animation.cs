using System;

public class Animation
{
    public Func<float, float> Ease;
    public Action<float> Update;
    public Action OnStart;
    public Action OnEnd;

    public float duration;
//    public float endTime;
//    public float startTime;
}
