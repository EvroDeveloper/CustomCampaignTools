#if MELONLOADER
using MelonLoader;
using BoneLib;
using Il2CppSLZ.Bonelab;
#endif
using System;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Campaign Unlocking")]
#endif
    public class CampaignUnlocking : MonoBehaviour
    {
#if MELONLOADER
        public CampaignUnlocking(IntPtr ptr) : base(ptr) { }
#endif
        public void UnlockDevTools(bool enableInstantly)
        {
#if MELONLOADER
            Campaign.Session.saveData.UnlockDevTools();

            if(enableInstantly)
            {
                UIRig.Instance.popUpMenu.AddDevMenu(null);
            }
#endif
        }

        public void UnlockAvatars(bool enableInstantly)
        {
#if MELONLOADER
            Campaign.Session.saveData.UnlockAvatar();

            if(!enableInstantly) return;

            PullCordDevice bodyLog = Player.PhysicsRig.GetComponentInChildren<PullCordDevice>(true);
            bodyLog?.gameObject.SetActive(true);

            UIRig.Instance.popUpMenu.AddAvatarsMenu();
#endif
        }

        public void UnlockLevel(string barcode)
        {
#if MELONLOADER
            Campaign.Session.saveData.UnlockLevel(barcode);
#endif
        }

        public void SetIntroSkip(bool skipIntro)
        {
#if MELONLOADER
            Campaign.Session.saveData.SkipIntro = skipIntro;
#endif
        }

        public void ResetSave()
        {
#if MELONLOADER
            Campaign.Session.saveData.ResetSave();
#endif
        }

        public void UNLOCKALL()
        {
#if MELONLOADER
            UnlockDevTools(true);
            UnlockAvatars(true);
            foreach (var level in Campaign.Session.AllLevels)
            {
                UnlockLevel(level.BarcodeString);
            }
#endif
        }
    }
}