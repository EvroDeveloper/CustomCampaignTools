using System;
using Newtonsoft.Json;
using UnityEngine;

namespace FullSave.Data.SimpleSerializables;

public struct Vector3Ser
{
    public float x;
    public float y;
    public float z;

    public Vector3Ser() {}

    public Vector3Ser(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
