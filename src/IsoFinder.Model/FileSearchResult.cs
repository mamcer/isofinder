using System;

namespace IsoFinder.Model
{
    public class FileSearchResult : SearchResult
    {
        public int FolderId { get; set; }

        public string Extension { get; set; }

        public string Path { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public decimal Size { get; set; }

        public string IsoVolumeLabel { get; set; }

        public string SizeString
        {
            get { return GetSizeString(Size); }
        }

        private string GetSizeString(decimal size)
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
