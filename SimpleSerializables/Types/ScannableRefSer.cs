#if MELONLOADER
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
#else
using SLZ.Marrow.Warehouse;
#endif
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSerializables.Types
{
    [JsonConverter(typeof(ScannableRefSerConverter))]
    public class ScannableRefSer<T> where T : ScannableReference
    {
        public Barcode barcode;

        protected T _scannableReference;

        public ScannableRefSer() { barcode = Barcode.EmptyBarcode(); }
        public ScannableRefSer(string barcode)
        {
            this.barcode = new(barcode);
        }

        public ScannableRefSer(Barcode barcode)
        {
            this.barcode = barcode;
        }

        public ScannableRefSer(T reference)
        {
            this.barcode = reference.Barcode;
        }

        public virtual T ToScannableReference()
        {
            if(_scannableReference == null)
            {
                _scannableReference = (T)Activator.CreateInstance(typeof(T)); // i hate it here
                _scannableReference.Barcode = this.barcode;
            }
            return _scannableReference;
        }

        public bool IsValid()
        {
            if (!barcode.IsValid()) return false;
            return IsPresentInWarehouse();
        }

        public bool IsPresentInWarehouse()
        {
            return MarrowGame.assetWarehouse.HasScannable(barcode);
        }

        public override string ToString()
        {
            return barcode.ID;
        }

        public static implicit operator T(ScannableRefSer<T> b) => b.ToScannableReference();
        public static implicit operator Barcode(ScannableRefSer<T> b) => b.barcode;
        public static implicit operator string(ScannableRefSer<T> b) => b.barcode.ID;
    }

    [JsonConverter(typeof(ScannableRefSerConverter))]
    public class CrateRefSer<T> : ScannableRefSer<T> where T : ScannableReference
    {
        public CrateRefSer() : base() {}
        public CrateRefSer(string barcode) : base(barcode) {}
        public CrateRefSer(Barcode barcode) : base(barcode) {}
        public CrateRefSer(T crateReference) : base(crateReference) {}
    }

    [JsonConverter(typeof(ScannableRefSerConverter))]
    public class SpawnableCrateRefSer : CrateRefSer<SpawnableCrateReference>
    {
        public SpawnableCrateRefSer() : base() {}
        public SpawnableCrateRefSer(string barcode) : base(barcode) {}
        public SpawnableCrateRefSer(Barcode barcode) : base(barcode) {}
        public SpawnableCrateRefSer(SpawnableCrateReference crateReference) : base(crateReference) {} 

        public override SpawnableCrateReference ToScannableReference()
        {
            _scannableReference ??= new SpawnableCrateReference(barcode);
            return _scannableReference;
        }
    }

    [JsonConverter(typeof(ScannableRefSerConverter))]
    public class AvatarCrateRefSer : CrateRefSer<AvatarCrateReference>
    {
        public AvatarCrateRefSer() : base() {}
        public AvatarCrateRefSer(string barcode) : base(barcode) {}
        public AvatarCrateRefSer(Barcode barcode) : base(barcode) {}
        public AvatarCrateRefSer(AvatarCrateReference crateReference) : base(crateReference) {} 

        public override AvatarCrateReference ToScannableReference()
        {
            _scannableReference ??= new AvatarCrateReference(barcode);
            return _scannableReference;
        }
    }

    public class LevelCrateRefSer : CrateRefSer<LevelCrateReference>
    {
        public LevelCrateRefSer() : base() {}
        public LevelCrateRefSer(string barcode) : base(barcode) {}
        public LevelCrateRefSer(Barcode barcode) : base(barcode) {}
        public LevelCrateRefSer(LevelCrateReference crateReference) : base(crateReference) {}

        public override LevelCrateReference ToScannableReference()
        {
            _scannableReference ??= new LevelCrateReference(barcode);
            return _scannableReference;
        }
    }

    [JsonConverter(typeof(ScannableRefSerConverter))]
    public class DataCardRefSer<T> : ScannableRefSer<T> where T : ScannableReference
    {
        public DataCardRefSer() : base() {}
        public DataCardRefSer(string barcode) : base(barcode) {}
        public DataCardRefSer(Barcode barcode) : base(barcode) {}
        public DataCardRefSer(T crateReference) : base(crateReference) {}
    }

    [JsonConverter(typeof(ScannableRefSerConverter))]
    public class MonoDiscRefSer : DataCardRefSer<MonoDiscReference>
    {
        public MonoDiscRefSer() : base() {}
        public MonoDiscRefSer(string barcode) : base(barcode) {}
        public MonoDiscRefSer(Barcode barcode) : base(barcode) {}
        public MonoDiscRefSer(MonoDiscReference crateReference) : base(crateReference) {}

        public override MonoDiscReference ToScannableReference()
        {
            _scannableReference ??= new MonoDiscReference(barcode);
            return _scannableReference;
        }
    }

    public class ScannableRefSerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if(objectType.IsAssignableTo(typeof(ScannableRefSer<>))) return true;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            string str = (string)reader.Value;

            var instance = Activator.CreateInstance(objectType);
            objectType.GetField("barcode").SetValue(instance, new Barcode(str));

            return instance;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
