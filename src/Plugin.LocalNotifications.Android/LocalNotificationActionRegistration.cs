using System;

namespace Plugin.LocalNotifications
{
    internal class LocalNotificationActionRegistration
    {
        public string Id { get; set; }
        public int IconId { get; set; }
        public string Title { get; set; }
        public Action<string> Action { get; set; }
    }
}