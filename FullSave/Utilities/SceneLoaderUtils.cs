using System;
using BoneLib;
using FullSave.Data;

namespace FullSave.Utilities;

public class SceneLoaderUtils
{
    public static Action OnNextSceneLoaded;

    public static void OnSceneLoaded(LevelInfo info)
    {
        HashingUtility.OnSceneLoaded();
        
        OnNextSceneLoaded.Invoke();
        OnNextSceneLoaded = null;
    }
}
