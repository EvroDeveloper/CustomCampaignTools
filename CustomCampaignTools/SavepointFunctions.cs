using BoneLib;
using CustomCampaignTools.Debug;
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

            if (!savePoint.IsValid(out bool hasSpawnPoint))
                return;

            if (hasSpawnPoint)
            {
                Player.RigManager.Teleport(savePoint.Position, savePoint.GetForwardVector());

                foreach(CampaignSaveData.BarcodePosRot barcode in savePoint.BoxContainedBarcodes)
                {
                    HelperMethods.SpawnCrate(barcode.barcode, barcode.position, barcode.rotation.ToQuaternion(), Vector3.one, spawnAction: (g) => { CampaignLogger.Msg(campaign, $"Successfully Spawned {barcode} in Box"); });
                }
            }

            savePoint.InventoryData.ApplyToRigManagerDelayed();
        }
    }
}
