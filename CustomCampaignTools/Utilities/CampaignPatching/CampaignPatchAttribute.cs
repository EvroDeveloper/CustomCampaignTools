using System;
using System.Reflection;

namespace CustomCampaignTools.Utilities.Patching;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class CampaignPatchAttribute : Attribute
{
    readonly Type targetType;
    readonly string methodName;
    readonly CampaignPatchRunFlags runFlags;
    
    public CampaignPatchAttribute(Type targetType, string methodName)
    {
        this.targetType = targetType;
        this.methodName = methodName;
        runFlags = CampaignPatchRunFlags.Always;
    }

    public CampaignPatchAttribute(Type targetType, string methodName, CampaignPatchRunFlags runFlags)
    {
        this.targetType = targetType;
        this.methodName = methodName;
        this.runFlags = runFlags;
    }
    
    public Type TargetType
    {
        get { return targetType; }
    }

    public string MethodName
    {
        get { return methodName; }
    }

    public MethodInfo Method
    {
        get { return targetType.GetMethod(methodName); }
    }

    public CampaignPatchRunFlags RunFlags
    {
        get { return runFlags; }
    }
}