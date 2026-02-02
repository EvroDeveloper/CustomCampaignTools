using System;
using System.Reflection;
using FullSave.Utilities;
using Il2CppInterop.Runtime;
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

    public Component[] GetComponentsInChildren(GameObject gameObject);
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

    public Component[] GetComponentsInChildren(GameObject gameObject)
    {
        return gameObject.GetComponentsInChildren<TComponent>();
    }
}

public static class ComponentSaverManager
{
    private static Dictionary<Type, IComponentSaver> componentSaversByType = [];

    public static void InitializeSavers()
    {
        var ass = Assembly.GetAssembly(typeof(IComponentSaver)); //  i hate reflecting
        var compSavers = ass.GetTypes().Where((t) => t.IsAssignableTo(typeof(IComponentSaver)));
        foreach(Type compSaver in compSavers)
        {
            var attribute = compSaver.GetCustomAttribute<ComponentSaverAttribute>();
            if(attribute == null) continue;

            componentSaversByType.Add(attribute.type, (IComponentSaver)Activator.CreateInstance(compSaver));
            MelonLogger.Msg($"Found Component Saver {compSaver.Name}");
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

    public static SavedComponent[] SaveComponentsInChildren(GameObject gameObject)
    {
        List<SavedComponent> savedComponents = [];
        foreach(Type type in GetValidComponentTypes())
        {
            IComponentSaver saver = GetComponentSaver(type);
            foreach(Component c in saver.GetComponentsInChildren(gameObject))
            {
                savedComponents.Add(new(c, saver, gameObject.transform));
            }
        }
        return [.. savedComponents];
    }
}

public class SavedComponent
{
    public int componentHash;
    public string componentType;
    public byte[] savedState;

    public SavedComponent()
    {
        
    }

    public SavedComponent(Component component, IComponentSaver componentSaver, Transform root = null)
    {
        componentHash = HashingUtility.GetComponentHash(component, root);
        componentType = component.GetType().FullName;
        savedState = componentSaver.SaveComponentState(component);
    }

    public void ApplySave(GameObject referenceEntity = null)
    {
        // Evaluate Type from componentType string
        Type realComponentType = Type.GetType(componentType);
        // Find ComponentSaver type for handling a LOT of things
        IComponentSaver componentSaver = ComponentSaverManager.GetComponentSaver(realComponentType);
        if(componentSaver == null)
        {
            MelonLogger.Error($"Uhh couldnt find a component saver for type {componentType}");
            return;
        }
        componentSaver.RestoreComponentState(referenceEntity.transform, componentHash, savedState); // i dont like this, generics pmo
    }
}