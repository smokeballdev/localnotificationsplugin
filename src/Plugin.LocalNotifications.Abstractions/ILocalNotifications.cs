namespace Plugin.LocalNotifications.Abstractions
{
    /// <summary>
    ///     Local Notification Interface
    /// </summary>
    public interface ILocalNotifications
    {
        /// <summary>
        ///     Register a group of action(s) for use with notifications
        /// </summary>
        /// <param name="id">Category identifier used for grouping actions</param>
        ILocalNotificationActionRegistrar RegisterActionSet(string id);

        /// <summary>
        ///     Build and show a custom local notification given the specified notification id
        /// </summary>
        /// <returns></returns>
        ILocalNotificationBuilder New(int id);

        /// <summary>
        ///     Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        void Cancel(int id);

        /// <summary>
        ///     Cancel all local notifications
        /// </summary>
        void CancelAll();
    }
}
