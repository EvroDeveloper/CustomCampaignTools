using System;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace CustomCampaignTools.Utilities;

public static class ButtonExtensions
{
    public static void ClearClickCallbacks(this ButtonClickedEvent onClick)
    {
        onClick.m_PersistentCalls.Clear();
        onClick.m_Calls.ClearPersistent();
        onClick.m_Calls.Clear();
    }

    public static void ClearAndSetCallback(this ButtonClickedEvent onClick, Action action)
    {
        onClick.ClearClickCallbacks();
        onClick.AddListener(action);
    }
}