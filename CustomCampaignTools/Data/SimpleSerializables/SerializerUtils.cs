using System;
using CustomCampaignTools.Debug;
using Newtonsoft.Json;

namespace CustomCampaignTools.Data;

public static class SerializerUtils
{
    private static readonly JsonSerializerSettings _settings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects, // Theoretically i shoudnt need this. Might break loading saves but whatever im already doing that to an extent
        Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
            CampaignLogger.Error($"Error found when serializing path {args.ErrorContext.Path} of Save Data. This property will be reset");
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
}
