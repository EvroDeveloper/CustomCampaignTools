using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;

namespace CustomCampaignTools.AvatarRestriction;

public class AvatarRestrictor
{
    public virtual bool IsAvatarAllowed(Avatar avatar) { return true; }
    public virtual bool IsAvatarAllowed(Barcode avatarBarcode) { return true; }
    public virtual bool IsAvatarMenuAllowed() { return true; }
    public virtual void OnFailedAvatarSwitch() { }
    public virtual void ForceRigManagerAvatar(RigManager rm) { }
}