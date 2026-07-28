using System;
using System.Collections.Generic;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;
using SimpleSerializables.Types;

namespace CustomCampaignTools;

public partial class CampaignSaveData
{
    [JsonIgnore] // Not complete feature yet. Planned for 1.3.0
    public HashSet<BarcodeSer> CampaignSpecificUnlocks = [];

    public void UnlockSpawnable(Barcode barcode)
    {
        CampaignSpecificUnlocks.Add(new(barcode));
        SaveToDisk();
    }

    public void UnlockSpawnable(SpawnableCrateReference crateRef)
    {
        CampaignSpecificUnlocks.Add(new(crateRef.Barcode));
        SaveToDisk();
    }

    public void UnlockSpawnable(MarrowEntity entity)
    {
        if(entity._poolee.SpawnableCrate == null) return;
        CampaignSpecificUnlocks.Add(new(entity._poolee.SpawnableCrate.Barcode));
        SaveToDisk();
    }

    public void LockSpawnable(Barcode barcode)
    {
        CampaignSpecificUnlocks.RemoveWhere(b => b.ID == barcode.ID);
        SaveToDisk();
    }
}
