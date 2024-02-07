using System.Reflection;
using Labworks;
using MelonLoader;

[assembly: AssemblyTitle(Labworks.BuildInfo.Description)]
[assembly: AssemblyDescription(Labworks.BuildInfo.Description)]
[assembly: AssemblyCompany(Labworks.BuildInfo.Company)]
[assembly: AssemblyProduct(Labworks.BuildInfo.Name)]
[assembly: AssemblyCopyright("Developed by " + Labworks.BuildInfo.Author)]
[assembly: AssemblyTrademark(Labworks.BuildInfo.Company)]
[assembly: AssemblyVersion(Labworks.BuildInfo.Version)]
[assembly: AssemblyFileVersion(Labworks.BuildInfo.Version)]
[assembly: MelonInfo(typeof(Labworks.Main), Labworks.BuildInfo.Name, Labworks.BuildInfo.Version, Labworks.BuildInfo.Author, Labworks.BuildInfo.DownloadLink)]
[assembly: MelonColor(System.ConsoleColor.White)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]