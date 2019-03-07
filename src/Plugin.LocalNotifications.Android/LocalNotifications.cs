using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Plugin.LocalNotifications.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for Android
    /// </summary>
    public class LocalNotifications : ILocalNotifications
    {
        private static readonly List<ActionRegistrar> _actionRegistrars = new List<ActionRegistrar>();

  
        /// <summary>
        /// Register actions for notifications
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ILocalNotificationActionRegistrar RegisterActionSet(string id) => NewActionRegistrar(id);

        /// <summary>
        /// Build and schedule a local notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ILocalNotificationBuilder New(int id) => new NotificationBuilder(id);

        /// <inheritdoc />
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

        public static void Initialize(Type notificationActivityType, int notificationIconId)
        {
            NotificationActivityType = notificationActivityType;
            NotificationIconId = notificationIconId;
        }

        public static void ProcessIntent(Intent intent)
        {
            var actionSetId = intent.GetStringExtra(LocalNotification.ActionSetId);
            var actionId = intent.GetStringExtra(LocalNotification.ActionId);
            var action = GetRegisteredActions(actionSetId)?.FirstOrDefault(a => a.UniqueIdentifier == actionId);

            //CrossLocalNotifications.Current
            //    .New(101)
            //    .WithTitle("inside local notifications")
            //    .WithBody($"A null?: {action == null}" +
            //              $" ASId: {actionSetId}" +
            //              $" AId: {actionId}")
            //    .Show(DateTime.Now.AddSeconds(4));

            action?.Action(intent.GetStringExtra(LocalNotification.ActionParameter));
        }

        internal static IEnumerable<LocalNotificationActionRegistration> GetRegisteredActions(string actionSetId) => _actionRegistrars?.FirstOrDefault(a => a.Id == actionSetId)?.RegisteredActions;

        private static ILocalNotificationActionRegistrar NewActionRegistrar(string id)
        {
            if (_actionRegistrars.Any(a => a.Id == id))
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