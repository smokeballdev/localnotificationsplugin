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

        public ILocalNotificationBuilder WithActionSet(string actionSetId, string parameter)
        {
            var registeredActions = _actionReceiver.GetRegisteredActions(actionSetId).ToArray();

            if (!registeredActions.Any())
            {
                throw new InvalidOperationException($"Unable to associate action set id {actionSetId} with notification because it has not been registered.");
            }

            _actions.AddRange(registeredActions.OfType<ButtonLocalNotificationActionRegistration>().Select(r => r.ToAction(parameter)));
            _actions.Add(registeredActions.FirstOrDefault(a => a.Id == LocalNotificationActionRegistration.DismissActionIdentifier).ToAction(parameter));

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
            builder.SetPriority((int)NotificationPriority.Max);

            // User actions
            var actions = notification.Actions.Where(a => a.Id != LocalNotificationActionRegistration.DismissActionIdentifier).ToArray();
            if (actions.Any())
            {
                builder.SetActions(GetNotificationActions(actions).ToArray());
            }

            // Default dismissal action
            var dismissAction = notification.Actions.FirstOrDefault(a => a.Id == LocalNotificationActionRegistration.DismissActionIdentifier);
            if (dismissAction != null)
            {
                var dismissActionIntent = CreateActionReceiverIntent(dismissAction, LocalNotificationActionReceiver.LocalNotificationIntentDismiss);
                var dismissActionPendingIntent = PendingIntent.GetBroadcast(Application.Context, GetRandomId(), dismissActionIntent, PendingIntentFlags.CancelCurrent);

                builder.SetDeleteIntent(dismissActionPendingIntent);
            }

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
                var actionIntent = CreateActionReceiverIntent(action, LocalNotificationActionReceiver.LocalNotificationIntentAction);
                var actionPendingIntent = PendingIntent.GetBroadcast(Application.Context, GetRandomId(), actionIntent, PendingIntentFlags.CancelCurrent);

                var iconId = action.IconId == 0 ? Resource.Drawable.plugin_lc_smallicon : action.IconId;

                yield return new Notification.Action(iconId, action.Title, actionPendingIntent);
            }
        }

        private static Intent CreateActionReceiverIntent(LocalNotificationAction action, string actionType)
        {
            var actionIntent = new Intent();
            actionIntent.SetAction(actionType);
            actionIntent.PutExtra(LocalNotificationActionReceiver.LocalNotificationActionSetId, action.ActionSetId);
            actionIntent.PutExtra(LocalNotificationActionReceiver.LocalNotificationActionId, action.Id);
            actionIntent.PutExtra(LocalNotificationActionReceiver.LocalNotificationActionParameter, action.Parameter);
            return actionIntent;
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