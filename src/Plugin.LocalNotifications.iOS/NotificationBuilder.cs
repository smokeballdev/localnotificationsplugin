using System;
using Foundation;
using Plugin.LocalNotifications.Abstractions;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    public class NotificationBuilder : ILocalNotificationBuilder
    {
        private readonly int _id;
        private string _title;
        private string _body;
        private string _actionSetId;
        private string _actionSetParameter;

        public NotificationBuilder(int id)
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
            Console.WriteLine($"Will show notification (id: {_id}) with title '{_title}' at {notifyAt}");

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
                    UserInfo = GetUserInfo(),
                };

                if (_title != null)
                {
                    notification.AlertTitle = _title;
                }

                if (_body != null)
                {
                    notification.AlertBody = _body;
                }

                if (_actionSetId != null)
                {
                    notification.Category = _actionSetId;
                }

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
        }

        public void Show()
        {
            Show(DateTime.Now.AddSeconds(2));
        }

        private void ShowUserNotification(UNNotificationTrigger trigger)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                return;
            }

            var content = new UNMutableNotificationContent
            {
                UserInfo = GetUserInfo(),
            };

            if (_title != null)
            {
                content.Title = _title;
            }

            if (_body != null)
            {
                content.Body = _body;
            }

            if (_actionSetId != null)
            {
                content.CategoryIdentifier = _actionSetId;
            }

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