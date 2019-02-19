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

        public ILocalNotificationActionRegistrar WithActionHandler(string actionId, string displayName, int iconId, Action<string> action)
        {
            RegisteredActions.Add(new LocalNotificationActionRegistration
            {
                ActionSetId = Id,
                Id = actionId,
                IconId = iconId,
                DisplayName = displayName,
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