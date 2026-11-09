using System;
using System.Runtime.Serialization;

namespace SimpleSerializables;

[Serializable]
public class TypeIdWrapper : ISerializable
{
    public Type type;
    public object storedObject;
    public bool DeserializeSucceeded;

    public TypeIdWrapper(object sourceObject)
    {
        type = sourceObject.GetType();
        storedObject = sourceObject;
        DeserializeSucceeded = true;
    }

    public TypeIdWrapper(SerializationInfo info, StreamingContext context)
    {
        if(IsInfoOfType(info))
        {
            string typeString = info.GetString("typeName");
            type = Type.GetType(typeString);
            storedObject = info.GetValue("storedObject", type);
            DeserializeSucceeded = true;
        }
        else
        {
            DeserializeSucceeded = false;
        }
    }

    private static bool IsInfoOfType(SerializationInfo info)
    {
        if(info.MemberCount != 2) return false;
        bool hasTypeName = false;
        bool hasStoredObject = false;
        foreach(var thingy in info)
        {
            if(thingy.Name == "typeName") hasTypeName = true;
            if(thingy.Name == "storedObject") hasStoredObject = true;
        }
        return hasTypeName && hasStoredObject;
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("typeName", type.AssemblyQualifiedName);
        info.AddValue("storedObject", storedObject);
    }
}