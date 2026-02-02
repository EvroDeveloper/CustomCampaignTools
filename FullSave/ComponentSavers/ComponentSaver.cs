using System;
using System.Reflection;
using FullSave.Utilities;
using MelonLoader;
using UnityEngine;

namespace FullSave.ComponentSavers;

public class ComponentSaverAttribute : Attribute
{
    public Type type;

    public ComponentSaverAttribute(Type type)
    {
        this.type = type;
    }
}

public interface IComponentSaver
{
    public byte[] SaveComponentState(Component obj);

    public void RestoreComponentState(Component obj, byte[] stateData);

    public void RestoreComponentState(Transform root, int componentHash, byte[] stateData);
}

public class ComponentSaver<TComponent> : IComponentSaver where TComponent : Component
{
    public byte[] SaveComponentState(Component obj)
    {
        if(obj is TComponent comp)
            return SaveComponentState(comp);
        else
            return [];
    }

    public void RestoreComponentState(Component obj, byte[] stateData)
    {
        if(obj is TComponent comp)
            RestoreComponentState(comp, stateData);
        else
            throw new ArgumentException("nah that component aint right... hmm");
    }

    public void RestoreComponentState(Transform root, int componentHash, byte[] stateData)
    {
        TComponent foundComponent = HashingUtility.FindComponentWithMatchingHash<TComponent>(componentHash, root);
        if(foundComponent == null)
        {
            MelonLogger.Error($"Could not find component of type {typeof(TComponent).Name}, hash {componentHash}, and root {root.gameObject.name}. Skipping...");
            return;
        }
        RestoreComponentState(foundComponent, stateData);
    }

    public virtual byte[] SaveComponentState(TComponent component) { return []; }

    public virtual void RestoreComponentState(TComponent component, byte[] stateData) { }
}

public static class ComponentSaverManager
{
    public static Dictionary<Type, IComponentSaver> componentSaversByType = [];

    public static void InitializeSavers()
    {
        var ass = Assembly.GetAssembly(typeof(IComponentSaver)); //  i hate reflecting
        var compSavers = ass.GetTypes().Where((t) => t.IsAssignableTo(typeof(IComponentSaver)));
        foreach(Type compSaver in compSavers)
        {
            var attribute = compSaver.GetCustomAttribute<ComponentSaverAttribute>();
            if(attribute == null) continue;

            componentSaversByType.Add(attribute.type, (IComponentSaver)Activator.CreateInstance(compSaver));
        }
    }

    public static IComponentSaver GetComponentSaver(Type componentType)
    {
        if(componentSaversByType.ContainsKey(componentType))
        {
            return componentSaversByType[componentType];
        }
        return null;
    }

    public static List<Type> GetValidComponentTypes()
    {
        return [..componentSaversByType.Keys];
    }
}
