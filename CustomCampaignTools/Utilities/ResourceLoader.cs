using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomCampaignTools.Utilities;

public static class ResourceLoader
{
    public static byte[] GetBytes(Assembly assembly, string resourceName, bool loosePath = false)
    {
        if(assembly == null)
        {
            throw new NullReferenceException("Attempting to load resource from null Assembly");
        }

        foreach (string resource in assembly.GetManifestResourceNames())
        {
            if ((resource.EndsWith(resourceName) && loosePath) || resource == resourceName)
            {
                using Stream resFilestream = assembly.GetManifestResourceStream(resource);
                if (resFilestream == null) return null;
                byte[] byteArr = new byte[resFilestream.Length];
                resFilestream.Read(byteArr, 0, byteArr.Length);
                return byteArr;
            }
        }
        return null;
    }

    public static Texture2D GetTexture(Assembly assembly, string resourceName, bool loosePath = false)
    {
        byte[] imgBytes =  GetBytes(assembly, resourceName, loosePath);

        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(imgBytes))
        {
            return null;
        }
        texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        return texture;
    }
}
