using System;
using UnityEngine;

namespace FullSave.Data.SimpleSerializables;

public class TransformSer
{
    public Vector3Ser position;
    public QuaternionSer rotation;
    public Vector3Ser scale;

    public TransformSer() {}

    public TransformSer(Transform transform, bool useLocalValues)
    {
        if(useLocalValues)
        {
            position = new(transform.localPosition);
            rotation = new(transform.localRotation);
        }
        else
        {
            position = new(transform.position);
            rotation = new(transform.rotation);
        }
        // rahh i hate lossy scale none of that fo you
        scale = new(transform.localScale);
    }
}
