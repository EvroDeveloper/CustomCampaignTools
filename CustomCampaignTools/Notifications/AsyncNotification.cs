using System.Collections;
using MelonLoader;

namespace CustomCampaignTools.Notifications;

public class AsyncNotification : NotificationBase
{
    public override void PlayNotification()
    {
        MelonCoroutines.Start(CoPlayNotification());
    }

    public virtual IEnumerator ExecuteBeforeNotification() {yield return null;}
    public virtual IEnumerator ExecuteAfterNotification() {yield return null;}

    public virtual IEnumerator CoPlayNotification()
    {
        yield return ExecuteBeforeNotification();
        NotificationManager.Send(this);
        yield return ExecuteAfterNotification();
    }
}