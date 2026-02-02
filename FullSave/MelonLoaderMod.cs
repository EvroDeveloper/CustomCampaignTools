using BoneLib;
using FullSave.BoneMenu;
using FullSave.ComponentSavers;
using FullSave.Utilities;
using MelonLoader;

namespace FullSave;

public class Main : MelonMod
{
    public override void OnLateInitializeMelon()
    {
        Hooking.OnLevelLoaded += SceneLoaderUtils.OnSceneLoaded;

        ComponentSaverManager.InitializeSavers();

        // Load saves from filez
        SaveStack.LoadSavesFromFiles();
        BoneMenuCreator.CreateBoneMenu();
    }
}
