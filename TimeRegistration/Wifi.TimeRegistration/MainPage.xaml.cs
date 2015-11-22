using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Wifi.TimeRegistration.Resources;
using Microsoft.Phone.Scheduler;
using Wifi.TimeRegistration.ViewModels;

namespace Wifi.TimeRegistration
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            DataContext = App.ViewModel;
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.InializeNetworkInformation();
                App.ViewModel.LoadData();

                StartPeriodicAgent();
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarMenuItem refreshMenuItem = new ApplicationBarMenuItem("Refresh");
            refreshMenuItem.Click += appBarMenuItem_Click;
            ApplicationBar.MenuItems.Add(refreshMenuItem);
        }

        void appBarMenuItem_Click(object sender, EventArgs e)
        {
            App.ViewModel.LoadData();
        }

        private void StartPeriodicAgent()
        {
            string periodicTaskName = "TimeRegistration";
            PeriodicTask periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);
            try
            {
                periodicTask.Description = "Time registration";
                ScheduledActionService.Add(periodicTask);
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    MessageBox.Show("The maximum number of ScheduledActions of this type have already been added.");
                }
            }
            catch (SchedulerServiceException)
            {
                MessageBox.Show("There is something wrong with starting background agent");
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
                MessageBox.Show("There is something wrong with removing background agent");
            }
        }

        private void DeleteHours_Click(object sender, RoutedEventArgs e)
        {
            ItemViewModel vm = GetViewModel(sender);
            if (vm != null)
                App.ViewModel.DeleteNetworkHours(vm.LineOne);
        }

        private void DeleteNetwork_Click(object sender, RoutedEventArgs e)
        {
            ItemViewModel vm = GetViewModel(sender);
            if (vm != null)
                App.ViewModel.DeleteNetwork(vm.LineOne);
        }

        private ItemViewModel GetViewModel(object sender)
        {
            ItemViewModel viewModel = null;
            var menuItem = (MenuItem)sender;
            if (menuItem != null)
            {
                var ctxMenu = (ContextMenu)menuItem.Parent;
                if (ctxMenu != null)
                {
                    viewModel = (ViewModels.ItemViewModel)ctxMenu.DataContext;
                }
            }

            return viewModel;
        }

    }
}