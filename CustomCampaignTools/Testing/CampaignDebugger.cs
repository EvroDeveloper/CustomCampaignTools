using System;
using System.Collections.Generic;
using System.Reflection;

namespace CustomCampaignTools.Debug
{
    public static class CampaignDebugger
    {
        public static string[] GetAllNullFieldsInObject<T>(T obj)
        {
            List<string> outputList = [];
            Type type = typeof(T);
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic);
            foreach(FieldInfo field in fieldInfos)
            {
                try
                {
                    var value = field.GetValue(obj);
                    if (value == null)
                    {
                        outputList.Add(field.Name);
                    }
                }
                catch { }
            }
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic);
            foreach(PropertyInfo property in propertyInfos)
            {
                try
                {
                    var value = property.GetValue(obj);
                    if (value == null)
                    {
                        outputList.Add(property.Name);
                    }
                }
                catch { }
            }
            return outputList.ToArray();
        }

        public static string LogFieldValue<T>(T obj, string fieldName)
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField(fieldName);
            PropertyInfo prop = type.GetProperty(fieldName);
            if(field != null)
                return $"{fieldName} = {field.GetValue(obj) ?? "null"}";
            else if(prop != null)
                return $"{fieldName} = {prop.GetValue(obj) ?? "null"}";
            else
                return $"Could not find field or property named {fieldName}";
        }
    }
}
