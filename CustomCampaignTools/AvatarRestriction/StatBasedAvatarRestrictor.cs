using UnityEngine;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;
using BoneLib.Notifications;
using BoneLib;
using CustomCampaignTools.Debug;

namespace CustomCampaignTools.AvatarRestriction;

public class StatBasedAvatarRestrictor : DefaultAvatarRestrictor
{
    public StatBasedAvatarRestrictor(AvatarStatRanges statRanges, AvatarCrateReference defaultCampaignAvatar, AvatarCrateReference fallbackAvatar) : base(defaultCampaignAvatar, fallbackAvatar)
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

    public override bool IsAvatarAllowed(Barcode avatarBarcode)
    {
        // only stat restriciton here, dont use default's barcode restricting
        return true;
    }

    public override bool IsAvatarMenuAllowed()
    {
        return true;
    }

    public override void OnFailedAvatarSwitch()
    {
        Notifier.Send(new Notification()
        {
            Title = Campaign.Session.Name,
            Message = $"{Campaign.Session.Name} does not allow avatars with these proportions",
            Type = NotificationType.Error,
            ShowTitleOnPopup = true,
        });
    }

    static bool IsInRange(float value, Vector2 range)
    {
        return range.x <= value && value <= range.y;
    }
}