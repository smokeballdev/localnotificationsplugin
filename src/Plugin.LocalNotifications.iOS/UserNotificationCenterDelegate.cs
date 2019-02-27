﻿using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.LocalNotifications.Abstractions;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        private readonly List<LocalNotificationActionRegistrar> _actionRegistrars;

        public const string LocalNotificationActionParameterKey = "LocalNotificationActionParameterKey";

        public UserNotificationCenterDelegate()
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
            var actionSetId = response.Notification.Request.Content.CategoryIdentifier;
            var actionSet = _actionRegistrars.FirstOrDefault(r => r.Id == actionSetId);

            // Perform default dismiss action if available, otherwise perform selected action
            var actionIdentifier = response.IsDismissAction || response.IsDefaultAction ?
                LocalNotificationActionRegistration.DismissActionIdentifier :
                response.ActionIdentifier;

            var action = actionSet?.RegisteredActions.FirstOrDefault(s => s.Id == actionIdentifier);

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