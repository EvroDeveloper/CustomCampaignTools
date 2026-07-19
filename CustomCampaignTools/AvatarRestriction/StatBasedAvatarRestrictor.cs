using UnityEngine;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;
using BoneLib.Notifications;
using BoneLib;

namespace CustomCampaignTools.AvatarRestriction
{
    public class StatBasedAvatarRestrictor : AvatarRestrictor
    {
        public StatBasedAvatarRestrictor(AvatarStatRanges statRanges)
        {
            _avatarHeightRange = new Vector2(statRanges.heightRangeLow, statRanges.heightRangeHigh);
            _avatarMassRange = new Vector2(statRanges.massRangeLow, statRanges.massRangeHigh);
            _avatarArmLengthRange = new Vector2(statRanges.armRangeLow, statRanges.armRangeHigh);
        }

        private Vector2 _avatarHeightRange;
        private Vector2 _avatarMassRange;
        private Vector2 _avatarArmLengthRange;

        public override bool IsAvatarAllowed(Avatar avatar)
        {
            return IsInRange(avatar.height, _avatarHeightRange) && IsInRange(avatar.massTotal, _avatarMassRange) && IsInRange(avatar.armLength, _avatarArmLengthRange);
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

        bool IsInRange(float value, Vector2 range)
        {
            return range.x <= value && value <= range.y;
        }

        public override void ForceRigManagerAvatar(RigManager rm)
        {
            rm.SwapAvatarCrate(new Barcode(CommonBarcodes.Avatars.PolyBlank));
        }
    }
}