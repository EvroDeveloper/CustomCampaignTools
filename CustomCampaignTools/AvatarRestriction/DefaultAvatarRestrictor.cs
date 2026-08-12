using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools.AvatarRestriction;

public class DefaultAvatarRestrictor : AvatarRestrictor
{
    public DefaultAvatarRestrictor(AvatarCrateReference defaultCampaignAvatar, AvatarCrateReference fallbackAvatar)
    {
        this.defaultCampaignAvatar = defaultCampaignAvatar;
        this.fallbackAvatar = fallbackAvatar;
    }
    

    public AvatarCrateReference defaultCampaignAvatar;
    public AvatarCrateReference fallbackAvatar;

    public Barcode _cachedAvatar;
    public Barcode CampaignAvatar
    {
        get
        {
            if(!_cachedAvatar.IsValid() || _cachedAvatar == null)
            {
                if (defaultCampaignAvatar.TryGetCrate(out _))
                    _cachedAvatar = defaultCampaignAvatar.Barcode;
                else
                    _cachedAvatar = fallbackAvatar.Barcode;
            }
            return _cachedAvatar;
        }
    }

    public override bool IsAvatarAllowed(Barcode avatarBarcode)
    {
        return avatarBarcode == CampaignAvatar;
    }

    public override void ForceRigManagerAvatar(RigManager rm)
    {
        rm.SwapAvatarCrate(CampaignAvatar);
    }

    public override bool IsAvatarMenuAllowed()
    {
        return false;
    }
}