using System;
using System.Collections.Generic;
using System.Reflection;
using BoneLib;
using CustomCampaignTools.Debug;
using HarmonyLib;

namespace CustomCampaignTools.Utilities.Patching;

public static class CampaignPatcher
{
    private class CampaignPatchWrapper
    {
        public HarmonyLib.Harmony patchHarmony;
        public MethodInfo sourceMethod;
        public HarmonyMethod prefix;
        public HarmonyMethod postfix;

        public CampaignPatchWrapper(HarmonyLib.Harmony patchHarmony, MethodInfo sourceMethod, MethodInfo prefix, MethodInfo postfix)
        {
            this.patchHarmony = patchHarmony;
            this.sourceMethod = sourceMethod;
            this.prefix = new HarmonyMethod(prefix);
            this.postfix = new HarmonyMethod(postfix);
        }

        public CampaignPatchWrapper(HarmonyLib.Harmony patchHarmony, MethodInfo sourceMethod, MethodInfo patchMethod, bool isPrefix)
        {
            this.patchHarmony = patchHarmony;
            this.sourceMethod = sourceMethod;
            if(isPrefix)
                this.prefix = new HarmonyMethod(patchMethod);
            else
                this.postfix = new HarmonyMethod(patchMethod);
        }
        
        public void Patch()
        {
            patchHarmony.Patch(sourceMethod, prefix, postfix);
        }

        public void Unpatch()
        {
            patchHarmony.UnpatchSelf();
        }
    }

    private static List<CampaignPatchWrapper> CampaignOnlyPatches = [];
    private static List<CampaignPatchWrapper> NonCampaignPatches = [];

    public static void PatchAssembly(Assembly assembly)
    {
        CampaignLogger.Msg("CampaignPatching Assembly " + assembly.GetName());
        var patches = AssemblyUtils.FindMethodsWithAttribute<CampaignPatchAttribute>(assembly, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach(var patch in patches)
        {
            var attribute = patch.Item2;
            
            MethodInfo sourceMethod = attribute.Method;

            MethodInfo hookMethod = patch.Item1;
            Type hookType = hookMethod.DeclaringType;
            
            bool isPrefix = hookMethod.GetCustomAttribute<HarmonyPrefix>() != null;
            
            HarmonyLib.Harmony harmony = new($"customcampaigntools.patch.{hookType.Name}.{hookMethod.Name}");

            CampaignPatchWrapper wrapper = new(harmony, sourceMethod, hookMethod, isPrefix);

            CampaignLogger.Msg("Found Campaign Patch: " + harmony.Id);

            if(attribute.RunFlags == CampaignPatchRunFlags.Always)
                wrapper.Patch();
            else if(attribute.RunFlags == CampaignPatchRunFlags.SessionActive)
                CampaignOnlyPatches.Add(wrapper);
            else if(attribute.RunFlags == CampaignPatchRunFlags.SessionInactive)
                NonCampaignPatches.Add(wrapper);
        }
    }

    public static void OnExitedCampaign()
    {
        CampaignLogger.MsgVerbose("Exiting Campaign. Unpatching campaign patches, repatching non-campaign patches");
        foreach(CampaignPatchWrapper campaignPatch in CampaignOnlyPatches)
        {
            campaignPatch.Unpatch();
        }
        foreach(CampaignPatchWrapper campaignPatch in NonCampaignPatches)
        {
            campaignPatch.Patch();
        }
    }

    public static void OnEnterCampaign()
    {
        CampaignLogger.MsgVerbose("Entering Campaign. Repatching campaign patches, Unpatching non-campaign patches");
        foreach(CampaignPatchWrapper campaignPatch in CampaignOnlyPatches)
        {
            campaignPatch.Patch();
        }
        foreach(CampaignPatchWrapper campaignPatch in NonCampaignPatches)
        {
            campaignPatch.Unpatch();
        }
    }
}