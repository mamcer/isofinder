using System;
using System.ComponentModel;

namespace IsoFinder.Web.Models
{
    public class SearchInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public DateTime Created { get; set; }

        [DisplayName("Size")]
        public string SizeString { get; set; }

        public static string GetSizeString(decimal size)
        {
            string unit;
            decimal finalSize;
            if (size < 1024)
            {
                unit = "bytes";
                finalSize = size;
            }
            else if (size < 1024 * 1024)
            {
                unit = "KB";
                finalSize = Convert.ToDecimal(size) / 1024;
            }
            else if (size < 1024 * 1024 * 1024)
            {
                unit = "MB";
                finalSize = Convert.ToDecimal(size) / (1024 * 1024);
            }
            else
            {
                unit = "GB";
                finalSize = Convert.ToDecimal(size) / (1024 * 1024 * 1024);
            }

            return string.Format("{0} {1}", Math.Round(finalSize, 2), unit);
        }
    }
}