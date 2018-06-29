using System;
using System.IO;
using System.Windows.Forms;

namespace IsoFinder.IsoScanner.Views
{
    public partial class IsoFolderPath : UserControl, IScannerStep
    {
        private IsoScannerInfo _isoScannerInfo;

        public IsoFolderPath()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        public bool ValidateStep()
        {
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                ShowError("Path cannot be null");
                return false;
            }

            if (!Directory.Exists(txtPath.Text))
            {
                ShowError(string.Format("Folder '{0}' does not exists or do not have access", txtPath.Text));
                return false;
            }

            _isoScannerInfo.IsoFolderPath = txtPath.Text;
            return true;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        public void Execute()
        {
            lblError.Visible = false;

            if (WorkFinished != null)
            {
                WorkFinished();
            }
        }

        public void Initialize(IsoScannerInfo scannerInfo)
        {
            _isoScannerInfo = scannerInfo;
        }

        public IsoScannerInfo Info
        {
            get { return _isoScannerInfo; }
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            lblError.Visible = false;
        }


        public event Action WorkFinished;
    }
}