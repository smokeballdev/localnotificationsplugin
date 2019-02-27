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

        public ILocalNotificationActionRegistrar WithActionHandler(string title, Action<string> action)
        {
            if (RegisteredActions.Any(a => a.Id == title))
            {
                throw new InvalidOperationException($"Could not register action {title} into action set {Id} because one has with the same name ahs already been registered");
            }

            RegisteredActions.Add(new ButtonLocalNotificationActionRegistration
            {
                Id = title,
                ActionSetId = Id,
                Title = title,
                Action = action,
            });

            return this;
        }

        public ILocalNotificationActionRegistrar WithDismissActionHandler(Action<string> action)
        {
            RegisteredActions.Add(new LocalNotificationActionRegistration
            {
                Id = LocalNotificationActionRegistration.DismissActionIdentifier,
                ActionSetId = Id,
                Action = action
            });

            return this;
        }

        public void Register()
        {
            var actions = RegisteredActions
                .OfType<ButtonLocalNotificationActionRegistration>()
                .Select(action => UNNotificationAction.FromIdentifier(action.Title, action.Title, UNNotificationActionOptions.Foreground))
                .ToArray();

            var category = UNNotificationCategory.FromIdentifier(
                Id,
                actions,
                new string[] { },
                UNNotificationCategoryOptions.CustomDismissAction);

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(category));
        }

        public string Id { get; }

        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}