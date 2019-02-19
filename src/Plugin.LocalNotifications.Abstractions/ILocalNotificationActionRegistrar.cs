using System;

namespace Plugin.LocalNotifications.Abstractions
{
    public interface ILocalNotificationActionRegistrar
    {
        /// <summary>
        /// Register an action to be used with notifications
        /// </summary>
        /// <param name="title">Unique display name for the action</param>
        /// <param name="iconId">Icon identifier for display purposes</param>
        /// <param name="action">Action to be performed when action is selected, parameter is specified when action is applied to a notification</param>
        /// <returns></returns>
        ILocalNotificationActionRegistrar WithActionHandler(string title, int iconId, Action<string> action);

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
        public string Title { get; set; }
        public int IconId { get; set; }
        public Action<string> Action { get; set; }
    }
}
