using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IsoFinder.IsoScanner.Views
{
    public partial class Analize : UserControl, IScannerStep
    {
        private IsoScannerInfo _isoScannerInfo;

        public Analize()
        {
            InitializeComponent();
        }

        public void AnalizeFolder()
        {
            chkListIso.Visible = false;
            chkListIso.Items.Clear();

            string[] isoFilesPath = GetIsoFilesFromFolder(_isoScannerInfo.IsoFolderPath);
            if (isoFilesPath.Length > 0)
            {
                lblIsoFiles.Text = string.Format("{0} iso files found", isoFilesPath.Length);
                lblIsoFiles.Visible = true;
                Array.Sort(isoFilesPath,
                           (s1, s2) =>
                           string.Compare(Path.GetFileNameWithoutExtension(s1), Path.GetFileNameWithoutExtension(s2),
                                          StringComparison.Ordinal));
                foreach (var isoFilePath in isoFilesPath)
                {
                    var fileName = Path.GetFileName(isoFilePath);
                    chkListIso.Items.Add(fileName);
                }
            }
            else
            {
                ShowError("No iso files found");
            }

            chkListIso.Visible = true;
        }


        private string[] GetIsoFilesFromFolder(string folderPath)
        {
            var isoFiles = Directory.GetFiles(folderPath, "*.iso");
            var binFiles = Directory.GetFiles(folderPath, "*.bin");
            return isoFiles.Concat(binFiles).ToArray();
        }

        public void Execute()
        {
            lblError.Visible = false;
            lblIsoFiles.Visible = false;
            AnalizeFolder();
            if (WorkFinished != null)
            {
                WorkFinished();
            }
        }

        public bool ValidateStep()
        {
            if (chkListIso.CheckedItems.Count > 0)
            {
                _isoScannerInfo.SelectedIsoFileNames.Clear();
                foreach (var item in chkListIso.CheckedItems)
                {
                    _isoScannerInfo.SelectedIsoFileNames.Add(item.ToString());
                }

                return true;
            }

            ShowError("Please select at least one iso file");
            return false;
        }

        public void Initialize(IsoScannerInfo scannerInfo)
        {
            _isoScannerInfo = scannerInfo;
        }

        public IsoScannerInfo Info
        {
            get { return _isoScannerInfo; }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }


        public event Action WorkFinished;
    }
}