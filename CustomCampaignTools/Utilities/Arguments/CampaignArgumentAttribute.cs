namespace CustomCampaignTools.Utilities;

[System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class CampaignArgumentAttribute : System.Attribute
{
    readonly string argumentIdentifier;
    readonly int extraArgs;
    
    public CampaignArgumentAttribute(string argumentIdentifier)
    {
        this.argumentIdentifier = argumentIdentifier;
        extraArgs = 0;
    }

    public CampaignArgumentAttribute(string argumentIdentifier, int extraArgs)
    {
        this.argumentIdentifier = argumentIdentifier;
        this.extraArgs = extraArgs;
    }
    
    public string ArgumentIdentifier
    {
        get { return argumentIdentifier; }
    }

    public int ExtraArgs
    {
        get { return extraArgs; }
    }
}