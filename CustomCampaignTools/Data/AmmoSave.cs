using SimpleSerializables.Types;
using Il2CppSLZ.Marrow;

namespace CustomCampaignTools.Data;

public struct AmmoCount
{
    public int LightAmmo { get; set; }
    public int MediumAmmo { get; set; }
    public int HeavyAmmo { get; set; }

    public AmmoCount(int light, int medium, int heavy)
    {
        LightAmmo = light;
        MediumAmmo = medium;
        HeavyAmmo = heavy;
    }

    public readonly int Total => LightAmmo + MediumAmmo + HeavyAmmo;

    public static AmmoCount GetFromPlayer()
    {
        return new AmmoCount()
        {
            LightAmmo = AmmoInventory.Instance.GetCartridgeCount("light"),
            MediumAmmo = AmmoInventory.Instance.GetCartridgeCount("medium"),
            HeavyAmmo = AmmoInventory.Instance.GetCartridgeCount("heavy"),
        };
    }
    
    public void AddToPlayer()
    {
        AmmoInventory ammoInventory = AmmoInventory.Instance;
        ammoInventory.AddCartridge(ammoInventory.lightAmmoGroup, LightAmmo);
        ammoInventory.AddCartridge(ammoInventory.mediumAmmoGroup, MediumAmmo);
        ammoInventory.AddCartridge(ammoInventory.heavyAmmoGroup, HeavyAmmo);
    }

    public static AmmoCount Max(AmmoCount lhs, AmmoCount rhs)
    {
        if(lhs.Total >= rhs.Total)
        {
            return lhs;
        }
        return rhs;
    }

    public static AmmoCount operator +(AmmoCount lhs, AmmoCount rhs)
    {
        return new AmmoCount()
        {
            LightAmmo = lhs.LightAmmo + rhs.LightAmmo,
            MediumAmmo = lhs.MediumAmmo + rhs.MediumAmmo,
            HeavyAmmo = lhs.HeavyAmmo + rhs.HeavyAmmo,
        };
    }

    public static AmmoCount operator -(AmmoCount lhs, AmmoCount rhs)
    {
        return new AmmoCount()
        {
            LightAmmo = lhs.LightAmmo - rhs.LightAmmo,
            MediumAmmo = lhs.MediumAmmo - rhs.MediumAmmo,
            HeavyAmmo = lhs.HeavyAmmo - rhs.HeavyAmmo,
        };
    }
}


public struct LegacyAmmoSave
{
    public BarcodeSer LevelBarcode { get; set; }
    public int LightAmmo { get; set; }
    public int MediumAmmo { get; set; }
    public int HeavyAmmo { get; set; }

    public LegacyAmmoSave()
    {
        
    }
}