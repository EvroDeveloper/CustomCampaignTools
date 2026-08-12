using CustomCampaignTools.GameSupport;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCampaignTools.BonelabSupport;

public class BoneLabDataManager : IGameDataManager
{
    public void ClearUnlockForBarcode(Barcode barcode)
    {
        DataManager.Instance._activeSave.Unlocks.ClearUnlockForBarcode(barcode);
    }

    public void TrySaveActiveSave(SaveFlags flags)
    {
        DataManager.TrySaveActiveSave(flags);
    }
}
