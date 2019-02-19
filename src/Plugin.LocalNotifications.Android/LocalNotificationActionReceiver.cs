using System.Collections.Generic;
using System.Linq;
using Android.Content;

namespace Plugin.LocalNotifications
{
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Action Broadcast Receiver")]
    internal class LocalNotificationActionReceiver : BroadcastReceiver
    {
        public const string LocalNotificationIntentAction = "LocalNotificationIntentAction";
        public const string LocalNotificationActionId = "LocalNotificationActionId";
        public const string LocalNotificationActionParameter = "LocalNotificationActionParameter";

        private readonly List<LocalNotificationActionRegistration> _registeredActions;

        public LocalNotificationActionReceiver()
        {
            _registeredActions = new List<LocalNotificationActionRegistration>();
        }

        public void Register(LocalNotificationActionRegistration actionRegistration) => _registeredActions.Add(actionRegistration);

        public LocalNotificationActionRegistration GetRegisteredAction(string id) => _registeredActions.FirstOrDefault(a => a.Id == id);

        public override void OnReceive(Context context, Intent intent)
        {
            var actionId = intent.HasExtra(LocalNotificationActionId) ? intent.GetStringExtra(LocalNotificationActionId) : null;
            var action = GetRegisteredAction(actionId);

            if (action != null)
            {
                var parameter = intent.HasExtra(LocalNotificationActionParameter) ? intent.GetStringExtra(LocalNotificationActionParameter) : null;
                action.Action(parameter);
            }
        }
    }
}