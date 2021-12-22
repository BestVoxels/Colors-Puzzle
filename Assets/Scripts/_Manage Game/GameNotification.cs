using UnityEngine;
using System; // Being used
#if !UNITY_EDITOR && UNITY_ANDROID
using Unity.Notifications.Android;
#elif !UNITY_EDITOR && UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class GameNotification : MonoBehaviour
{
    private readonly byte[] _notiTimes =
    {
        2,
        4,
        24
    };
    private readonly string[] _text1 =
    {
        "New Endless Mode",
        "New Timer Mode",
        "New Game Types",
        "Dark Theme"
    };
    private readonly string[] _text2 =
    {
        "Give it a Try!",
        "Unlock now and Play!"
    };

    private void Awake() => ClearOldNotification();

    private void OnApplicationPause(bool appPauseStatus)
    {
        if (appPauseStatus == true && NotificationButton.CanSend == true)
        {
            byte curTask = MessageUI.GetTask();
            SendNewNotification(curTask, MessageUI.GetDiff(curTask));
        }
        else if (appPauseStatus == false)
        {
            ClearOldNotification();
        }
    }

#pragma warning disable CS0219
    private void SendNewNotification(byte a, int diff)
    {
        for (byte i = 0; i < 3; i++)
        {
            if (a != 0 || i == 1 || i == 2)
            {
                string text = "";

                if (a != 0)
                {
                    switch (i)
                    {
                        case 0:
                            text = _text1[a - 1] + " ~ " + MessageUI.Prefix(diff) + diff + " more to Unlock!";
                            break;
                        case 1:
                            text = _text2[UnityEngine.Random.Range(0, 2)];
                            break;
                        case 2:
                            text = "Don't wanna try " + _text1[a - 1] + "? ~ " + MessageUI.Prefix(diff) + diff + " more to go!";
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 1:
                            text = "Keep practicing your counting skills! :)";
                            break;
                        case 2:
                            text = "Wanna Train your eyes? :)";
                            break;
                    }
                }


#if !UNITY_EDITOR && UNITY_ANDROID
                // Only Once create
                if (i == 0)
                {
                    var channel = new AndroidNotificationChannel
                    {
                        Id = "channel_default",
                        Name = "Default Channel",
                        Importance = Importance.High,
                        Description = "for notifications to posted to",
                        LockScreenVisibility = LockScreenVisibility.Public
                    };

                    AndroidNotificationCenter.RegisterNotificationChannel(channel);
                }

                // Nothing need to change here inorder to clone this and send as new Notification
                var androidDailyNoti = new AndroidNotification
                {
                    Title = "Colors Puzzle",
                    Text = text,
                    FireTime = DateTime.Now.AddHours(_notiTimes[i]),
                    LargeIcon = "large_icon",
                    SmallIcon = "small_icon"
                };

                // Send Notification with this method
                int identifier = AndroidNotificationCenter.SendNotification(androidDailyNoti, "channel_default");

#elif !UNITY_EDITOR && UNITY_IOS
                var timeTrigger = new iOSNotificationTimeIntervalTrigger
                {
                    TimeInterval = new TimeSpan(_notiTimes[i], 0, 0),
                    Repeats = false
                };

                // Change Identifier inorder to clone this and send as new Notification
                var iOSDailyNoti = new iOSNotification
                {
                    // You can optionally specify a custom Identifier which can later be 
                    // used to cancel the notification, if you don't set one, an unique 
                    // string will be generated automatically.
                    Identifier = "daily notification " + i,
                    //Title = "Colors Puzzle",
                    Body = text,
                    //CategoryIdentifier = "category_a",
                    //ThreadIdentifier = thread,
                    Trigger = timeTrigger,
                    Badge = 1
                };

                iOSNotificationCenter.ScheduleNotification(iOSDailyNoti);
#endif
            }
        }

        // ******IF there is a promotion stuff******
        if (GamePromotion.CurShowing != -1)
        {
            TimeSpan difference = DateTime.Now.Subtract(GamePromotion.OldDate);
            int remainTime = (GamePromotion.TargetClose * 60) - (int)difference.TotalSeconds;
            int priorTime = 1800; // 30 minutes

            if (remainTime > priorTime)
            {
                string text = "Check out! Your promotion is ending soon!";

#if !UNITY_EDITOR && UNITY_ANDROID
                var androidPromoNoti = new AndroidNotification
                {
                    Title = "Colors Puzzle",
                    Text = text,
                    FireTime = DateTime.Now.AddSeconds(remainTime - priorTime),
                    LargeIcon = "large_icon",
                    SmallIcon = "small_icon"
                };

                // Send Notification with this method
                int identifier2 = AndroidNotificationCenter.SendNotification(androidPromoNoti, "channel_default");

#elif !UNITY_EDITOR && UNITY_IOS
                var timeTrigger2 = new iOSNotificationTimeIntervalTrigger
                {
                    TimeInterval = new TimeSpan(0, 0, remainTime - priorTime),
                    Repeats = false
                };

                var iOSPromoNoti = new iOSNotification
                {
                    // You can optionally specify a custom Identifier which can later be 
                    // used to cancel the notification, if you don't set one, an unique 
                    // string will be generated automatically.
                    Identifier = "promotion notification",
                    //Title = "Colors Puzzle",
                    Body = text,
                    //CategoryIdentifier = "category_a",
                    //ThreadIdentifier = thread,
                    Trigger = timeTrigger2,
                    Badge = 1
                };

                iOSNotificationCenter.ScheduleNotification(iOSPromoNoti);
#endif
            }
        }
    }
#pragma warning restore CS0219

    private void ClearOldNotification()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        // Just clear all cuz we can't use identifier variable to check & close when closed the app
        AndroidNotificationCenter.CancelAllNotifications();
#elif !UNITY_EDITOR && UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.ApplicationBadge = 0;
#endif
    }
}
