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

        public ILocalNotificationActionRegistrar WithActionHandler(string actionId, string displayName, int iconId, Action<string> action)
        {
            RegisteredActions.Add(new LocalNotificationActionRegistration
            {
                Id = actionId,
                ActionSetId = Id,
                DisplayName = displayName,
                Action = action,
            });

            return this;
        }

        public void Register()
        {
            var category = UNNotificationCategory.FromIdentifier(
                Id,
                RegisteredActions.Select(action => UNNotificationAction.FromIdentifier(action.Id, action.DisplayName, UNNotificationActionOptions.Foreground)).ToArray(),
                new string[] { },
                UNNotificationCategoryOptions.CustomDismissAction);

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(category));
        }

        public string Id { get; }

        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}