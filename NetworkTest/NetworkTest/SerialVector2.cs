using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct SerialVector2
{
    public float x;

    public float y;

    public SerialVector2(UnityEngine.Vector2 rhs)
    {
        x = rhs.x;
        y = rhs.y;
    }

    public static implicit operator UnityEngine.Vector2(SerialVector2 rhs)
    {
        UnityEngine.Vector2 temp =
        new UnityEngine.Vector2();
        temp.x = rhs.x;
        temp.y = rhs.y;
        return temp;
    }

    public static implicit operator SerialVector2(UnityEngine.Vector2 rhs)
    {
        SerialVector2 temp =
        new SerialVector2();
        temp.x = rhs.x;
        temp.y = rhs.y;
        return temp;
    }

    public static UnityEngine.Vector2[] ToVector2Array(SerialVector2[] vects)
    {
        if (null == vects)
        {
            return null;
        }

        UnityEngine.Vector2[] temp =
        new UnityEngine.Vector2[vects.Length];

        for (int index = vects.Length - 1; index >= 0; --index)
        {
            temp[index] = vects[index];
        }

        return temp;
    }

    public static SerialVector2[] ToVector2Array(UnityEngine.Vector2[] vects)
    {
        if (null == vects)
        {
            return null;
        }

        SerialVector2[] temp =
        new SerialVector2[vects.Length];

        for (int index = vects.Length - 1; index >= 0; --index)
        {
            temp[index] = vects[index];
        }

        return temp;
    }
}
