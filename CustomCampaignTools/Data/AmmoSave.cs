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

    public AmmoCount(AmmoInventory ammoInventory)
    {
        LightAmmo = ammoInventory.GetCartridgeCount("light");
        MediumAmmo = ammoInventory.GetCartridgeCount("medium");
        HeavyAmmo = ammoInventory.GetCartridgeCount("heavy");
    }

    public readonly int Total => LightAmmo + MediumAmmo + HeavyAmmo;

    public static AmmoCount GetFromPlayer()
    {
        return AmmoInventory.Instance.GetAmmoCount();
    }
    
    public void AddToPlayer()
    {
        AmmoInventory.Instance.AddAmmoCount(this);
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

public static class AmmoInventoryExtensions
{
    public static void AddAmmoCount(this AmmoInventory ammoInventory, AmmoCount ammoCount)
    {
        ammoInventory.AddCartridge(ammoInventory.lightAmmoGroup, ammoCount.LightAmmo);
        ammoInventory.AddCartridge(ammoInventory.mediumAmmoGroup, ammoCount.MediumAmmo);
        ammoInventory.AddCartridge(ammoInventory.heavyAmmoGroup, ammoCount.HeavyAmmo);
    }

    public static AmmoCount GetAmmoCount(this AmmoInventory ammoInventory)
    {
        return new AmmoCount(ammoInventory);
    }

    public static void SetAmmoCount(this AmmoInventory ammoInventory, AmmoCount ammoCount)
    {
        ammoInventory.ClearAmmo();
        ammoInventory.AddAmmoCount(ammoCount);
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