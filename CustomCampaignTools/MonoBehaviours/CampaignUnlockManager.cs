using System;
using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow;
using BoneLib;

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
                UIRig.Instance.popUpMenu.AddDevMenu();
            }
        }

        public void UnlockAvatars(bool enableInstantly)
        {
            Campaign.Session.saveData.UnlockAvatars();

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