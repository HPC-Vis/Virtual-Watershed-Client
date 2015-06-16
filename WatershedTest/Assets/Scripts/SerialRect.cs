using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct SerialRect
{
    public float x;

    public float y;
    public float width;
    public float height;

    public SerialRect(UnityEngine.Rect rhs)
    {
        x = rhs.x;
        y = rhs.y;
        width = rhs.width;
        height = rhs.height;

    }

    public static implicit operator UnityEngine.Rect(SerialRect rhs)
    {
        UnityEngine.Rect temp = new UnityEngine.Rect();
        temp.x = rhs.x;
        temp.y = rhs.y;
        temp.width = rhs.width;
        temp.height = rhs.height;
        return temp;
    }

    public static implicit operator SerialRect(UnityEngine.Rect rhs)
    {
        SerialRect temp =
        new SerialRect();
        temp.x = rhs.x;
        temp.y = rhs.y;
        temp.width = rhs.width;
        temp.height = rhs.height;
        return temp;
    }

    public static UnityEngine.Rect[] ToRectArray(SerialRect[] vects)
    {
        if (null == vects)
        {
            return null;
        }

        UnityEngine.Rect[] temp =
        new UnityEngine.Rect[vects.Length];

        for (int index = vects.Length - 1; index >= 0; --index)
        {
            temp[index] = vects[index];
        }

        return temp;
    }

    public static SerialRect[] ToRectArray(UnityEngine.Rect[] vects)
    {
        if (null == vects)
        {
            return null;
        }

        SerialRect[] temp =
        new SerialRect[vects.Length];

        for (int index = vects.Length - 1; index >= 0; --index)
        {
            temp[index] = vects[index];
        }

        return temp;
    }
}
