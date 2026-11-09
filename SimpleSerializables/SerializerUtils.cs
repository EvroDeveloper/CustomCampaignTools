using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SimpleSerializables.Utils
{
    public static class SerializerUtils
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects, // Theoretically i shoudnt need this. Might break loading saves but whatever im already doing that to an extent
            Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            }
        };

        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        public static string SaveObjectToFile(object obj, string path)
        {
            string json = SerializeObject(obj);
            File.WriteAllText(path, json);
            return json;
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public static T LoadObjectFromFile<T>(string path)
        {
            string json = File.ReadAllText(path);
            return DeserializeObject<T>(json);
        }

        /// <summary>
        /// Serialize an object with Type Inferrence
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializedObjectInferred(object obj)
        {
            TypeIdWrapper wrapper = new(obj);
            return SerializeObject(wrapper);
        }

        /// <summary>
        /// Deserializes an object of an unknown type, source json must have been generated with type inferrence from <see cref="SerializedObjectInferred"/>
        /// </summary>
        /// <param name="json"></param>
        /// <param name="obj">The deserialized object</param>
        /// <param name="type">The type stored in the json</param>
        /// <returns>Return true if the object could be deserialized with type inferrence</returns>
        public static bool TryDeserializeObjectInferred(string json, out object obj, out Type type)
        {
            TypeIdWrapper wrapper = DeserializeObject<TypeIdWrapper>(json);
            type = wrapper.type;
            obj = wrapper.storedObject;
            return wrapper.DeserializeSucceeded;
        }

        public static bool TryDeserializeObjectInferred<T>(string json, out T obj, out Type type)
        {
            obj = default(T);

            if (TryDeserializeObjectInferred(json, out var objS, out type))
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    obj = (T)objS;
                    return true;
                }
            }
            return false;
        }
    }
}
