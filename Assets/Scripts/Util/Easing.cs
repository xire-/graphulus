// https://gist.github.com/gre/1650294

public class Easing
{
    // accelerating from zero velocity
    public static float EaseInCubic(float t)
    {
        return t * t * t;
    }

    // acceleration until halfway, then deceleration
    public static float EaseInOutCubic(float t)
    {
        return t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
    }

    // acceleration until halfway, then deceleration
    public static float EaseInOutQuad(float t)
    {
        return t < .5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }

    // acceleration until halfway, then deceleration
    public static float EaseInOutQuart(float t)
    {
        return t < .5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
    }

    // acceleration until halfway, then deceleration
    public static float EaseInOutQuint(float t)
    {
        return t < .5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;
    }

    // accelerating from zero velocity
    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    // accelerating from zero velocity
    public static float EaseInQuart(float t)
    {
        return t * t * t * t;
    }

    // accelerating from zero velocity
    public static float EaseInQuint(float t)
    {
        return t * t * t * t * t;
    }

    // decelerating to zero velocity
    public static float EaseOutCubic(float t)
    {
        return (--t) * t * t + 1;
    }

    // decelerating to zero velocity
    public static float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }

    // decelerating to zero velocity
    public static float EaseOutQuart(float t)
    {
        return 1 - (--t) * t * t * t;
    }

    // decelerating to zero velocity
    public static float EaseOutQuint(float t)
    {
        return 1 + (--t) * t * t * t * t;
    }
}