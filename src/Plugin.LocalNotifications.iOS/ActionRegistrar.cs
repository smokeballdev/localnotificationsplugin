using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Plugin.LocalNotifications.Abstractions;
using Plugin.LocalNotifications.Models;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    public class ActionRegistrar : ILocalNotificationActionRegistrar
    {
        public ActionRegistrar(string categoryId)
        {
            ActionSetId = categoryId;
        }

        public ILocalNotificationActionRegistrar WithActionHandler(string title, Action<string> action)
        {
            if (RegisteredActions.OfType<ButtonLocalNotificationActionRegistration>().Any(a => a.Title == title))
            {
                throw new InvalidOperationException($"Could not register action {title} into action set {ActionSetId} because one has with the same name has already been registered");
            }

            RegisteredActions.Add(new ButtonLocalNotificationActionRegistration
            {
                ActionSetId = ActionSetId,
                ActionId = title,
                Title = title,
                Action = action,
            });

            return this;
        }

        public ILocalNotificationActionRegistrar WithDefaultActionHandler(Action<string> action) => WithUniqueActionHandler(ActionIdentifiers.Default, action);

        public ILocalNotificationActionRegistrar WithDismissActionHandler(Action<string> action) => WithUniqueActionHandler(ActionIdentifiers.Dismiss, action);

        private ILocalNotificationActionRegistrar WithUniqueActionHandler(string actionId, Action<string> action)
        {
            RegisteredActions.RemoveAll(a => a.ActionId == actionId);
            RegisteredActions.Add(new LocalNotificationActionRegistration
            {
                ActionSetId = ActionSetId,
                ActionId = actionId,
                Action = action
            });

            return this;
        }

        public void Register()
        {
            var actions = RegisteredActions
                .OfType<ButtonLocalNotificationActionRegistration>()
                .Select(action => UNNotificationAction.FromIdentifier(action.Id, action.Title, UNNotificationActionOptions.Foreground))
                .ToArray();

            var category = UNNotificationCategory.FromIdentifier(
                ActionSetId,
                actions,
                new string[] { },
                UNNotificationCategoryOptions.CustomDismissAction);

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(category));
            LocalNotifications.Register();
        }

        public string ActionSetId { get; }

        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}