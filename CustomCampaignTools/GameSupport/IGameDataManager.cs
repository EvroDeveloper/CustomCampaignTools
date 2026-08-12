using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.GameSupport;

public interface IGameDataManager
{
    public void TrySaveActiveSave(SaveFlags flags);
    public void ClearUnlockForBarcode(Barcode barcode);
}