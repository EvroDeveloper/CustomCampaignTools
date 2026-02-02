using System;
using Il2CppInterop.Runtime;
using UnityEngine;

namespace FullSave.Utilities;

public static class HashingUtility
{
    /// <summary>
    /// Gets an Interger hash of the transform, recursively accounting for Gameobject Name and Sibling Index
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="rootFrame">A reference frame to measure hash against. Will not scan for hashes beyond this parent. Defaults to null, representing a scene-wide hashing scope</param>
    /// <returns>Integer Hash unique to the Transform</returns>
    public static int GetHierarchyHash(Transform transform, Transform rootFrame = null)
    {
        HashCode hash2 = new();

        Transform checkingTransform = transform;
        while(checkingTransform != rootFrame && checkingTransform != null)
        {
            hash2.Add(checkingTransform.gameObject.name);
            if(checkingTransform.parent != null) // I got differing hashes and I think its because of the jumbled nature of the root objects in the scene. Only check sibling index on non-root objects
                hash2.Add(checkingTransform.GetSiblingIndex());
            checkingTransform = checkingTransform.parent;
        }

        return hash2.ToHashCode();
    }

    /// <summary>
    /// Gets an Integer hash of the passed component with respect to the root frame
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <param name="rootFrame">A reference frame to measure hash against. Will not scan for hashes beyond this parent. Defaults to null, representing a scene-wide hashing scope</param>
    /// <returns>Integer hash unique to the component</returns>
    public static int GetComponentHash<T>(T component, Transform rootFrame = null) where T : Component
    {
        HashCode hash2 = new();

        hash2.Add(GetHierarchyHash(component.transform, rootFrame));

        var compList = component.gameObject.GetComponents<T>(); // Need to differentiate if there are multiple of the same component, and add comp index to that hash accordingly
        if(compList.Length > 1)
        {
            hash2.Add(Array.IndexOf(compList, component));
        }
        
        return hash2.ToHashCode();
    }

    /// <summary>
    /// Gets an Integer hash of the passed component with respect to the root frame
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <param name="rootFrame">A reference frame to measure hash against. Will not scan for hashes beyond this parent. Defaults to null, representing a scene-wide hashing scope</param>
    /// <returns>Integer hash unique to the component</returns>
    public static int GetComponentHash(Component component, Transform rootFrame = null)
    {
        HashCode hash2 = new();

        hash2.Add(GetHierarchyHash(component.transform, rootFrame));

        var compList = component.gameObject.GetComponents(component.GetIl2CppType()); // Need to differentiate if there are multiple of the same component, and add comp index to that hash accordingly
        if(compList.Length > 1)
        {
            hash2.Add(Array.IndexOf(compList, component));
        }
        
        return hash2.ToHashCode();
    }

    public static T FindComponentWithMatchingHash<T>(int hash, Transform rootFrame = null) where T : Component
    {
        T[] allFoundComponents;
        if(rootFrame != null)
            allFoundComponents = rootFrame.GetComponentsInChildren<T>();
        else
            allFoundComponents = UnityEngine.Object.FindObjectsOfType<T>();
            
        foreach(T comp in allFoundComponents)
        {
            int componentHash = GetComponentHash(comp, rootFrame); // naive approach, i want to cache values perhaps
            if(componentHash == hash) return comp;
            //hashRememberer.Add(hierarchyHash, comp.gameObject);
        }

        /*
        // old code with caching, ill figure this out later
        if(!componentHashRememberByType.ContainsKey(typeof(T)))
        {
            componentHashRememberByType.Add(typeof(T), []);
        }

        if(hashRememberer.ContainsKey(hash))
        {
            return hashRememberer[hash].GetComponent<T>();
        }
        else
        {
            foreach(T comp in UnityEngine.Object.FindObjectsOfType<T>())
            {
                int hierarchyHash = GetHierarchyHash(comp.transform);
                if(hierarchyHash == hash) return comp;
                hashRememberer.Add(hierarchyHash, comp.gameObject);
            }
        }
        */

        return null;
    }
    
    public static Dictionary<int, GameObject> hashRememberer = [];

    public static Dictionary<Type, Dictionary<int, Component>> componentHashRememberByType = [];

    public static void OnSceneLoaded()
    {
        hashRememberer.Clear();
    }
}
