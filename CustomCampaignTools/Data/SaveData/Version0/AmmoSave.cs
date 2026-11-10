using SimpleSerializables.Types;

namespace CustomCampaignTools.SaveData.Version0;

public struct AmmoSave
{
    public BarcodeSer LevelBarcode { get; set; }
    public int LightAmmo { get; set; }
    public int MediumAmmo { get; set; }
    public int HeavyAmmo { get; set; }

    public AmmoSave()
    {
        
    }
}