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
                IconId = iconId,
                Action = action
            });

            return this;
        }

        // TODO: Consider removing
        public void Register()
        {
            // Do nothing
        }

        public string Id { get; }
        public List<LocalNotificationActionRegistration> RegisteredActions { get; } = new List<LocalNotificationActionRegistration>();
    }
}