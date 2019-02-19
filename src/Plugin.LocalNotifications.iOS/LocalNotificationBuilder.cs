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
                ShowUserNotification(_title, _body, _id, trigger);
            }
            else
            {
                var notification = new UILocalNotification
                {
                    Category = _actionSetId,
                    FireDate = (NSDate)notifyAt,
                    AlertTitle = _title,
                    AlertBody = _body,
                    UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(_id), NSObject.FromObject(LocalNotifications.NotificationKey)),
                };
                notification.UserInfo.SetValueForKey(NSObject.FromObject(_actionSetParameter), new NSString(""));

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
        }

        public void Show()
        {
            Show(DateTime.Now);
        }

        private void ShowUserNotification(string title, string body, int id, UNNotificationTrigger trigger)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                return;
            }

            var content = new UNMutableNotificationContent
            {
                CategoryIdentifier = _actionSetId,
                Title = title,
                Body = body,
            };
            content.UserInfo.SetValueForKey(NSObject.FromObject(_actionSetParameter), new NSString(""));

            var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, error => { });
        }

        private NSDateComponents GetNSDateComponentsFromDateTime(DateTime dateTime)
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
    }
}