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
            CampaignLogger.Msg("Enabling Patch: " + patchHarmony.Id);
        }

        public void Unpatch()
        {
            patchHarmony.UnpatchSelf();
            CampaignLogger.Msg("Disabling Patch: " + patchHarmony.Id);
        }
    }

    private static List<CampaignPatchWrapper> CampaignOnlyPatches = [];
    private static List<CampaignPatchWrapper> NonCampaignPatches = [];

    public static void PatchAssembly(Assembly assembly)
    {
        CampaignLogger.Msg("CampaignPatching Assembly " + assembly.GetName());
        var patches = AssemblyUtils.FindMethodsWithAttribute<CampaignPatchAttribute>(assembly, BindingFlags.Static);
        foreach(var patch in patches)
        {
            var attribute = patch.Item2;
            
            MethodInfo sourceMethod = attribute.Method;

            MethodInfo hookMethod = patch.Item1;
            Type hookType = hookMethod.DeclaringType;
            
            bool isPrefix = hookMethod.GetCustomAttribute<HarmonyPrefix>() != null;
            
            HarmonyLib.Harmony harmony = new($"customcampaigntools.patch.{hookType.Name}.{hookMethod.Name}");

            CampaignPatchWrapper wrapper = new(harmony, sourceMethod, hookMethod, isPrefix);

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
        CampaignLogger.Msg("Exiting Campaign. Unpatching campaign patches, repatching non-campaign patches");
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
        CampaignLogger.Msg("Entering Campaign. Repatching campaign patches, Unpatching non-campaign patches");
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