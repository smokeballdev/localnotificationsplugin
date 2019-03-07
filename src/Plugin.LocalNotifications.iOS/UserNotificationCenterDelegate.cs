using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.LocalNotifications.Abstractions;
using Plugin.LocalNotifications.Models;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        private readonly List<ActionRegistrar> _actionRegistrars;

        public const string LocalNotificationActionParameterKey = "LocalNotificationActionParameterKey";

        public UserNotificationCenterDelegate()
        {
            _actionRegistrars = new List<ActionRegistrar>();
        }

        public ILocalNotificationActionRegistrar NewActionRegistrar(string id)
        {
            if (_actionRegistrars.Any(a => a.Id == id))
            {
                throw new InvalidOperationException($"Could not register action set '{id}' because an action set with the same id already exists.");
            }

            var registrar = new ActionRegistrar(id);
            _actionRegistrars.Add(registrar);
            return registrar;
        } 

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var actionSetId = response.Notification.Request.Content.CategoryIdentifier;
            var actionSet = _actionRegistrars.FirstOrDefault(r => r.Id == actionSetId);

            // Create a unique identifier for actions
            string actionIdentifier = actionSetId + response.ActionIdentifier;

            if (response.IsDismissAction)
            {
                actionIdentifier = ActionIdentifiers.Dismiss;
            }

            if (response.IsDefaultAction)
            {
                actionIdentifier = ActionIdentifiers.Default;
            }

            var action = actionSet?.RegisteredActions.FirstOrDefault(s => s.UniqueIdentifier == actionIdentifier);

            if (action != null)
            {
                var parameter = response.Notification.Request.Content.UserInfo[LocalNotificationActionParameterKey]?.ToString();
                action.Action(parameter);
            }

            completionHandler();
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}