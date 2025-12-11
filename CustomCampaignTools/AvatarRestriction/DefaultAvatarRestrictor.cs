using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools
{
    public class DefaultAvatarRestrictor : IAvatarRestrictor
    {
        public DefaultAvatarRestrictor(Barcode defaultCampaignAvatar, Barcode fallbackAvatar)
        {
            this.defaultCampaignAvatar = defaultCampaignAvatar;
            this.fallbackAvatar = fallbackAvatar;
        }
        

        public Barcode defaultCampaignAvatar;
        public Barcode fallbackAvatar;

        public Barcode _cachedAvatar;
        public Barcode CampaignAvatar
        {
            get
            {
                if(!_cachedAvatar.IsValid() || _cachedAvatar == null)
                {
                    if (MarrowGame.assetWarehouse.HasCrate(defaultCampaignAvatar))
                        _cachedAvatar = defaultCampaignAvatar;
                    else
                        _cachedAvatar = fallbackAvatar;
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
    }
}