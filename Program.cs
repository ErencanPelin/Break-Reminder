using System;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading;
using System.Windows;

namespace FriendlyReminder
{
    class Program
    {
        static void Main(string[] args)
        {
            ToastButton doneButton = new ToastButton();
            doneButton
                .AddArgument("action", "done")
                .SetContent("All Done!");
                //.SetImageUri(new Uri("Assets/NotificationButtonIcons/Complete.png", UriKind.Relative))
                //.SetBackgroundActivation();
            
            ToastButton dismissButton = new ToastButton();
            dismissButton
                .AddArgument("action", "dismiss")
                .SetContent("Ignore")
                .SetImageUri(new Uri("Assets/NotificationButtonIcons/Dismiss.png", UriKind.Relative))
                .SetBackgroundActivation();

            StatusChecker._toast = new ToastContentBuilder();
            StatusChecker._toast.AddArgument("action", "viewConversation")
     .AddArgument("conversationId", 9813)
     .AddText("Break Time!")
     .AddText("Stand up and stretch!")
     .AddButton(doneButton)
     .AddButton(dismissButton);

            //initialise
            Console.WriteLine("After how many minutes do you want to be reminded?");

            int notifyPeriod = 0;
            while (notifyPeriod <= 1)
            {
                int.TryParse(Console.ReadLine(), out notifyPeriod);

                if (notifyPeriod <= 1)
                    if (notifyPeriod == 0)
                    {
                        notifyPeriod = 1;
                        break;
                    }
                    else
                        Console.WriteLine("Enter a number greater than 1");
                else
                    Console.WriteLine("");
            }

            Console.WriteLine("You will be notified every " + notifyPeriod.ToString() + " minutes to stand up! :)");
            Console.WriteLine("");
            Console.WriteLine("PRESS ANY KEY TO STOP");

            //repeat this every interval * 60000

            //METHOD 2
            /*Task task = new Task(() =>
            {
                while (true)
                {
                    ToastNotificationManagerCompat.History.Clear();
                    CallNotify();
                    Thread.Sleep(notifyPeriod * 60000); //move this line to the top when done
                }
            task.Start();
            });*/

            //METHOD 3
            // Create an AutoResetEvent to signal the timeout threshold in the
            // timer callback has been reached.
            var autoEvent = new AutoResetEvent(false);

            var statusChecker = new StatusChecker(10);

            // Create a timer that invokes CheckStatus after one second, 
            // and every 1/4 second thereafter.

            var stateTimer = new Timer(statusChecker.CheckStatus, autoEvent, 1000, 100000);
            Console.ReadKey();
        }
    }

    class StatusChecker
    {
        public static ToastContentBuilder _toast;

        private int invokeCount;
        private int maxCount;

        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

            //do stuff here
            Console.WriteLine("break time!");
            CallNotify();
            invokeCount += 1;

            // Reset the counter and signal the waiting thread.
            //invokeCount = 0;
            autoEvent.Set();
        }

        public static void CallNotify()
        {
            _toast
    .Show(toast =>
    {
        toast.ExpirationTime = DateTime.Now.AddMinutes(.1);
    });
        }
    }
}
