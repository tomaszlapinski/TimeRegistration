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
        object _lock = new object();

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

                UpdateNetworkInformation();
                NotifyComplete();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error. There is something wrong with the background agent");
                return;
            }
        }

        private void UpdateNetworkInformation()
        {
            NetworksContainer container = _networkUtility.GetNetworksFromFile();
            if (container == null)
            {
                MessageBox.Show("Error. There is something wrong with the contaner...");
                return;
            }

            string networkName = _networkUtility.GetCurrentNetworkName();
            if (!string.IsNullOrEmpty(networkName))
            {
                NetworkItem networkItem = GetNetworkItem(container, networkName);
                if (networkItem == null)
                {
                    networkItem = new NetworkItem();
                    networkItem.NetworkName = networkName;
                    networkItem.WeekNumber = TimeHelper.GetWeekNumber();
                    networkItem.MinutesInWeek += 1;
                    container.Networks.Add(networkItem);

                    ShellToast toast = new ShellToast();
                    toast.Content = string.Format("New network: {0} detected. Date: {1}. Time {2}", networkName, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                    toast.Title = "Info";
                    toast.Show();
                }
                else
                {
                    if (TimeHelper.GetWeekNumber() != networkItem.WeekNumber)
                    {
                        networkItem.LastWeekHours = networkItem.MinutesInWeek;
                        networkItem.MinutesInWeek = 0;
                        networkItem.WeekNumber = TimeHelper.GetWeekNumber();
                    }

                    networkItem.MinutesInWeek += 1;
                }
                _networkUtility.SaveNetworksContainer(container);
            }
        }

        private NetworkItem GetNetworkItem(NetworksContainer networkContainer, string networkName)
        {
            foreach (var item in networkContainer.Networks)
            {
                if (item.NetworkName == networkName)
                {
                    return item;
                }
            }

            return null;
        }
    }
}