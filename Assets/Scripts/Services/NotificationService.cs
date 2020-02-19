using System;
using Unity.Notifications.Android;
using UnityEngine;

namespace Services
{
    public class NotificationService
    {
        private AndroidNotificationChannel _reminderChannel;
        private AndroidNotificationChannel _freeCoinsChannel;
        
        public NotificationService()
        {
            _reminderChannel = new AndroidNotificationChannel()
            {
                Id = "channel_reminder",
                Name = "Reminder Channel",
                Importance = Importance.High,
                Description = "Notifications to remind player to play game.",
            };
            
            _freeCoinsChannel = new AndroidNotificationChannel()
            {
                Id = "channel_free_coins",
                Name = "Free Coins Channel",
                Importance = Importance.High,
                Description = "Notifications to remind player to collect free coins.",
            };
            
            AndroidNotificationCenter.RegisterNotificationChannel(_reminderChannel);
            AndroidNotificationCenter.RegisterNotificationChannel(_freeCoinsChannel);

            Application.quitting += () => { SetReminderNotification(1); };
            
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
        }

        public void ShowTestNotification()
        {
            var notification = new AndroidNotification()
            {
                Title = "Test Notification",
                Text = "This is test notification",
                FireTime = DateTime.Now.AddSeconds(5)
            };

            var identifier = AndroidNotificationCenter.SendNotification(notification, _reminderChannel.Id);
            
            AndroidNotificationCenter.NotificationReceivedCallback notificationReceivedCallback = delegate(AndroidNotificationIntentData data)
            {
                var msg = "Notification recieved : " + data.Id;
                Debug.Log(msg);
            };

            AndroidNotificationCenter.OnNotificationReceived += notificationReceivedCallback;
        }

        private void SetReminderNotification(float hours)
        {
            var notification = new AndroidNotification()
            {
                Title = "Hey, come back!",
                Text = "Collect ALL coins!",
                FireTime = DateTime.Now.AddHours(hours),
                SmallIcon = "default_icon"
            };

            AndroidNotificationCenter.DeleteNotificationChannel(_reminderChannel.Id);
            AndroidNotificationCenter.RegisterNotificationChannel(_reminderChannel);
            AndroidNotificationCenter.SendNotification(notification, _reminderChannel.Id);
        }

        public void ShowFreeCoinsNotification(float delay)
        {
            var notification = new AndroidNotification()
            {
                Title = "Free coins!",
                Text = "Collect your free coins now!",
                FireTime = DateTime.Now.AddSeconds(delay),
                SmallIcon = "default_icon"
            };

            AndroidNotificationCenter.DeleteNotificationChannel(_freeCoinsChannel.Id);
            AndroidNotificationCenter.RegisterNotificationChannel(_freeCoinsChannel);
            AndroidNotificationCenter.SendNotification(notification, _freeCoinsChannel.Id);
        }
    }
}
