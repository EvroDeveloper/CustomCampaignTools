using System;

namespace CustomCampaignTools.Utilities.Patching;

[Flags]
enum CampaignPatchRunFlags
{
    Never = 0,
    SessionActive = 1,
    SessionInactive = 2,
    Always = SessionActive | SessionInactive,
}