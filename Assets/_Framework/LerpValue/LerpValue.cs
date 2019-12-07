using System;
using UnityEngine;

public class LerpValue
{
    public float Current { get; private set; }

    private float last = 0f;
    private float target = 0f;
    private float interpolationValue = 0f;
    private InterpolationMethod interpolationMethod = InterpolationMethod.Linear;
    private Func<float> setTarget;

    //private float changeTime = 0f;

    public LerpValue(Func<float> setTarget)
    {
        this.setTarget = setTarget;
        Init();
    }

    public LerpValue(Func<float> setTarget, InterpolationMethod interpolationMethod) 
        : this(setTarget)
    {
        this.interpolationMethod = interpolationMethod;
    }

    public LerpValue(Func<float> setTarget, float startFromValue, InterpolationMethod interpolationMethod)
        : this(setTarget, interpolationMethod)
    {
        StartFrom(startFromValue);
    }

    private void Init()
    {
        last = Current;
        target = setTarget();
        interpolationValue = 0f;
    }

    public void StartFrom(float value)
    {
        last = value;
    }

    public void Update()
    {
        Debug.Log(last);

        // Update the interpolationValue
        interpolationValue = Mathf.Clamp01(interpolationValue + Time.deltaTime);

        // Update the current value based on the interpolationValue
        float t = interpolationValue;

        switch (interpolationMethod)
        {
            case InterpolationMethod.Cosine:
                t = (1 - Mathf.Cos(t * Mathf.PI)) / 2;
                break;
        }

        Current = Mathf.Lerp(last, target, t);

        // Re-Initialize LerpValue if the interpolationValue has reached its target
        if (interpolationValue == 1)
        {
            Init();
        }
    }
}
