using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Plugin.LocalNotifications.Abstractions;
using System;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for Android
    /// </summary>
    public class LocalNotifications : ILocalNotifications
    {
        private readonly LocalNotificationActionReceiver _actionReceiver;

        public LocalNotifications()
        {
            _actionReceiver = new LocalNotificationActionReceiver();

            Application.Context.RegisterReceiver(_actionReceiver, new IntentFilter(LocalNotificationActionReceiver.LocalNotificationIntentAction));
        }

        public void RegisterActionHandler(string categoryId, string actionId, string displayName, int iconId, Action<string> action)
        {
            _actionReceiver.Register(new LocalNotificationActionRegistration
            {
                Id = actionId,
                IconId = iconId,
                DisplayName = displayName,
                Action = action,
            });
        }

        public ILocalNotificationBuilder New(int id) => new LocalNotificationBuilder(_actionReceiver, id);

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            var intent = new Intent(Application.Context, typeof(ScheduledAlarmHandler))
                .SetAction("LocalNotifierIntent" + id);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, Guid.NewGuid().GetHashCode(), intent, PendingIntentFlags.CancelCurrent);

            if (!(Application.Context.GetSystemService(Context.AlarmService) is AlarmManager alarmManager))
            {
                throw new NullReferenceException(nameof(alarmManager));
            }

            alarmManager.Cancel(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Cancel(id);
        }
    }
}