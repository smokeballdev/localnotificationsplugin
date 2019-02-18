using Android.Content;
using Plugin.LocalNotifications.Extensions;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Broadcast receiver
    /// </summary>
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Broadcast Receiver")]
    public class ScheduledAlarmHandler : BroadcastReceiver
    {
        public const string LocalNotificationKey = "LocalNotification";

        public override void OnReceive(Context context, Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = extra.Deserialize<LocalNotification>();

            LocalNotificationBuilder.Notify(notification);
        }
    }
}