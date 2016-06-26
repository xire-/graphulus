using System;

public struct Job {
    public Action OnEnd;
    public Action OnStart;
    public Func<float, float, bool> Update;
}
