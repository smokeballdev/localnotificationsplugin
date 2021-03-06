using System;
using System.Collections.Generic;

namespace Plugin.LocalNotifications
{
    public class LocalNotification
    {
        public const string NotificationId = "LocalNotification.NotificationId";
        public const string ActionSetId = "LocalNotification.ActionSetId";
        public const string ActionId = "LocalNotification.ActionId";
        public const string ActionParameter = "LocalNotification.ActionParameter";

        /// <summary>
        /// Unique notification id
        /// </summary>
        public int Id { get; set; }

        public int IconId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime NotifyTime { get; set; }
        public List<LocalNotificationAction> Actions { get; set; }
    }

    public class LocalNotificationAction
    {
        public string Id { get; set; }
        public string ActionId { get; set; }
        public string ActionSetId { get; set; }
        public int IconId { get; set; }
        public string Title { get; set; }
        public string Parameter { get; set; }
    }
}