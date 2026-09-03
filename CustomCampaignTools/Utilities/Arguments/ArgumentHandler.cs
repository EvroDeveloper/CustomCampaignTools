using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;

namespace CustomCampaignTools.Utilities;

public static class ArgumentHandler
{
    public static void HandleArguments(string[] args)
    {
        var values = AssemblyUtils.FindMethodsWithAttribute<CampaignArgumentAttribute>(Main.ModAssembly, BindingFlags.Static).ToList();
        values.AddRange(AssemblyUtils.FindMethodsWithAttribute<CampaignArgumentAttribute>(GameManager.currentGameConfiguration.SupportAssembly, BindingFlags.Static));

        Dictionary<string, (MethodInfo, CampaignArgumentAttribute)> argToMethod = [];
        foreach(var pair in values)
        {
            argToMethod.Add(pair.Item2.ArgumentIdentifier, pair);
        }

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i].ToLower();
            if (argToMethod.ContainsKey(arg))
            {
                var pair = argToMethod[arg];
                var attribute = pair.Item2;
                var method = pair.Item1;

                if(attribute.ExtraArgs != 0)
                {
                    string[] extraArgs = new string[attribute.ExtraArgs];
                    for(int a = 0; a < attribute.ExtraArgs; a++)
                    {
                        extraArgs[a] = args[i+a+1];
                    }
                    i += attribute.ExtraArgs;
                    method.Invoke(null, extraArgs);
                }
                else
                {
                    method.Invoke(null, null);
                }
            }
            else continue;
        }
    }
}
