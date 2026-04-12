using System;
using CustomCampaignTools;
using UnityEngine;

namespace VoidTakeoverSupport;

public static class FunValueManager
{
    private const string funValueKey = "fun";

    public static int GetFunValue(Campaign c)
    {
        if(c == null) return 0;
        if((int)c.saveData.GetValue(funValueKey) == 0)
            RefreshFunValue(c);
        
        return (int)c.saveData.GetValue(funValueKey);
    }

    public static void RefreshFunValue(Campaign c, int min = 1, int max = 100)
    {
        c.saveData.SetValue(funValueKey, UnityEngine.Random.Range(min, max + 1));
    }
}
