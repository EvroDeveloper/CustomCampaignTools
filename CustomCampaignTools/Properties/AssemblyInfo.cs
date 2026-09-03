using System.Reflection;
using System.Runtime.CompilerServices;
using CustomCampaignTools;
using MelonLoader;

[assembly: MelonInfo(typeof(CustomCampaignTools.Main), CustomCampaignTools.CampaignConstants.ModName, CustomCampaignTools.CampaignConstants.Version, CustomCampaignTools.CampaignConstants.ModAuthor, CustomCampaignTools.BuildInfo.DownloadLink)]
[assembly: MelonOptionalDependencies("BrowsingPlus")]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

[assembly: InternalsVisibleTo("CustomCampaignTools.BonelabSupport")]
[assembly: InternalsVisibleTo("CustomCampaignTools.BoneworksSupport")]