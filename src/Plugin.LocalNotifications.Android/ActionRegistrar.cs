using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.LocalNotifications.Abstractions;
using Plugin.LocalNotifications.Models;

namespace Plugin.LocalNotifications
{
    public class ActionRegistrar : ILocalNotificationActionRegistrar
    {
        public ActionRegistrar(string id)
        {
            ActionSetId = id;
        }

        public ILocalNotificationActionRegistrar WithActionHandler(string title, Action<LocalNotificationArgs> action)
        {
            if (RegisteredActions.OfType<ButtonLocalNotificationActionRegistration>().Any(a => a.Title == title))
            {
                throw new InvalidOperationException($"Could not register action {title} into action set {ActionSetId} because one has with the same name has already been registered");
            }

            RegisteredActions.Add(new ButtonLocalNotificationActionRegistration
            {
                ActionSetId = ActionSetId,
                ActionId = ActionIdentifiers.Action,
                Title = title,
                Action = action
            });

            return this;
        }

        public ILocalNotificationActionRegistrar WithDefaultActionHandler(Action<LocalNotificationArgs> action) => WithUniqueActionHandler(ActionIdentifiers.Default, action);

        public ILocalNotificationActionRegistrar WithDismissActionHandler(Action<LocalNotificationArgs> action) => WithUniqueActionHandler(ActionIdentifiers.Dismiss, action);

        private ILocalNotificationActionRegistrar WithUniqueActionHandler(string actionId, Action<LocalNotificationArgs> action)
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
            // Do nothing
        }

        public string ActionSetId { get; }
        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}