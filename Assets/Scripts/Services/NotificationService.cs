using System;
using Unity.Notifications.Android;
using UnityEngine;

namespace Services
{
    public class NotificationService
    {
        private AndroidNotificationChannel _channel;
        
        public NotificationService()
        {
            _channel = new AndroidNotificationChannel()
            {
                Id = "channel_id",
                Name = "Default Channel",
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            
            AndroidNotificationCenter.RegisterNotificationChannel(_channel);
        }

        public void ShowTestNotification()
        {
            var notification = new AndroidNotification()
            {
                Title = "Test Notification",
                Text = "This is test notification",
                FireTime = DateTime.Now.AddSeconds(5)
            };

            var identifier = AndroidNotificationCenter.SendNotification(notification, _channel.Id);
            
            AndroidNotificationCenter.NotificationReceivedCallback notificationReceivedCallback = delegate(AndroidNotificationIntentData data)
            {
                var msg = "Notification recieved : " + data.Id;
                Debug.Log(msg);
            };

            AndroidNotificationCenter.OnNotificationReceived += notificationReceivedCallback;

            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationIntentData != null)
            {
                Debug.Log("App was opened with notification!");
            }
        }
    }
}
