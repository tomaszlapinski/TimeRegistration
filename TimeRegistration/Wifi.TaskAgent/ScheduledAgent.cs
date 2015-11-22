using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using Wifi.TaskAgent;
using Wifi.TaskAgent.Common;

namespace Wifi.TimeRegistration
{
    public class TaskAgent : ScheduledTaskAgent
    {
        NetworkUtility _networkUtility = new NetworkUtility();

        static TaskAgent()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            try
            {
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));

                UpdateTime();
                NotifyComplete();
            }
            catch (Exception e)
            {

            }
        }

        private void UpdateTime()
        {
            NetworksContainer container = _networkUtility.GetNetworksFromFile();
            string networkName = _networkUtility.GetCurrentNetworkName();

            foreach (var item in container.Networks)
            {
                if (item.NetworkName == networkName)
                {
                    if (_networkUtility.GetWeekNumber() != item.WeekNumber)
                    {
                        item.MinutesInWeek = 0;
                        item.WeekNumber = _networkUtility.GetWeekNumber();
                    }

                    item.MinutesInWeek += 1;
                }

            }

            _networkUtility.SaveNetworksContainer(container);
        }
    }
}