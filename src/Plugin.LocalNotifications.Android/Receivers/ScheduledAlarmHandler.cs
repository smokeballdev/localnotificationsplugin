using Android.Content;
using Android.Support.V4.Content;
using Plugin.LocalNotifications.Extensions;

namespace Plugin.LocalNotifications
{
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Broadcast Receiver")]
    public class ScheduledAlarmHandler : WakefulBroadcastReceiver
    {
        public const string LocalNotificationKey = "LocalNotification";

        public override void OnReceive(Context context, Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = extra.Deserialize<LocalNotification>();

            NotificationBuilder.Notify(notification);
        }
    }
}