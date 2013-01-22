using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoStoreSpy.Tools
{
    public class SizeConverter
    {
        private const long TeraByte = 1024 * GigaByte;
        private const long GigaByte = 1024 * MegaByte;
        private const long MegaByte = 1024 * KiloByte;
        private const long KiloByte = 1024;

        public static string FormatFileSize(long bytes)
        {
            if (bytes > TeraByte)
            {
                return ((float)bytes / (float)TeraByte).ToString("0TB");
            }
            else if (bytes > GigaByte)
            {
                return ((float)bytes / (float)GigaByte).ToString("0GB");
            }
            else if (bytes > MegaByte)
            {
                return (((float)bytes / (float)MegaByte)).ToString("0MB");
            }
            else if (bytes > KiloByte)
            {
                return ((float)bytes / (float)KiloByte).ToString("0KB");
            }
            else return bytes + " Bytes";
        }
    }
}
