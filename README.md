## Local Notifications Plugin for Xamarin and Windows

[![Build status](https://ci.appveyor.com/api/projects/status/5m35u4e23c4neybn/branch/master?svg=true)](https://ci.appveyor.com/project/smokeballdev/localnotificationsplugin-2pvir/branch/master)

A consistent and easy way to show local notifications in Xamarin and Windows apps.

### Setup
* Available on NuGet: [https://www.nuget.org/packages/Xam.Plugins.Notifier/](https://www.nuget.org/packages/Xam.Plugins.Notifier/)
* Install in your PCL project and platform client projects.

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|7.0+|
|Xamarin.Android|3.0+ (API 11+)|


### API Usage

Call `CrossLocalNotifications.Current` from any project or PCL to gain access to APIs.

#### Display a local notification immediately

```csharp
CrossLocalNotifications.Current
  .New(1)
  .WithTitle("title")
  .WithBody("body")
  .Show();
```

#### Display a local notification at a scheduled date/time

```csharp
CrossLocalNotifications.Current
  .New(1)
  .WithTitle("title")
  .WithBody("body")
  .Show(DateTime.Now.AddSeconds(5));
```


#### Display a local notification with action(s)

```csharp
CrossLocalNotifications.Current
  .RegisterActionSet("ReminderActions")
    .WithActionHandler("Dismiss", OnDismiss)
    .WithActionHandler("Snooze", OnSnooze)
    .WithDismissActionHandler(OnDismiss)
    .Register();

CrossLocalNotifications.Current
  .New(1)
  .WithTitle("title")
  .WithBody("body")
  .WithActionSet("ReminderActions", "Parameter")
  .Show(DateTime.Now.AddSeconds(5));
```

Note: Action set identifier must be unique, the identifier may then be used accross different notifications.

#### Cancel a local notification

```csharp
CrossLocalNotifications.Current.Cancel(101);
```


### Platform Specific Notes

Some platforms require certain permissions or settings before it will display notifications. 

#### iOS 8.0+ 
You must get permission from the user to allow the app to show local notifications.

To do so, include the following code in the `FinishedLaunching()` method of `AppDelegate`:

```csharp
if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
{
        // Ask the user for permission to get notifications on iOS 10.0+
        UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, error) => { });
}
else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
{
        // Ask the user for permission to get notifications on iOS 8.0+
        var settings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                new NSSet());

        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
}
```

On iOS 10.0+ in order to specify how notifications are handled when the app is active you must create a delegate class 
that subclasses `UNUserNotificationCenterDelegate` and assign it to the `UNUserNotificationCenter`.

For more details see the [sample included in this repository](https://github.com/edsnider/LocalNotificationsPlugin/tree/master/samples/LocalNotificationsSample/LocalNotificationsSample.iOS) 
and check out [Xamarin's iOS 10 UserNotifications framework documentation](https://developer.xamarin.com/guides/ios/platform_features/introduction-to-ios10/user-notifications/).

#### Android
Currently, if the phone is re-booted then the pending notifications are not sent, you should save them out to settings and re-send on re-boot.

### Contributors

* [James Montemagno](https://github.com/jamesmontemagno)

### License

Licensed under MIT see [License file](https://github.com/edsnider/LocalNotificationsPlugin/blob/master/LICENSE)
