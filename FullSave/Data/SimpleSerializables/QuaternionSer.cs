using System;
using UnityEngine;

namespace FullSave.Data.SimpleSerializables;

public struct QuaternionSer
{
    public float x;
    public float y;
    public float z;
    public float w;

    public QuaternionSer() {}

    public QuaternionSer(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}
