using BoneLib.Notifications;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools
{
    public class WhitelistAvatarRestrictor : IAvatarRestrictor
    {
        public List<string> WhitelistedAvatars = [];

        public WhitelistAvatarRestrictor(List<string> whitelistedAvatars)
        {
            WhitelistedAvatars = [.. whitelistedAvatars];
        }
        
        public bool IsAvatarAllowed(Avatar avatar)
        {
            return true;
        }

        public bool IsAvatarAllowed(Barcode avatarBarcode)
        {
            return WhitelistedAvatars.Contains(avatarBarcode.ID);
        }

        public void OnFailedAvatarSwitch(RigManager rm)
        {
            Notifier.Send(new Notification()
            {
                Title = Campaign.Session.Name,
                Message = "This avatar is not allowed at this time",
                Type = NotificationType.Error,
                ShowTitleOnPopup = true,
            });

            rm.SwapAvatarCrate(new Barcode(WhitelistedAvatars[0]));
        }
    }
}