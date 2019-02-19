using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Plugin.LocalNotifications.Abstractions;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    public class LocalNotificationActionRegistrar : ILocalNotificationActionRegistrar
    {
        public LocalNotificationActionRegistrar(string categoryId)
        {
            Id = categoryId;
        }

        public ILocalNotificationActionRegistrar WithActionHandler(string title, int iconId, Action<string> action)
        {
            if (RegisteredActions.Any(a => a.Title == title))
            {
                throw new InvalidOperationException($"Could not register action {title} into action set {Id} because one has with the same name ahs already been registered");
            }

            RegisteredActions.Add(new LocalNotificationActionRegistration
            {
                ActionSetId = Id,
                Title = title,
                Action = action,
            });

            return this;
        }

        public void Register()
        {
            var category = UNNotificationCategory.FromIdentifier(
                Id,
                RegisteredActions.Select(action => UNNotificationAction.FromIdentifier(action.Title, action.Title, UNNotificationActionOptions.Foreground)).ToArray(),
                new string[] { },
                UNNotificationCategoryOptions.CustomDismissAction);

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(category));
        }

        public string Id { get; }

        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}