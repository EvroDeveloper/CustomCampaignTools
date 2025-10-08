#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppInterop.Runtime.Attributes;
using MelonLoader;
#endif
using System;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Saving/GameObject Enabled Saver")]
#endif
    public class ObjectEnabledSaver : MonoBehaviour
    {
#if MELONLOADER
        public ObjectEnabledSaver(IntPtr ptr) : base(ptr) { }

        public Il2CppValueField<int> uniqueID;

        public void Awake()
        {
            if (SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                RestoreActiveState(Campaign.Session.saveData.LoadedSavePoint);
            }
        }

        public void RestoreActiveState(CampaignSaveData.SavePoint savePoint)
        {
            gameObject.SetActive(savePoint.GetEnabledStateFromID(uniqueID.Get(), gameObject.activeSelf));
        }
#else 
        [Tooltip("A unique ID for this object. Used to identify it in save data. A random ID will be assigned on Reset().")]
        public int uniqueID;
#endif

#if UNITY_EDITOR
        void Reset()
        {
            uniqueID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
#endif
    }
}