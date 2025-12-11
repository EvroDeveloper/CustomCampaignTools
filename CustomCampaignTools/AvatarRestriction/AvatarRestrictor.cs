using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools
{
    public interface IAvatarRestrictor
    {
        bool IsAvatarAllowed(Avatar avatar);

        bool IsAvatarAllowed(Barcode avatarBarcode);

        void OnFailedAvatarSwitch(RigManager rm);
    }
}