using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager
{
    private List<AnimationInfo> animations, animationsToStart;

    public AnimationManager()
    {
        animations = new List<AnimationInfo>();
        animationsToStart = new List<AnimationInfo>();
    }

    public void Add(Animation animation)
//    public void Add(Action<float> update, float duration, Func<float, float> ease = null)
    {
//        var animation = new Animation
//        {
//            startTime = Time.realtimeSinceStartup,
//            duration = duration,
//            endTime = Time.realtimeSinceStartup + duration,
//            Update = update,
//            Ease = ease
//        };
//        animationsToStart.Add(animation);
//        return animation;

        var animationInfo = new AnimationInfo {
            animation = animation,
            startTime = Time.realtimeSinceStartup,
            endTime = Time.realtimeSinceStartup + animation.duration
        };
        animationsToStart.Add(animationInfo);
    }

    public void Update()
    {
        // start animations not yet started
        for (var i = animationsToStart.Count - 1; i >= 0; i--)
        {
            var animationInfo = animationsToStart[i];
            var animation = animationInfo.animation;
            if (animation.OnStart != null)
                animation.OnStart();
            animationsToStart.RemoveAt(i);
            animations.Add(animationInfo);
        }

        // update ongoing animations
        for (var i = animations.Count - 1; i >= 0; i--)
        {
            var animationInfo = animations[i];
            var animation = animationInfo.animation;
            if (Time.realtimeSinceStartup > animationInfo.endTime)
            {
                animation.Update(1f);
                if (animation.OnEnd != null)
                    animation.OnEnd();
                animations.RemoveAt(i);
            }
            else
            {
                float t = (Time.realtimeSinceStartup - animationInfo.startTime) / animation.duration;
                if (animation.Ease != null)
                    t = animation.Ease(t);
                animation.Update(t);
            }
        }
    }


    private struct AnimationInfo {
        public Animation animation;
        public float endTime;
        public float startTime;
    }
}