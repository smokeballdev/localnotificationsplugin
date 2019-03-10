using Android.Content;
using Android.Support.V4.Content;

namespace Plugin.LocalNotifications.Receivers
{
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Broadcast Receiver")]
    public class DismissActionReceiver : WakefulBroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // TODO
        }
    }
}