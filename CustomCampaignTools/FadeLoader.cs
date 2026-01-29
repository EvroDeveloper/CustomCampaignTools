using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using System.Collections;
using UnityEngine;
using BoneLib;

namespace CustomCampaignTools
{
    public static class FadeLoader
    {
        public static readonly string LoadFadeBarcode = "c1534c5a-dac0-44a1-b656-6c235646584c";

        public static void Load(Barcode level, Barcode loadScene)
        {
            MelonCoroutines.Start(FadeCoroutine(level, loadScene));
        }

        public static IEnumerator FadeCoroutine(Barcode level, Barcode loadScene)
        {
            HelperMethods.SpawnCrate(LoadFadeBarcode, Vector3.zero);
            yield return new WaitForSeconds(2);
            loadScene ??= new Barcode(CommonBarcodes.Maps.LoadDefault);
            SceneStreamer.Load(level, loadScene);
        }
    }
}