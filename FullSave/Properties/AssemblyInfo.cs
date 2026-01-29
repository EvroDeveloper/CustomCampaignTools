using System.Reflection;
using FullSave;
using MelonLoader;

//[assembly: AssemblyTitle(CustomCampaignTools.BuildInfo.Description)]
//[assembly: AssemblyDescription(CustomCampaignTools.BuildInfo.Description)]
//[assembly: AssemblyCompany(CustomCampaignTools.BuildInfo.Company)]
//[assembly: AssemblyProduct(CustomCampaignTools.BuildInfo.Name)]
//[assembly: AssemblyCopyright("Developed by " + CustomCampaignTools.BuildInfo.Author)]
//[assembly: AssemblyTrademark(CustomCampaignTools.BuildInfo.Company)]
//[assembly: AssemblyVersion(CustomCampaignTools.BuildInfo.Version)]
//[assembly: AssemblyFileVersion(CustomCampaignTools.BuildInfo.Version)]
[assembly: MelonInfo(typeof(FullSave.Main), FullSave.BuildInfo.Name, FullSave.BuildInfo.Version, FullSave.BuildInfo.Author, FullSave.BuildInfo.DownloadLink)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]