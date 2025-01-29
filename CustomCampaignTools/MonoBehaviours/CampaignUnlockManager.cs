using System;
using UnityEngine;
using MelonLoader;
using BoneLib;
using Il2CppSLZ.Bonelab;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class CampaignUnlockManager : MonoBehaviour
    {
        public CampaignUnlockManager(IntPtr ptr) : base(ptr) {}

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
    }
}