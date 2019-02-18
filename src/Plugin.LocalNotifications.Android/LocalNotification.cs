using System;
using System.Collections.Generic;

namespace Plugin.LocalNotifications
{
    internal class LocalNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int Id { get; set; }
        public int IconId { get; set; }
        public DateTime NotifyTime { get; set; }
        public List<LocalNotificationAction> Actions { get; set; }
    }

    internal class LocalNotificationAction
    {
        public string Id { get; set; }
        public int IconId { get; set; }
        public string Title { get; set; }
        public string Parameter { get; set; }
    }
}