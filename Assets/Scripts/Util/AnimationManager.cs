using UnityEngine;
using System.Collections.Generic;

public class AnimationManager
{
    public delegate void AnimationDelegate(float t);

    private List<Animation> animations;

    public AnimationManager()
    {
        animations = new List<Animation>();
    }

    public void Update()
    {
        // iterating backwards allows to modify the list
        for (var i = animations.Count - 1; i >= 0; i--)
        {
            var animation = animations[i];
            if (Time.realtimeSinceStartup > animation.endTime)
                animations.RemoveAt(i);
            else
            {
                float t = (Time.realtimeSinceStartup - animation.startTime) / animation.duration;
                animation.animationDelegate(t);
            }
        }
    }

    public void StartAnimation(AnimationDelegate animationDelegate, float duration)
    {
        var startTime = Time.realtimeSinceStartup;
        var animation = new Animation
        {
            startTime = startTime,
            duration = duration,
            endTime = startTime + duration,
            animationDelegate = animationDelegate
        };
        animations.Add(animation);
    }

    private struct Animation
    {
        public float startTime;
        public float duration;
        public float endTime;
        public AnimationDelegate animationDelegate;
    }
}
