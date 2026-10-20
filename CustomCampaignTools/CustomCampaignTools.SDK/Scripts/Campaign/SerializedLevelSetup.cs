#if MELONLOADER
using Il2CppSLZ.Marrow.Warehouse;
#else
using SLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
#endif
using SimpleSerializables.Types;
#if UNITY_EDITOR
using System.Collections.Generic;
#endif

namespace CustomCampaignTools
{
    public class SerializedLevelSetup
    {
        public LevelCrateRefSer levelBarcode = new LevelCrateRefSer(Barcode.EmptyBarcode());
        public string levelName;

        public bool IsValid()
        {
            return levelBarcode.IsValid();
        }

#if UNITY_EDITOR
        public static List<SerializedLevelSetup> CopyFrom(LevelSetup[] levelSetups)
        {
            List<SerializedLevelSetup> list = new();
            foreach (var level in levelSetups)
            {
                list.Add(level.Serialize());
            }
            return list;
        }
#endif
    }
}
