using System.Collections.Generic;
using UnityEngine;

public class AnimationManager
{
    private List<Animation> animations;

    public AnimationManager()
    {
        animations = new List<Animation>();
    }

    public delegate void AnimationDelegate(float t);

    public delegate float EasingDelegate(float t);

    public void StartAnimation(AnimationDelegate animationDelegate, float duration, EasingDelegate easingDelegate = null)
    {
        var startTime = Time.realtimeSinceStartup;
        var animation = new Animation
        {
            startTime = startTime,
            duration = duration,
            endTime = startTime + duration,
            animationDelegate = animationDelegate,
            easingDelegate = easingDelegate
        };
        animations.Add(animation);
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
                if (animation.easingDelegate != null)
                    t = animation.easingDelegate(t);
                animation.animationDelegate(t);
            }
        }
    }

    private struct Animation
    {
        public AnimationDelegate animationDelegate;
        public float duration;
        public EasingDelegate easingDelegate;
        public float endTime;
        public float startTime;
    }
}