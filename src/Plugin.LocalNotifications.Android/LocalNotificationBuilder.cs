using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Plugin.LocalNotifications.Abstractions;
using Plugin.LocalNotifications.Extensions;

namespace Plugin.LocalNotifications
{
    internal class LocalNotificationBuilder : ILocalNotificationBuilder
    {
        private static NotificationManager _manager => (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

        private readonly int _id;
        private readonly List<LocalNotificationAction> _actions = new List<LocalNotificationAction>();
        private readonly LocalNotificationActionReceiver _actionReceiver;
        private int _iconId;
        private string _title;
        private string _body;

        public LocalNotificationBuilder(LocalNotificationActionReceiver actionReceiver, int id)
        {
            _actionReceiver = actionReceiver;
            _id = id;
        }

        public ILocalNotificationBuilder WithIcon(int iconId)
        {
            _iconId = iconId;
            return this;
        }

        public ILocalNotificationBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public ILocalNotificationBuilder WithBody(string body)
        {
            _body = body;
            return this;
        }

        public ILocalNotificationBuilder WithAction(string actionId, string parameter)
        {
            var registeredAction = _actionReceiver.GetRegisteredAction(actionId);

            if (registeredAction == null)
            {
                throw new InvalidOperationException($"Unable to associate action {actionId} with notification because it has not been registered.");
            }

            _actions.Add(new LocalNotificationAction
            {
                Id = actionId,
                DisplayName = registeredAction.DisplayName,
                Parameter = parameter
            });
            return this;
        }

        public void Show(DateTime notifyTime)
        {
            var localNotification = GetLocalNotification();
            localNotification.NotifyTime = notifyTime;

            if (_iconId != 0)
            {
                localNotification.IconId = _iconId;
            }
            else
            {
                localNotification.IconId = Resource.Drawable.plugin_lc_smallicon;
            }

            var intent = new Intent(Application.Context, typeof(ScheduledAlarmHandler))
                .SetAction("LocalNotifierIntent" + _id);

            intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, localNotification.Serialize());

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, GetRandomId(), intent, PendingIntentFlags.CancelCurrent);
            var triggerTime = localNotification.NotifyTime.AsEpochMilliseconds();

            if (!(Application.Context.GetSystemService(Context.AlarmService) is AlarmManager alarmManager))
            {
                throw new NullReferenceException(nameof(alarmManager));
            }

            alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        public void Show()
        {
            Notify(GetLocalNotification());
        }
        
        public static void Notify(LocalNotification notification)
        {
            var builder = new Notification.Builder(Application.Context);
            builder.SetContentTitle(notification.Title);
            builder.SetContentText(notification.Body);
            builder.SetAutoCancel(true);
            builder.SetActions(GetNotificationActions(notification.Actions).ToArray());
            builder.SetPriority((int)NotificationPriority.Max);

            if (notification.IconId != 0)
            {
                builder.SetSmallIcon(notification.IconId);
            }
            else
            {
                builder.SetSmallIcon(Resource.Drawable.plugin_lc_smallicon);
            }

            var packageName = Application.Context.PackageName;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelId = $"{packageName}.general";
                var channel = new NotificationChannel(channelId, "General", NotificationImportance.Max);

                _manager.CreateNotificationChannel(channel);

                builder.SetChannelId(channelId);
            }

            var resultIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);

            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(resultPendingIntent);

            _manager.Notify(notification.Id, builder.Build());
        }

        private static IEnumerable<Notification.Action> GetNotificationActions(IEnumerable<LocalNotificationAction> actions)
        {
            foreach (var action in actions)
            {
                var actionIntent = new Intent();
                actionIntent.SetAction(LocalNotificationActionReceiver.LocalNotificationIntentAction);
                actionIntent.PutExtra(LocalNotificationActionReceiver.LocalNotificationActionId, action.Id);
                actionIntent.PutExtra(LocalNotificationActionReceiver.LocalNotificationActionParameter, action.Parameter);

                var actionPendingIntent = PendingIntent.GetBroadcast(Application.Context, GetRandomId(), actionIntent, PendingIntentFlags.CancelCurrent);

                var iconId = action.IconId == 0 ? Resource.Drawable.plugin_lc_smallicon : action.IconId;

                yield return new Notification.Action(iconId, action.DisplayName, actionPendingIntent);
            }
        }

        private LocalNotification GetLocalNotification() =>
            new LocalNotification
            {
                Id = _id,
                IconId = _iconId,
                Title = _title,
                Body = _body,
                Actions = _actions,
            };

        private static int GetRandomId() => Guid.NewGuid().GetHashCode();
    }
}