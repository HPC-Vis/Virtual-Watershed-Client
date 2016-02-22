using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct SerialVector3
{
    public float x;

    public float y;

    public float z;

    public SerialVector3(UnityEngine.Vector3 rhs)
    {
        x = rhs.x;
        y = rhs.y;
        z = rhs.z;
    }

    public static implicit operator UnityEngine.Vector3(SerialVector3 rhs)
    {
        UnityEngine.Vector3 temp =
        new UnityEngine.Vector3();
        temp.x = rhs.x;
        temp.y = rhs.y;
        return temp;
    }

    public static implicit operator SerialVector3(UnityEngine.Vector3 rhs)
    {
        SerialVector3 temp =
        new SerialVector3();
        temp.x = rhs.x;
        temp.y = rhs.y;
        return temp;
    }

    public static UnityEngine.Vector3[] ToVector2Array(SerialVector3[] vects)
    {
        if (null == vects)
        {
            return null;
        }

        UnityEngine.Vector3[] temp =
        new UnityEngine.Vector3[vects.Length];

        for (int index = vects.Length - 1; index >= 0; --index)
        {
            temp[index] = vects[index];
        }

        return temp;
    }

    public static SerialVector3[] ToVector2Array(UnityEngine.Vector3[] vects)
    {
        if (null == vects)
        {
            return null;
        }

        SerialVector3[] temp =
        new SerialVector3[vects.Length];

        for (int index = vects.Length - 1; index >= 0; --index)
        {
            temp[index] = vects[index];
        }

        return temp;
    }
}
