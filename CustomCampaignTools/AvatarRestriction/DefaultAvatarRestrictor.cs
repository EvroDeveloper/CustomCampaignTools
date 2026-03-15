using CustomCampaignTools.AvatarRestriction;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools
{
    public class DefaultAvatarRestrictor : IAvatarRestrictor
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

        public bool IsAvatarAllowed(Avatar avatar)
        {
            return true;
        }

        public bool IsAvatarAllowed(Barcode avatarBarcode)
        {
            return avatarBarcode == CampaignAvatar;
        }

        public void OnFailedAvatarSwitch(RigManager rm)
        {
            rm.SwapAvatarCrate(CampaignAvatar);
        }

        public bool IsAvatarMenuAllowed()
        {
            return false;
        }
    }
}