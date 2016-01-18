using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using Wifi.TaskAgent;
using Wifi.TaskAgent.Common;
using Wifi.TimeRegistration.Resources;

namespace Wifi.TimeRegistration.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        NetworkUtility _networkUtility;

        public MainViewModel()
        {
            _networkUtility = new NetworkUtility();
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "";
        public string SampleProperty
        {
            get
            {
                return string.Format("Time Registration - [ Current week : {0} ]", TimeHelper.GetWeekNumber());
            }
        }    

        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            NetworksContainer networkContainer = _networkUtility.GetNetworksFromFile();
            if (networkContainer != null)
            {
                DisplayNetworks(networkContainer);
            }

            this.IsDataLoaded = true;
        }

        public void DeleteNetwork(string networkName)
        {
            NetworksContainer contanier = _networkUtility.GetNetworksFromFile();
            if (contanier != null)
            {
                for (int i = 0; i < contanier.Networks.Count; i++)
                {
                    if (contanier.Networks[i].NetworkName == networkName)
                    {
                        contanier.Networks.Remove(contanier.Networks[i]);
                        _networkUtility.SaveNetworksContainer(contanier);
                    }
                }
            }
        }

        public void DeleteNetworkHours(string networkName)
        {
            var contanier = _networkUtility.GetNetworksFromFile();
            if (contanier != null)
            {
                foreach (NetworkItem item in contanier.Networks)
                {
                    if (item.NetworkName == networkName)
                    {
                        item.MinutesInWeek = 0;
                        _networkUtility.SaveNetworksContainer(contanier);
                    }
                }
            }
        }

        public void InitializeNetworkInformation()
        {
            string currentNetworkName = _networkUtility.GetCurrentNetworkName();
            NetworksContainer contanier = _networkUtility.GetNetworksFromFile();
            if (contanier == null)
            {
                contanier = new NetworksContainer();
                _networkUtility.AddCurrentNetworkToFile(contanier, currentNetworkName);
                return;
            }
            else
            {
                bool networkFound = false;
                foreach (NetworkItem item in contanier.Networks)
                {
                    networkFound |= item.NetworkName == currentNetworkName;
                }

                if (!networkFound)
                    _networkUtility.AddCurrentNetworkToFile(contanier, currentNetworkName);
            }
        }

        private void DisplayNetworks(NetworksContainer networksContainer)
        {
            this.Items.Clear();
            foreach (NetworkItem network in networksContainer.Networks)
            {
                var time = TimeSpan.FromMinutes(network.MinutesInWeek);
                var lastWeek = string.Empty;
                if (network.LastWeekHours > 0)
                {
                    var weekTime = TimeSpan.FromMinutes(network.LastWeekHours);
                    lastWeek = string.Format("  [ Last week worked: {0:hh}:{1:mm} ]", weekTime, weekTime);
                }
                    
                string formatedTime = string.Format("{0:hh}:{1:mm} {2}", time, time, lastWeek);
                this.Items.Add(new ItemViewModel() { LineOne = network.NetworkName, LineTwo = formatedTime });
            }

        }

        private NetworksContainer GetNetworkNetworksContainer()
        {
            NetworksContainer container = null;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.FileExists("wifi.xml"))
                    {
                        return null;
                    }
                    using (IsolatedStorageFileStream stream = store.OpenFile("wifi.xml", FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(NetworksContainer));
                        container = (NetworksContainer)serializer.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return container;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}