using System;

namespace Plugin.LocalNotifications.Abstractions
{
    public interface ILocalNotificationActionRegistrar
    {
        /// <summary>
        /// Register an action to be used with notifications
        /// </summary>
        /// <param name="actionId">Unique identifier for the action</param>
        /// <param name="displayName">Display name for the action</param>
        /// <param name="iconId">Icon identifier for display purposes</param>
        /// <param name="action">Action to be performed when action is selected, parameter is specified when action is applied to a notification</param>
        /// <returns></returns>
        ILocalNotificationActionRegistrar WithActionHandler(string actionId, string displayName, int iconId, Action<string> action);

        /// <summary>
        /// Finish registration
        /// </summary>
        void Register();

        /// <summary>
        /// Id for grouping the registered actions
        /// </summary>
        string Id { get; }
    }

    public class LocalNotificationActionRegistration
    {
        public string ActionSetId { get; set; }
        public string Id { get; set; }
        public int IconId { get; set; }
        public string DisplayName { get; set; }
        public Action<string> Action { get; set; }
    }
}
