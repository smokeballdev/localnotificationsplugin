using System;
using Foundation;
using Plugin.LocalNotifications.Abstractions;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    public class LocalNotificationBuilder : ILocalNotificationBuilder
    {
        private readonly int _id;
        private string _title;
        private string _body;
        private string _actionSetId;
        private string _actionSetParameter;

        public LocalNotificationBuilder(int id)
        {
            _id = id;
        }

        public ILocalNotificationBuilder WithIcon(int iconId)
        {
            return this;
        }

        public ILocalNotificationBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public ILocalNotificationBuilder WithBody(string body)
        {
            _body = body;
            return this;
        }

        public ILocalNotificationBuilder WithActionSet(string id, string parameter)
        {
            _actionSetId = id;
            _actionSetParameter = parameter;
            return this;
        }

        public void Show(DateTime notifyAt)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponentsFromDateTime(notifyAt), false);
                ShowUserNotification(trigger);
            }
            else
            {
                var notification = new UILocalNotification
                {
                    FireDate = (NSDate)notifyAt,
                    AlertTitle = _title,
                    AlertBody = _body,
                    UserInfo = GetUserInfo()
                };

                if (_actionSetId != null)
                {
                    notification.Category = _actionSetId;
                }

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
        }

        public void Show()
        {
            Show(DateTime.Now);
        }

        private void ShowUserNotification(UNNotificationTrigger trigger)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                return;
            }

            var content = new UNMutableNotificationContent
            {
                CategoryIdentifier = _actionSetId,
                Title = _title,
                Body = _body,
                UserInfo = GetUserInfo()
            };

            var request = UNNotificationRequest.FromIdentifier(_id.ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, error => { });
        }

        private static NSDateComponents GetNSDateComponentsFromDateTime(DateTime dateTime)
        {
            return new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }

        private NSDictionary GetUserInfo()
        {
            var userInfo = NSMutableDictionary.FromObjectAndKey(NSObject.FromObject(_id), NSObject.FromObject(LocalNotifications.NotificationKey));

            if (_actionSetId != null)
            {
                userInfo.SetValueForKey(NSObject.FromObject(_actionSetParameter), new NSString(UserNotificationCenterDelegate.LocalNotificationActionParameterKey));
            }

            return userInfo;
        }
    }
}