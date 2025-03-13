using System;
using UnityEngine;
using MelonLoader;
using BoneLib;
using Il2CppSLZ.Bonelab;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class CampaignUnlocking : MonoBehaviour
    {
        public CampaignUnlocking(IntPtr ptr) : base(ptr) {}

        public void UnlockDevTools(bool enableInstantly)
        {
            Campaign.Session.saveData.UnlockDevTools();

            if(enableInstantly)
            {
                UIRig.Instance.popUpMenu.AddDevMenu(null);
            }
        }

        public void UnlockAvatars(bool enableInstantly)
        {
            Campaign.Session.saveData.UnlockAvatar();

            if(!enableInstantly) return;

            PullCordDevice bodyLog = Player.PhysicsRig.GetComponentInChildren<PullCordDevice>(true);
            if(bodyLog != null)
            {
                bodyLog.gameObject.SetActive(true);
            }

            UIRig.Instance.popUpMenu.AddAvatarsMenu();
        }

        public void UnlockLevel(string barcode)
        {
            Campaign.Session.saveData.UnlockLevel(barcode);
        }

        public void ResetSave()
        {
            Campaign.Session.saveData.ResetSave();
        }
    }
}