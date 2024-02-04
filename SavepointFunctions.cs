using MelonLoader;
using SLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Labworks_Ammo_Saver
{
    internal class SavepointFunctions
    {
        public static bool WasLastLoadByContinue = false;

        public static void SavePlayer(float levelIndex, Vector3 position)
        {
            BonelibCreator.savePoint.Value = new List<float> { levelIndex, position.x, position.y, position.z };

            BonelibCreator.savePoint.Save();
        }


        public static void LoadPlayerFromSave()
        {
            WasLastLoadByContinue = false;

            List<float> saveData = BonelibCreator.savePoint.Value;
            if (saveData[1] == 0 && saveData[2] == 0 && saveData[3] == 0) return;

            BoneLib.Player.rigManager.Teleport(new UnityEngine.Vector3(saveData[1], saveData[2], saveData[3]));
        }

        public static void ClearSavePoint()
        {
            BonelibCreator.savePoint.Value = null;

            BonelibCreator.savePoint.Save();
        }

    }
}
