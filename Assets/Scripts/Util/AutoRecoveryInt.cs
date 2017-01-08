using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class AutoRecoveryInt
{
    public int max;
    public int value;
    public int recovery;
    public float interval;
    public float time;

    public int GetValue()
    {
        float currentTime = Time.realtimeSinceStartup;
        if (max > value && 0 < interval)
        {
			float deltaTime = (currentTime - time) * Time.timeScale;
            int recoveryCount = (int)(deltaTime / interval);
            value = Math.Min(recoveryCount * recovery + value, max);
            if (value == max)
            {
                time = currentTime;
            }
            else
            {
				time = recoveryCount * (1.0f / Time.timeScale) * interval + time;
            }
        }
        else
        {
            time = currentTime;
        }
        return value;
    }
    public int SetDelta(int delta)
    {
        GetValue();
        value += delta;
        if (0 > value)
        {
            value = 0;
        }
        if (max < value)
        {
            value = max;
        }
        return value;
    }

    public static implicit operator int(AutoRecoveryInt value)
    {
        return value.GetValue();
    }

    public static AutoRecoveryInt operator +(AutoRecoveryInt value, int delta)
    {
        value.SetDelta(delta);
        return value;
    }
    public static AutoRecoveryInt operator -(AutoRecoveryInt value, int delta)
    {
        value.SetDelta(delta * -1);
        return value;
    }
}
