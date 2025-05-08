using BoneLib;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools
{
    public static class SavepointFunctions
    {
        public static bool WasLastLoadByContinue = false;
        public static bool CurrentLevelLoadedByContinue = false;

        public static void LoadPlayerFromSave()
        {
            WasLastLoadByContinue = false;

            Campaign campaign = CampaignUtilities.GetFromLevel();

            CampaignSaveData.SavePoint savePoint = campaign.saveData.LoadedSavePoint;

            if (!campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;

            if (hasSpawnPoint)
            {
                Player.RigManager.Teleport(savePoint.GetPosition());

                foreach(CampaignSaveData.BarcodePosRot barcode in savePoint.BoxContainedBarcodes)
                {
                    HelperMethods.SpawnCrate(barcode.barcode, barcode.GetPosition(), barcode.GetRotation(), Vector3.one, spawnAction: ((g) => { MelonLogger.Msg($"Successfully Spawned {barcode} in Box"); }));
                }
            }

            MelonCoroutines.Start(ApplyInventoryDataAfterTime(savePoint.InventoryData));

        }

        public static IEnumerator ApplyInventoryDataAfterTime(InventoryData invData)
        {
            yield return new WaitForSeconds(2);

            invData.ApplyToRigmanager(Player.RigManager);
        }
    }
}
