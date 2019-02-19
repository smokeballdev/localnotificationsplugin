using System;

namespace Plugin.LocalNotifications.Abstractions
{
    /// <summary>
    /// Local Notification Interface
    /// </summary>
    public interface ILocalNotifications
    {
        /// <summary>
        /// Register an action for use with notifications
        /// </summary>
        /// <param name="categoryId">Category identifier used for grouping actions</param>
        /// <param name="actionId">Unique identifier for the action</param>
        /// <param name="displayName">Display name for the action</param>
        /// <param name="iconId">Icon identifier for display purposes</param>
        /// <param name="action">Action to be performed when action is selected, parameter is specified when action is applied to a notification</param>
        void RegisterActionHandler(string categoryId, string actionId, string displayName, int iconId, Action<string> action);

        /// <summary>
        /// Build and show a custom local notification
        /// </summary>
        /// <returns></returns>
        ILocalNotificationBuilder New(int id);

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        void Cancel(int id);
    }
}
