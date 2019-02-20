using System;

namespace Plugin.LocalNotifications.Abstractions
{
    public interface ILocalNotificationBuilder
    {
        /// <summary>
        /// Build a notification with the specified icon id
        /// </summary>
        /// <param name="iconId"></param>
        /// <returns></returns>
        ILocalNotificationBuilder WithIcon(int iconId);

        /// <summary>
        /// Build a notification with the specified title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        ILocalNotificationBuilder WithTitle(string title);

        /// <summary>
        /// Build a notification with the specified body
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        ILocalNotificationBuilder WithBody(string body);

        /// <summary>
        /// Build a notification with the specified action
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        ILocalNotificationBuilder WithActionSet(string id, string parameter);

        /// <summary>
        /// Show local notification at the specified time
        /// </summary>
        /// <param name="notifyAt"></param>
        void Show(DateTime notifyAt);

        /// <summary>
        /// Show local notification immediately
        /// </summary>
        void Show();
    }
}