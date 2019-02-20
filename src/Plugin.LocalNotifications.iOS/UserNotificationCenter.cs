using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.LocalNotifications.Abstractions;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    internal class UserNotificationCenter : UNUserNotificationCenterDelegate
    {
        private readonly List<LocalNotificationActionRegistrar> _actionRegistrars;

        public const string LocalNotificationActionParameterKey = "LocalNotificationActionParameterKey";

        public UserNotificationCenter()
        {
            _actionRegistrars = new List<LocalNotificationActionRegistrar>();
        }

        public ILocalNotificationActionRegistrar NewActionRegistrar(string id)
        {
            if (_actionRegistrars.Any(a => a.Id == id))
            {
                throw new InvalidOperationException($"Could not register action set '{id}' because an action set with the same id already exists.");
            }

            var registrar = new LocalNotificationActionRegistrar(id);
            _actionRegistrars.Add(registrar);
            return registrar;
        } 

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var categoryId = response.Notification.Request.Content.CategoryIdentifier;
            var actionSet = _actionRegistrars.FirstOrDefault(r => r.Id == categoryId);

            var action = actionSet?.RegisteredActions.FirstOrDefault(s => s.Title == response.ActionIdentifier);

            if (action != null)
            {
                var parameter = response.Notification.Request.Content.UserInfo[LocalNotificationActionParameterKey]?.ToString();
                action.Action(parameter);
            }
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}