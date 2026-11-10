#if MELONLOADER
using MelonLoader;
#else
#endif
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Stat Manager")]
#endif
    public class StatManager : MonoBehaviour
    {
        public int ReadStat(string key)
        {
#if MELONLOADER
            return Campaign.SessionActive ? Campaign.Session.saveData.ReadStat(key) : 0;
#else
            return 0;
#endif
        } 

        public void IncrementStat(string key)
        {
#if MELONLOADER
            if(Campaign.SessionActive) Campaign.Session.saveData.IncrementStat(key);
#endif
        }

        public void DecrementStat(string key)
        {
#if MELONLOADER
            if(Campaign.SessionActive) Campaign.Session.saveData.DecrementStat(key);
#endif
        }

        public void SetStat(string key, int value)
        {
#if MELONLOADER
            if(Campaign.SessionActive) Campaign.Session.saveData.ForceSetStat(key, value);
#endif
        }
    }
}