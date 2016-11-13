using UnityEngine;
using System;

public enum RunMode
{
    Play = 0,
    Editor,
    Any
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class ReadOnlyAttribute : PropertyAttribute
{
    public readonly RunMode scope = RunMode.Play;

    public ReadOnlyAttribute(RunMode scope = default(RunMode))
    {
        this.scope = scope;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class ReadOnlyRangeAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;

    public ReadOnlyRangeAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

