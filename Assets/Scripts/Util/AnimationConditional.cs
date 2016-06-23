using System;

public struct AnimationConditional {
    public Action OnEnd;
    public Action OnStart;
    public Func<float, bool> Update;
}
