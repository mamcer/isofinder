using System;
using System.Text;

namespace IsoFinder.FileScanner
{
    public class FileScanResult
    {
        public int ProcessedFileCount { get; set; }

        public int FilesWithErrorCount { get; set; }

        public int ProcessedFolderCount { get; set; }

        public int FoldersWithErrorCount { get; set; }

        public TimeSpan TotalTime { get; set; }

        public StringBuilder Log { get; set; }

        public FileScanResult()
        {
            ProcessedFileCount = 0;
            FilesWithErrorCount = 0;
            ProcessedFolderCount = 0;
            FoldersWithErrorCount = 0;
            Log = new StringBuilder();
        }
    }
}