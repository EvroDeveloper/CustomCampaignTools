using System;
using Il2CppSLZ.Marrow.Utilities;
using UnityEngine;

namespace CustomCampaignTools.Data.SimpleSerializables;

public class SimpleTransformSer
{
    public Vector3Ser position;
    public QuaternionSer rotation;

    public SimpleTransformSer() {}
    
    public SimpleTransformSer(SimpleTransform transform)
    {
        position = new(transform.position);
        rotation = new(transform.rotation);
    }

    public SimpleTransformSer(Transform transform)
    {
        position = new(transform.position);
        rotation = new(transform.rotation);
    }

    public SimpleTransform ToSimpleTransform()
    {
        return new SimpleTransform(position, rotation.ToQuaternion());
    }
}
