using BoneLib;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools
{
    public static class SavepointFunctions
    {
        public static bool WasLastLoadByContinue = false;
        public static bool LoadByContine_AmmoPatchHint = false; // Used for letting the AmmoInventory patcher know when we should give SavePoint ammo
        public static bool LoadByContinue_ObjectEnabledHint = false;
        public static bool LoadByContinue_SaveDespawnHint = false;

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

            MelonCoroutines.Start(ApplyInventoryDataAfterTime(savePoint));

        }

        public static IEnumerator ApplyInventoryDataAfterTime(CampaignSaveData.SavePoint savePoint)
        {
            yield return new WaitForSeconds(2);

            savePoint.InventoryData.ApplyToRigmanager(Player.RigManager);
        }
    }
}
