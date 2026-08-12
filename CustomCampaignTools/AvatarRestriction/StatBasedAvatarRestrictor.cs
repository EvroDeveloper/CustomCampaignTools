using UnityEngine;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;
using BoneLib.Notifications;
using BoneLib;
using CustomCampaignTools.Debug;

namespace CustomCampaignTools.AvatarRestriction;

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
        CampaignLogger.Msg("Testing avatar: " + avatar.gameObject.name);
        CampaignLogger.Msg($"Avatar Height: {avatar.height}, Valid Range: {_avatarHeightRange}");
        CampaignLogger.Msg($"Avatar Height: {avatar.massTotal}, Valid Range: {_avatarMassRange}");
        CampaignLogger.Msg($"Avatar Height: {avatar.armLength}, Valid Range: {_avatarArmLengthRange}");
        return IsInRange(avatar.height, _avatarHeightRange) && IsInRange(avatar.massTotal, _avatarMassRange) && IsInRange(avatar.armLength, _avatarArmLengthRange);
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

    public override void ForceRigManagerAvatar(RigManager rm)
    {
        rm.SwapAvatarCrate(new Barcode(CommonBarcodes.Avatars.PolyBlank));
    }
}