﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Plugin.LocalNotifications.Abstractions;
using Plugin.LocalNotifications.Extensions;
using Plugin.LocalNotifications.Models;

namespace Plugin.LocalNotifications
{
    internal class NotificationBuilder : ILocalNotificationBuilder
    {
        private static NotificationManager _manager => (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

        private readonly int _id;
        private readonly List<LocalNotificationAction> _actions = new List<LocalNotificationAction>();
        private int _iconId;
        private string _title;
        private string _body;

        public NotificationBuilder(int id)
        {
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
            var registeredActions = LocalNotifications.GetRegisteredActions(actionSetId).ToArray();

            if (!registeredActions.Any())
            {
                throw new InvalidOperationException($"Unable to associate notificationAction set id {actionSetId} with notification because it has not been registered.");
            }

            _actions.AddRange(registeredActions.Select(r => r.ToAction(parameter)));

            return this;
        }

        public void Show(DateTime notifyTime)
        {
            Console.WriteLine($"Will show notification (id: {_id}) with title '{_title}' at {notifyTime}");

            var localNotification = BuildLocalNotification();
            localNotification.NotifyTime = notifyTime;
            localNotification.IconId = GetIconId(_iconId);
            
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
            Console.WriteLine($"Will show notification (id: {_id}) with title '{_title}' immediately");

            Notify(BuildLocalNotification());
        }

        public static void Notify(LocalNotification notification)
        {
            var builder = new Notification.Builder(Application.Context);
            builder.SetContentTitle(notification.Title);
            builder.SetContentText(notification.Body);
            builder.SetAutoCancel(true);
            builder.SetPriority((int)NotificationPriority.Max);
            builder.SetSmallIcon(notification.IconId);

            // Tapping on the notification
            var defaultAction = notification.Actions.FirstOrDefault(a => a.ActionId == ActionIdentifiers.Default);
            builder.SetContentIntent(CreateActivityPendingIntent(notification.Id, defaultAction));

            // Dismissing a notification (swiping the notification)
            var dismissAction = notification.Actions.FirstOrDefault(a => a.ActionId == ActionIdentifiers.Dismiss);
            if (dismissAction != null)
            {
                builder.SetDeleteIntent(CreatePendingIntent(notification.Id, dismissAction));
            }

            // User actions
            var actions = notification.Actions.Where(a => a.ActionId == ActionIdentifiers.Action).ToArray();
            if (actions.Any())
            {
                builder.SetActions(GetNotificationActions(notification, actions).ToArray());
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelId = $"{Application.Context.PackageName}.general";
                var channel = new NotificationChannel(channelId, "General", NotificationImportance.Max);

                _manager.CreateNotificationChannel(channel);

                builder.SetChannelId(channelId);
            }

            _manager.Notify(notification.Id, builder.Build());
        }

        private static IEnumerable<Notification.Action> GetNotificationActions(LocalNotification notification, IEnumerable<LocalNotificationAction> actions)
        {
            foreach (var action in actions)
            {
                var pendingIntent = CreatePendingIntent(notification.Id, action);
                var iconId = action.IconId == 0 ? Resource.Drawable.plugin_lc_smallicon : action.IconId;

                yield return new Notification.Action(iconId, action.Title, pendingIntent);
            }
        }

        private static PendingIntent CreateActivityPendingIntent(int notificationId, LocalNotificationAction action)
        {
            var intent = CreateIntent(notificationId, action, launchActivity: true);
            return PendingIntent.GetActivity(Application.Context, GetRandomId(), intent, PendingIntentFlags.UpdateCurrent);
        }

        private static PendingIntent CreatePendingIntent(int notificationId, LocalNotificationAction action)
        {
            var intent = CreateIntent(notificationId, action);
            return PendingIntent.GetBroadcast(Application.Context, GetRandomId(), intent, PendingIntentFlags.UpdateCurrent);
        }

        private static Intent CreateIntent(int notificationId, LocalNotificationAction notificationAction, bool launchActivity = false)
        {
            Intent intent = null;

            if (launchActivity)
            {
                // Set activity as receiver
                intent = typeof(Activity).IsAssignableFrom(LocalNotifications.NotificationActivityType)
                    ? new Intent(Application.Context, LocalNotifications.NotificationActivityType)
                    : Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
            }
            else
            {
                intent = new Intent(LocalNotifications.NotificationAction);

                // Find the broadcast receiver associated with the intent
                var packageManager = Application.Context.PackageManager;
                var match = packageManager.QueryBroadcastReceivers(intent, 0).FirstOrDefault();

                if (match != null)
                {
                    // Explicitly set the component to receive this intent
                    intent.SetComponent(new ComponentName(match.ActivityInfo.ApplicationInfo.PackageName, match.ActivityInfo.Name));
                }
            }

            if (notificationAction != null)
            {
                intent.PutExtra(LocalNotification.NotificationId, notificationId);
                intent.PutExtra(LocalNotification.ActionSetId, notificationAction.ActionSetId);
                intent.PutExtra(LocalNotification.ActionId, notificationAction.Id);
                intent.PutExtra(LocalNotification.ActionParameter, notificationAction.Parameter);
            }

            return intent;
        }

        private LocalNotification BuildLocalNotification() =>
            new LocalNotification
            {
                Id = _id,
                IconId = GetIconId(_iconId),
                Title = _title,
                Body = _body,
                Actions = _actions
            };

        private static int GetIconId(int iconId)
        {
            if (iconId != 0)
            {
                return iconId;
            }

            if (LocalNotifications.NotificationIconId != 0)
            {
                return LocalNotifications.NotificationIconId;
            }

            return Resource.Drawable.plugin_lc_smallicon;
        }

        private static int GetRandomId() => Guid.NewGuid().GetHashCode();
    }
}