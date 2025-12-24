using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools.AvatarRestriction
{
    public interface IAvatarRestrictor
    {
        bool IsAvatarAllowed(Avatar avatar);

        bool IsAvatarMenuAllowed();

        bool IsAvatarAllowed(Barcode avatarBarcode);

        void OnFailedAvatarSwitch(RigManager rm);
    }
}