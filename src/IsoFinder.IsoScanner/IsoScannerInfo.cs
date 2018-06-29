using System.Collections.Generic;

namespace IsoFinder.IsoScanner
{
    public class IsoScannerInfo
    {
        public IsoScannerInfo()
        {
            SelectedIsoFileNames = new List<string>();
        }

        public string IsoFolderPath { get; set; }

        public IList<string> SelectedIsoFileNames { get; set; }
    }
}