using System;

namespace Plugin.LocalNotifications.Abstractions
{
    public class LocalNotificationArgs
    {
        /// <summary>
        /// Parameter set by the consumer of this notification
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// Timestamp that the notification was acted on in UTC
        /// </summary>
        public DateTime TimestampUtc { get; set; }
    }
}
