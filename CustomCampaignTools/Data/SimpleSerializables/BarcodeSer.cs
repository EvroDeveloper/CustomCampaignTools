using System;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Data.SimpleSerializables;

public class BarcodeSer
{
    public string ID = "";

    public BarcodeSer() {}

    public BarcodeSer(Barcode barcode)
    {
        ID = barcode.ID;
    }

    public Barcode ToBarcode()
    {
        return new Barcode(ID);
    }
}
