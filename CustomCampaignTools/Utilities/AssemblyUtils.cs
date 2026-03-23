using System;
using System.IO;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace CustomCampaignTools.Utilities;

public class AssemblyUtils
{
    public static Type FindTypeInAssembly<T>(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (type.Name.Contains("Mono") && type.Name.Contains("Security"))
            {
                continue;
            }

            if (typeof(T).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                return type;
            }
        }
        return null;
    }

    public static byte[] LoadBytesFromAssembly(Assembly assembly, string name)
    {
        string[] manifestResources = assembly.GetManifestResourceNames();

        if (!manifestResources.Contains(name))
        {
            return null;
        }

        using (Stream str = assembly.GetManifestResourceStream(name))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                str.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public static void HarmonyPatchAssembly(Assembly assembly, string harmonyInstanceName) => HarmonyLib.Harmony.CreateAndPatchAll(assembly, harmonyInstanceId: harmonyInstanceName);
    public static void RegisterAssemblyMonoBehaviours(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (type.Name.Contains("Mono") && type.Name.Contains("Security"))
            {
                continue;
            }

            if (typeof(MonoBehaviour).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
        }
    }

    public static Assembly LoadEmbeddedAssembly(Assembly assembly, string name)
    {
        byte[] assemblyBytes = ResourceLoader.GetBytes(assembly, name);

        if (assemblyBytes == null) return null;

        return Assembly.Load(assemblyBytes);
    }
}
