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
        ILocalNotificationActionRegistrar WithActionHandler(string title, Action<string> action);

        /// <summary>
        /// Registers the default action to be used when the user taps a notification
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ILocalNotificationActionRegistrar WithDefaultActionHandler(Action<string> action);

        /// <summary>
        /// Registers the default dismiss action to be used when the user swipes a notification
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ILocalNotificationActionRegistrar WithDismissActionHandler(Action<string> action);

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
        public virtual string UniqueIdentifier => Id;

        public string ActionSetId { get; set; }
        public string Id { get; set; }
        public int IconId { get; set; }
        public Action<string> Action { get; set; }
    }

    public class ButtonLocalNotificationActionRegistration : LocalNotificationActionRegistration
    {
        public override string UniqueIdentifier => ActionSetId + Title;

        public string Title { get; set; }
    }
}
