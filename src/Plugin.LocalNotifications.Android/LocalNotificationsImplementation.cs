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
    public class LocalNotificationsImplementation : ILocalNotifications
    {
        private readonly IntentFilter _actionIntentFilter;
        private readonly LocalNotificationActionReceiver _actionReceiver;

        public LocalNotificationsImplementation()
        {
            _actionIntentFilter = new IntentFilter();
            _actionReceiver = new LocalNotificationActionReceiver();

            Application.Context.RegisterReceiver(_actionReceiver, _actionIntentFilter);
        }

        public void RegisterAction(int iconId, string categoryId, string actionId, string displayName, Action<string> action)
        {
            _actionReceiver.Register(new LocalNotificationActionRegistration
            {
                Id = actionId,
                IconId = iconId,
                Title = displayName,
                Action = action,
            });
            _actionIntentFilter.AddAction(actionId);
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