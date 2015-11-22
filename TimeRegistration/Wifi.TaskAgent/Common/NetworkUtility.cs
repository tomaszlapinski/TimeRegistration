using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Wifi.TaskAgent.Common
{
    public class NetworkUtility
    {
        public NetworksContainer GetNetworksFromFile()
        {
            NetworksContainer container = null;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists("wifi.xml"))
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile("wifi.xml", FileMode.Open, FileAccess.Read))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(NetworksContainer));
                            container = (NetworksContainer)serializer.Deserialize(stream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return container;
        }

        public void SaveNetworksContainer(NetworksContainer container)
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.FileExists("wifi.xml"))
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile("wifi.xml", FileMode.CreateNew))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(NetworksContainer));
                            serializer.Serialize(stream, container);
                        }
                    }
                    else
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile("wifi.xml", FileMode.Create, FileAccess.ReadWrite))
                        {
                            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                            XmlSerializer serializer = new XmlSerializer(typeof(NetworksContainer));
                            using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                            {
                                serializer.Serialize(xmlWriter, container);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public NetworkItem GetCurrentNetworkFromFile()
        {
            NetworkItem network = new NetworkItem();
            string currentNetwork = GetCurrentNetworkName();
            NetworksContainer container = GetNetworksFromFile();
            foreach (NetworkItem item in container.Networks)
            {
                if (item.NetworkName == currentNetwork)
                {
                    return item;
                }

            }

            return null;
        }

        public string GetCurrentNetworkName()
        {
            NetworkInterfaceList networkInterfaceList = new NetworkInterfaceList();
            StringBuilder networks = new StringBuilder();
            foreach (NetworkInterfaceInfo networkInterfaceInfo in networkInterfaceList)
            {
                if (networkInterfaceInfo.InterfaceType == NetworkInterfaceType.Wireless80211)
                    return networkInterfaceInfo.InterfaceName;
            }

            return string.Empty;
        }

        public void AddCurrentNetworkToFile(NetworksContainer container, string networkName)
        {
            if (container != null)
            {
                if (!string.IsNullOrEmpty(networkName))
                {
                    if (container.Networks == null)
                        container.Networks = new List<NetworkItem>();

                    var networkItem = new NetworkItem();
                    networkItem.NetworkName = networkName;

                    container.Networks.Add(networkItem);
                    SaveNetworksContainer(container);
                }
            }
        }

        public int GetWeekNumber()
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
