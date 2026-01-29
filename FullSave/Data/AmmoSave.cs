using System;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using UnityEngine;

namespace FullSave.Data;
    
public struct AmmoSave
{
    public string LevelBarcode { get; set; }
    public int LightAmmo { get; set; }
    public int MediumAmmo { get; set; }
    public int HeavyAmmo { get; set; }

    public int GetCombinedTotal()
    {
        return LightAmmo + MediumAmmo + HeavyAmmo;
    }

    public AmmoSave()
    {
        
    }

    public AmmoSave(int light, int medium, int heavy)
    {
        LightAmmo = light;
        MediumAmmo = medium;
        HeavyAmmo = heavy;
    }

    public AmmoSave(string barcode, int light, int medium, int heavy)
    {
        LevelBarcode = barcode;
        LightAmmo = light;
        MediumAmmo = medium;
        HeavyAmmo = heavy;
    }

    public void AddToPlayer()
    {
        AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, LightAmmo);
        AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, MediumAmmo);
        AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, HeavyAmmo);
    }

    public static AmmoSave CreateFromPlayer(string levelBarcodeOverride = null)
    {
        string levelBarcode = SceneStreamer.Session.Level.Barcode.ID;
        if(string.IsNullOrEmpty(levelBarcodeOverride))
        {
            levelBarcode = levelBarcodeOverride;
        }
        return new AmmoSave()
        {
            LevelBarcode = levelBarcode,
            LightAmmo = AmmoInventory.Instance.GetCartridgeCount("light"),
            MediumAmmo = AmmoInventory.Instance.GetCartridgeCount("medium"),
            HeavyAmmo = AmmoInventory.Instance.GetCartridgeCount("heavy"),
        };
    }

    public static AmmoSave SumOfBest(AmmoSave save1, AmmoSave save2)
    {
        return new AmmoSave()
        {
            LevelBarcode = save1.LevelBarcode,
            LightAmmo = Mathf.Max(save1.LightAmmo, save2.LightAmmo),
            MediumAmmo = Mathf.Max(save1.MediumAmmo, save2.MediumAmmo),
            HeavyAmmo = Mathf.Max(save1.HeavyAmmo, save2.HeavyAmmo),
        };
    }

    public static AmmoSave operator -(AmmoSave lhs, AmmoSave rhs)
    {
        return new AmmoSave()
        {
            LevelBarcode = lhs.LevelBarcode,
            LightAmmo = lhs.LightAmmo - rhs.LightAmmo,
            MediumAmmo = lhs.MediumAmmo - rhs.MediumAmmo,
            HeavyAmmo = lhs.HeavyAmmo - rhs.HeavyAmmo,
        };
    }
}
