using Android.App;
using Android.Content;
using Plugin.LocalNotifications.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using AndroidX.Core.App;

namespace Plugin.LocalNotifications
{
    /// <summary>
    ///     Local Notifications implementation for Android
    /// </summary>
    public class LocalNotifications : ILocalNotifications
    {
        public const string NotificationAction = "plugin.localnotifications.NOTIFICATION_EVENT";

        private static readonly List<ActionRegistrar> _actionRegistrars = new List<ActionRegistrar>();

        /// <inheritdoc />
        /// <summary>
        ///     Register actions for notifications
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ILocalNotificationActionRegistrar RegisterActionSet(string id) => NewActionRegistrar(id);

        /// <inheritdoc />
        /// <summary>
        ///     Build and schedule a local notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ILocalNotificationBuilder New(int id) => new NotificationBuilder(id);

        /// <inheritdoc />
        /// <summary>
        ///     Cancel a local notification
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

        public void CancelAll()
        {
            // Cancels all shown notifications
            // DOES NOT cancel all pending notifications, does not seem possible without recreating the pending intent (see Cancel method)
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.CancelAll();
        }

        public static void Initialize(Type notificationActivityType, int notificationIconId)
        {
            NotificationActivityType = notificationActivityType;
            NotificationIconId = notificationIconId;
        }

        public static void ProcessIntent(Intent intent)
        {
            var notificationId = intent.GetIntExtra(LocalNotification.NotificationId, -1);

            // Don't process this intent if it doesn't have a notification id attached
            if (notificationId == -1)
            {
                return;
            }

            var actionSetId = intent.GetStringExtra(LocalNotification.ActionSetId);
            var id = intent.GetStringExtra(LocalNotification.ActionId);
            var action = GetRegisteredActions(actionSetId)?.FirstOrDefault(a => a.Id == id);

            action?.Action(new LocalNotificationArgs
            {
                Parameter = intent.GetStringExtra(LocalNotification.ActionParameter),
                TimestampUtc = DateTime.UtcNow,
            });
        }

        internal static IEnumerable<LocalNotificationActionRegistration> GetRegisteredActions(string actionSetId) => _actionRegistrars?.FirstOrDefault(a => a.ActionSetId == actionSetId)?.RegisteredActions;

        private static ILocalNotificationActionRegistrar NewActionRegistrar(string id)
        {
            if (_actionRegistrars.Any(a => a.ActionSetId == id))
            {
                throw new InvalidOperationException($"Could not register action set '{id}' because an action set with the same id already exists.");
            }

            var registrar = new ActionRegistrar(id);
            _actionRegistrars.Add(registrar);
            return registrar;
        }

        internal static Type NotificationActivityType { get; set; }
        internal static int NotificationIconId { get; set; }
    }
}