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
            Id = id;
        }

        public ILocalNotificationActionRegistrar WithActionHandler(string title, Action<string> action)
        {
            if (RegisteredActions.Any(a => a.Id == title))
            {
                throw new InvalidOperationException($"Could not register action {title} into action set {Id} because one has with the same name ahs already been registered");
            }

            RegisteredActions.Add(new ButtonLocalNotificationActionRegistration
            {
                ActionSetId = Id,
                Id = ActionIdentifiers.Action,
                Title = title,
                Action = action
            });

            return this;
        }

        public ILocalNotificationActionRegistrar WithDefaultActionHandler(Action<string> action) => WithUniqueActionHandler(ActionIdentifiers.Default, action);

        public ILocalNotificationActionRegistrar WithDismissActionHandler(Action<string> action) => WithUniqueActionHandler(ActionIdentifiers.Dismiss, action);

        private ILocalNotificationActionRegistrar WithUniqueActionHandler(string id, Action<string> action)
        {
            RegisteredActions.RemoveAll(a => a.Id == id);
            RegisteredActions.Add(new LocalNotificationActionRegistration
            {
                ActionSetId = Id,
                Id = id,
                Action = action
            });

            return this;
        }

        public void Register()
        {
            // Do nothing
        }

        public string Id { get; }
        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}