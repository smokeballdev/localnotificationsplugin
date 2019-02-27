using Plugin.LocalNotifications.Abstractions;
using System.Linq;
using Foundation;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for iOS
    /// </summary>
    public class LocalNotifications : ILocalNotifications
    {
        public const string NotificationKey = "LocalNotificationKey";

        private static readonly UserNotificationCenterDelegate UserNotificationCenterDelegate = new UserNotificationCenterDelegate();

        public static void Register() => UNUserNotificationCenter.Current.Delegate = UserNotificationCenterDelegate;

        public ILocalNotificationActionRegistrar RegisterActionSet(string id) => UserNotificationCenterDelegate.NewActionRegistrar(id);

        public ILocalNotificationBuilder New(int id) => new LocalNotificationBuilder(id);

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new [] { id.ToString() });
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new [] { id.ToString() });
            }
            else
            {
                var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
                var notification = notifications.Where(n => n.UserInfo.ContainsKey(NSObject.FromObject(NotificationKey)))
                    .FirstOrDefault(n => n.UserInfo[NotificationKey].Equals(NSObject.FromObject(id)));

                if (notification != null)
                {
                    UIApplication.SharedApplication.CancelLocalNotification(notification);
                }
            }
        }
    }
}
