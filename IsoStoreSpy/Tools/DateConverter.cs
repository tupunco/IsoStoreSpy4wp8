using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoStoreSpy.Tools
{
    public class DateConverter
    {
        public static string FormatDate(DateTime date)
        {
            DateTime now = DateTime.Now;

            if (date.Year == now.Year && date.Month == now.Month && date.Day == now.Day)
            {
                return date.ToShortTimeString();
            }
            else
            {
                return date.ToShortDateString();
            }
        }
    }
}
