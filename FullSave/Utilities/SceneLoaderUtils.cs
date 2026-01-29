using System;
using BoneLib;

namespace FullSave.Utilities;

public class SceneLoaderUtils
{
    public static Action OnNextSceneLoaded;

    public static void OnSceneLoaded(LevelInfo info)
    {
        OnNextSceneLoaded.Invoke();
        OnNextSceneLoaded = null;
    }
}
