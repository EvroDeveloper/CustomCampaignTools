using Labworks.Data;
using Labworks.Utilities;
using MelonLoader;
using SLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Labworks
{
    internal class SavepointFunctions
    {
        public static bool WasLastLoadByContinue = false;

        public static void SavePlayer(string levelBarcode, Vector3 position)
        {
            LabworksSaving.LoadedSavePoint = new LabworksSaving.SavePoint(levelBarcode, position);

            LabworksSaving.SaveToDisk();
        }


        public static void LoadPlayerFromSave()
        {
            WasLastLoadByContinue = false;

            if (!SaveParsing.IsSavePointValid(LabworksSaving.LoadedSavePoint, out bool hasSpawnPoint))
                return;

            if (hasSpawnPoint)
            {
                LabworksSaving.SavePoint savePoint = LabworksSaving.LoadedSavePoint;

                BoneLib.Player.rigManager.Teleport(new Vector3(savePoint.PositionX, savePoint.PositionY, savePoint.PositionZ));
            }
        }

        public static void ClearSavePoint()
        {
            LabworksSaving.LoadedSavePoint = new LabworksSaving.SavePoint(string.Empty, Vector3.zero);

            LabworksSaving.SaveToDisk();
        }

    }
}
