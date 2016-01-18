using System.Collections.Generic;

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
        public int LastWeekHours { get; set; }
    }
}
