using System;
using System.Text.Json;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;

namespace CustomCampaignTools.Data.SimpleSerializables;

[JsonConverter(typeof(BarcodeSerConverter))]
public class BarcodeSer
{
    public string ID = string.Empty;

    public BarcodeSer() {}

    public BarcodeSer(Barcode barcode)
    {
        ID = barcode.ID;
    }

    public BarcodeSer(string barcode)
    {
        if(barcode == string.Empty || barcode == null)
            barcode = "null.empty.barcode"; // geh
        ID = barcode;
    }

    public Barcode ToBarcode()
    {
        return new Barcode(ID);
    }

    public static implicit operator string(BarcodeSer b) => b.ID;

    public static implicit operator Barcode(BarcodeSer b) => b.ToBarcode();
}


// Crunch barcodes into a single string in json, allowing for backward compatibility and easy conversion in code.
public class BarcodeSerConverter : JsonConverter<BarcodeSer>
{    
    public override BarcodeSer ReadJson(JsonReader reader, Type objectType, BarcodeSer existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        return new((string)reader.Value);
    }

    public override void WriteJson(JsonWriter writer, BarcodeSer value, Newtonsoft.Json.JsonSerializer serializer)
    {
        writer.WriteValue(value.ID);
    }
}
