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
            
            AndroidNotificationCenter.CancelAllScheduledNotifications();
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
            
            SetReminderNotification(24);
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
        }

        private void SetReminderNotification(float hours)
        {
            var notification = new AndroidNotification()
            {
                Title = "Hey, come back!",
                Text = "Collect ALL coins!",
                FireTime = DateTime.Now.AddHours(hours),
                LargeIcon = "default_icon",
                SmallIcon = "icon_small",
                RepeatInterval = TimeSpan.FromHours(3)
            };

            AndroidNotificationCenter.CancelAllScheduledNotifications();
            AndroidNotificationCenter.SendNotification(notification, _reminderChannel.Id);
            
            AndroidNotificationCenter.OnNotificationReceived += NotificationReceivedCallback;
        }

        public void ShowFreeCoinsNotification(float delay)
        {
            var notification = new AndroidNotification()
            {
                Title = "Free coins!",
                Text = "Collect your free coins now!",
                FireTime = DateTime.Now.AddSeconds(delay),
                LargeIcon = "default_icon",
                SmallIcon = "icon_small"
            };
           
            AndroidNotificationCenter.CancelAllScheduledNotifications();
            AndroidNotificationCenter.SendNotification(notification, _freeCoinsChannel.Id);
            
            AndroidNotificationCenter.OnNotificationReceived += NotificationReceivedCallback;
        }
        
        private void NotificationReceivedCallback(AndroidNotificationIntentData data)
        {
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
            AndroidNotificationCenter.OnNotificationReceived -= NotificationReceivedCallback;
        }
    }
}
