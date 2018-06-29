using System;

namespace ISOFinder.Domain.MainModule
{
    public class FileDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public string Path { get; set; }

        public decimal Size { get; set; }

        public string SizeString 
        {
            get { return GetSizeString(Size); }
        }

        public string IsoVolumeLabel { get; set; }

        public string IsoFileName { get; set; }

        public int FolderId { get; set; }

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