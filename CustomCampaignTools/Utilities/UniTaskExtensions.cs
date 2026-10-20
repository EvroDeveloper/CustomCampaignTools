using System;
using Il2CppCysharp.Threading.Tasks;
using UnityEngine;

namespace CustomCampaignTools.Utilities;

public static class UniTaskExtensions
{
    public static void OnComplete<T>(this UniTask<T> task, Action<T> callback)
    {
        var awaiter = task.GetAwaiter();
        var continuation = () =>
        {
            var result = awaiter.GetResult();
            callback?.Invoke(result);
        };

        if (!awaiter.IsCompleted)
        {
            awaiter.UnsafeOnCompleted(continuation);
        }
        else
        {
            continuation();
        }
    }

    public static void OnComplete(this UniTask task, Action callback)
    {
        var awaiter = task.GetAwaiter();
        var continuation = () =>
        {
            awaiter.GetResult();
            callback?.Invoke();
        };

        if (!awaiter.IsCompleted)
        {
            awaiter.UnsafeOnCompleted(continuation);
        }
        else
        {
            continuation();
        }
    }
}