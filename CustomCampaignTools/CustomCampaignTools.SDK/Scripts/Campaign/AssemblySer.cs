#if MELONLOADER
using CustomCampaignTools.Debug;
#else
using SLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
#endif
using Newtonsoft.Json;
using System.Reflection;
using System;
using UnityEngine;

namespace CustomCampaignTools
{
    public class AssemblySer
    {
        public byte[] assemblyBytes;

        [JsonIgnore]
        private Assembly _assembly;

        public AssemblySer() { }

        public AssemblySer(TextAsset assemblyTextAsset)
        {
            assemblyBytes = assemblyTextAsset.bytes;
        }

#if MELONLOADER
        public Assembly LoadAssembly(Action<Assembly> callback)
        {
            if (!IsValid) return null;
            if(_assembly == null)
            {
                try
                {
                    _assembly = Assembly.Load(assemblyBytes);
                    callback.Invoke(_assembly);
                }
                catch (Exception ex)
                {
                    CampaignLogger.Error($"Unable to load Assembly: {ex}");
                }
            }
            return _assembly;
        }
#endif

        public bool IsValid
        {
            get
            {
                if (assemblyBytes == null || assemblyBytes.Length == 0) return false;
                return true;
            }
        }
    }
}
