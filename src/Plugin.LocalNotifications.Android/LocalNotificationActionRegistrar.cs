using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.LocalNotifications.Abstractions;

namespace Plugin.LocalNotifications
{
    public class LocalNotificationActionRegistrar : ILocalNotificationActionRegistrar
    {
        public LocalNotificationActionRegistrar(string id)
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
                Id = title,
                Title = title,
                Action = action
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
            // Do nothing
        }

        public string Id { get; }
        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}