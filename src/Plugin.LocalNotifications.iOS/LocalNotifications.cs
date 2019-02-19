﻿using Plugin.LocalNotifications.Abstractions;
using System.Linq;
using Foundation;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for iOS
    /// </summary>
    public class LocalNotifications : ILocalNotifications
    {
        public const string NotificationKey = "LocalNotificationKey";

        private readonly UserNotificationCenter _userNotificationCenter; 

        public LocalNotifications()
        {
            _userNotificationCenter = new UserNotificationCenter();

            UNUserNotificationCenter.Current.Delegate = _userNotificationCenter;
        }

        public ILocalNotificationActionRegistrar RegisterActionSet(string id) => _userNotificationCenter.NewActionRegistrar(id);

        public ILocalNotificationBuilder New(int id) => new LocalNotificationBuilder(id);

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new [] { id.ToString() });
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new [] { id.ToString() });
            }
            else
            {
                var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
                var notification = notifications.Where(n => n.UserInfo.ContainsKey(NSObject.FromObject(NotificationKey)))
                    .FirstOrDefault(n => n.UserInfo[NotificationKey].Equals(NSObject.FromObject(id)));

                if (notification != null)
                {
                    UIApplication.SharedApplication.CancelLocalNotification(notification);
                }
            }
        }
    }
}