using System;

namespace Plugin.LocalNotifications.Abstractions
{
    public interface ILocalNotificationActionRegistrar
    {
        /// <summary>
        /// Register an action to be used with notifications
        /// </summary>
        /// <param name="title">Unique display name for the action</param>
        /// <param name="action">Action to be performed when action is selected, parameter is specified when action is applied to a notification</param>
        /// <returns></returns>
        ILocalNotificationActionRegistrar WithActionHandler(string title, Action<LocalNotificationArgs> action);

        /// <summary>
        /// Registers the default action to be used when the user taps a notification
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ILocalNotificationActionRegistrar WithDefaultActionHandler(Action<LocalNotificationArgs> action);

        /// <summary>
        /// Registers the default dismiss action to be used when the user swipes a notification
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ILocalNotificationActionRegistrar WithDismissActionHandler(Action<LocalNotificationArgs> action);

        /// <summary>
        /// Finish registration
        /// </summary>
        void Register();

        /// <summary>
        /// Id for grouping the registered actions
        /// </summary>
        string ActionSetId { get; }
    }

    public class LocalNotificationActionRegistration
    {
        /// <summary>
        /// Unique identifier for the action
        /// </summary>
        public virtual string Id => ActionId;

        public string ActionSetId { get; set; }
        public string ActionId { get; set; }
        public int IconId { get; set; }
        public Action<LocalNotificationArgs> Action { get; set; }
    }

    public class ButtonLocalNotificationActionRegistration : LocalNotificationActionRegistration
    {
        /// <summary>
        /// Unique identifier for the action
        /// </summary>
        public override string Id => ActionSetId + Title;

        public string Title { get; set; }
    }
}
