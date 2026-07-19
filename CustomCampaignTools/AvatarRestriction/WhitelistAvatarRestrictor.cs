using System.Collections.Generic;
using BoneLib.Notifications;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools.AvatarRestriction
{
    public class WhitelistAvatarRestrictor : AvatarRestrictor
    {
        public List<string> WhitelistedAvatars = [];

        public WhitelistAvatarRestrictor(List<string> whitelistedAvatars)
        {
            WhitelistedAvatars = [.. whitelistedAvatars];
        }

        public override bool IsAvatarAllowed(Barcode avatarBarcode)
        {
            return WhitelistedAvatars.Contains(avatarBarcode.ID);
        }

        public override void OnFailedAvatarSwitch(RigManager rm)
        {
            Notifier.Send(new Notification()
            {
                Title = Campaign.Session.Name,
                Message = "This avatar is not allowed at this time",
                Type = NotificationType.Error,
                ShowTitleOnPopup = true,
            });
        }

        public override void ForceRigManagerAvatar(RigManager rm)
        {
            rm.SwapAvatarCrate(new Barcode(WhitelistedAvatars[0]));
        }
    }
}