using BoneLib;
using FullSave.BoneMenu;
using FullSave.Utilities;
using MelonLoader;

namespace FullSave;

public class Main : MelonMod
{
    public override void OnLateInitializeMelon()
    {
        Hooking.OnLevelLoaded += SceneLoaderUtils.OnSceneLoaded;

        // Load saves from filez
        SaveStack.LoadSavesFromFiles();
        BoneMenuCreator.CreateBoneMenu();
    }
}
