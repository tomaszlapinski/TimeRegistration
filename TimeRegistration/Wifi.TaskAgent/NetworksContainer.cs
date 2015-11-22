using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wifi.TaskAgent
{
    public class NetworksContainer
    {
        public List<NetworkItem> Networks { get; set; }
    }

    public class NetworkItem
    {
        public int MinutesInWeek { get; set; }
        public string NetworkName { get; set; }
        public int WeekNumber { get; set; }
    }
}
