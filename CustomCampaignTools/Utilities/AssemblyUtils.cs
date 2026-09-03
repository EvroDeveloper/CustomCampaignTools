using System;
using System.IO;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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

    public static (Type, T)[] FindTypesWithAttribute<T>(Assembly assembly) where T : Attribute
    {
        List<(Type, T)> types = [];
        ForEachTypeInAssembly(assembly, (type) =>
        {
            if(type.GetCustomAttribute<T>() != null)
            {
                types.Add((type, type.GetCustomAttribute<T>()));
            }
        });
        return [.. types];
    }

    public static (Type, A)[] FindTypesWithAttribute<T, A>(Assembly assembly) where A : Attribute
    {
        List<(Type, A)> types = [];
        ForEachTypeInAssembly(assembly, (type) =>
        {
            if(typeof(T).IsAssignableFrom(type) && type.GetCustomAttribute<A>() != null)
            {
                types.Add((type, type.GetCustomAttribute<A>()));
            }
        });
        return [.. types];
    }

    public static (MethodInfo, A)[] FindMethodsWithAttribute<A>(Assembly assembly, BindingFlags bindingFlags) where A : Attribute
    {
        List<(MethodInfo, A)> methods = [];
        ForEachTypeInAssembly(assembly, (type) =>
        {
            MethodInfo[] typeMethods = type.GetMethods(bindingFlags);
            foreach(MethodInfo method in typeMethods)
            {
                var attribute = method.GetCustomAttribute<A>();
                if(attribute != null)
                {
                    methods.Add((method, attribute));
                }
            }
        });
        return [.. methods];
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
        ForEachTypeInAssembly(assembly, (type) =>
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                ClassInjector.RegisterTypeInIl2Cpp(type);
            }
        });
    }

    public static void ForEachTypeInAssembly(Assembly assembly, Action<Type> typeCallback)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (type.Name.Contains("Mono") && type.Name.Contains("Security"))
            {
                continue;
            }

            typeCallback.Invoke(type);
        }
    }

    public static Assembly LoadEmbeddedAssembly(Assembly assembly, string name)
    {
        byte[] assemblyBytes = ResourceLoader.GetBytes(assembly, name);

        if (assemblyBytes == null) return null;

        return Assembly.Load(assemblyBytes);
    }

    public static T FindInheritingTypeAndCreate<T>(Assembly assembly)
    {
        Type type = FindTypeInAssembly<T>(assembly);
        return (T)Activator.CreateInstance(type);
    }
}
