using System.Collections.Generic;
using System.Linq;
using Android.Content;

namespace Plugin.LocalNotifications
{
    internal class LocalNotificationActionReceiver : BroadcastReceiver
    {
        public const string LocalNotificationActionParameterKey = "LocalNotificationActionParameter";

        private readonly List<LocalNotificationActionRegistration> _registeredActions;

        public LocalNotificationActionReceiver()
        {
            _registeredActions = new List<LocalNotificationActionRegistration>();
        }

        public void Register(LocalNotificationActionRegistration actionRegistration) => _registeredActions.Add(actionRegistration);

        public LocalNotificationActionRegistration GetRegisteredAction(string id) => _registeredActions.FirstOrDefault(a => a.Id == id);

        public override void OnReceive(Context context, Intent intent)
        {
            var action = GetRegisteredAction(intent.Action);

            if (action != null)
            {
                var key = LocalNotificationActionParameterKey + intent.Action;
                var parameter = intent.HasExtra(key) ? intent.GetStringExtra(key) : null;
                action.Action(parameter);
            }
        }

    }
}