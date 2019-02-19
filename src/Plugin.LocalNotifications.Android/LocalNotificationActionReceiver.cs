using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Plugin.LocalNotifications.Abstractions;

namespace Plugin.LocalNotifications
{
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Action Broadcast Receiver")]
    public class LocalNotificationActionReceiver : BroadcastReceiver
    {
        public const string LocalNotificationIntentAction = "LocalNotificationIntentAction";
        public const string LocalNotificationActionSetId = "LocalNotificationActionSetId";
        public const string LocalNotificationActionId = "LocalNotificationActionId";
        public const string LocalNotificationActionParameter = "LocalNotificationActionParameter";

        private readonly List<LocalNotificationActionRegistrar> _actionRegistrars;

        public LocalNotificationActionReceiver()
        {
            _actionRegistrars = new List<LocalNotificationActionRegistrar>();
        }

        public ILocalNotificationActionRegistrar NewActionRegistrar(string actionSetId)
        {
            var registrar = new LocalNotificationActionRegistrar(actionSetId);
            _actionRegistrars.Add(registrar);
            return registrar;
        }

        public IEnumerable<LocalNotificationActionRegistration> GetRegisteredActions(string actionSetId) => _actionRegistrars.FirstOrDefault(a => a.Id == actionSetId)?.RegisteredActions;

        public override void OnReceive(Context context, Intent intent)
        {
            var actionSetId = intent.HasExtra(LocalNotificationActionSetId) ? intent.GetStringExtra(LocalNotificationActionSetId) : null;
            var actionId = intent.HasExtra(LocalNotificationActionId) ? intent.GetStringExtra(LocalNotificationActionId) : null;
            var action = GetRegisteredActions(actionSetId).FirstOrDefault(a => a.Title == actionId);

            if (action != null)
            {
                var parameter = intent.HasExtra(LocalNotificationActionParameter) ? intent.GetStringExtra(LocalNotificationActionParameter) : null;
                action.Action(parameter);
            }
        }
    }
}