using Labworks.Data;
using SLZ.Marrow.SceneStreaming;
using SLZ.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labworks.Patching
{
    internal class ZoneSpawnerPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(ZoneSpawner), "Awake")]
        public static class ZoneSpawnerAwakePatch
        {
            public static void Prefix(ZoneSpawner __instance)
            {
                if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort" && LabworksSaving.IsClassicNPCs)
                {
                    string result = "";

                    switch (__instance.spawnable.crateRef.Barcode)
                    {
                        case "c1534c5a-d82d-4f65-89fd-a4954e756c6c":
                            result = "volx4.LabWorksBoneworksPort.Spawnable.BoneworksNullbody";
                            break;
                        case "c1534c5a-2775-4009-9447-22d94e756c6c":
                            result = "volx4.LabWorksBoneworksPort.Spawnable.BoneworksCorruptedNullbody";
                            break;
                        case "c1534c5a-ef15-44c0-88ae-aebc4e756c6c":
                            result = "volx4.LabWorksBoneworksPort.Spawnable.NullRat";
                            break;
                        case "c1534c5a-2ab7-46fe-b0d6-7495466f7264":
                            result = "volx4.LabWorksBoneworksPort.Spawnable.BoneworksEarlyExit";
                            break;
                        //case "":
                        //    result = "volx4.LabWorksBoneworksPort.Spawnable.BoneworksEarlyExitHeadset";
                        //    break;
                    }

                    if (result != "")
                        __instance.spawnable.crateRef = new SLZ.Marrow.Warehouse.SpawnableCrateReference(result);
                }
            }
        }
    }
}
