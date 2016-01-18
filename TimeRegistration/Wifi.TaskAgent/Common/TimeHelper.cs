using System;
using System.Globalization;

namespace Wifi.TaskAgent.Common
{
    public class TimeHelper
    {
        public static int GetWeekNumber()
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
