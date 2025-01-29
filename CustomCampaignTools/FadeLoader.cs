using MelonLoader;
using System;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools
{
    public class FadeLoader
    {
        public static readonly string LoadFadeBarcode = "none.empty.barcode";

        public static void Load(Barcode level, Barcode loadScene)
        {
            MelonCoroutines.StartCoroutine(FadeCoroutine(level, loadScene));
        }

        public static IEnumerator FadeCoroutine(Barcode level, Barcode loadScene)
        {
            BoneLib.HelperMethods.SpawnCrate(LoadFadeBarcode, Vector3.zero);
            yield return new WaitForSeconds(2);
            SceneStreamer.Load(level, loadScene);
        }
    }
}