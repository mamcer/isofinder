using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using IsoFinder.IsoScanner.Views;

namespace IsoFinder.IsoScanner
{
    public partial class Main : Form
    {
        private readonly IScannerStep[] _steps;
        private int _currentStep;
        private readonly IsoScannerInfo _isoScannerInfo;

        public string DriveLetter
        {
            get
            {
                if (ConfigurationManager.AppSettings["UnitLetter"] != null)
                {
                    return ConfigurationManager.AppSettings["UnitLetter"];
                }

                return "F";
            }
        }

        public string VCDMountPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["VCDMountPath"] != null)
                {
                    return ConfigurationManager.AppSettings["VCDMountPath"];
                }

                return @"C:\Program Files (x86)\Elaborate Bytes\VirtualCloneDrive\VCDMount.exe";
            }
        }

        public string LogFolder
        {
            get
            {
                if (ConfigurationManager.AppSettings["LogFolder"] != null)
                {
                    return ConfigurationManager.AppSettings["LogFolder"];
                }

                return @"C:\root\tmp\iso-finder\";
            }
        }

        public Main()
        {
            InitializeComponent();

            _isoScannerInfo = new IsoScannerInfo();

            _steps = new IScannerStep[]
                {
                    new IsoFolderPath(),
                    new Analize(), 
                    new Scan(DriveLetter, VCDMountPath, LogFolder)
                };

            foreach (var scannerStep in _steps)
            {
                scannerStep.Initialize(_isoScannerInfo);
                scannerStep.WorkFinished += EnableButtons;
            }

            _currentStep = 0;
            ShowStep();
        }

        private void EnableButtons()
        {
            btnNext.Enabled = _currentStep + 1 < _steps.Length;
            btnPrevious.Enabled = _currentStep > 0;
        }

        private void ShowStep()
        {
            btnNext.Enabled = false;
            btnPrevious.Enabled = false;
            pnlUserControl.Controls.Clear();
            pnlUserControl.Controls.Add((UserControl)_steps[_currentStep]);
            _steps[_currentStep].Execute();
            lblSteps.Text = string.Format("Step {0} of {1}", _currentStep + 1, _steps.Length);
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            if (_steps[_currentStep].ValidateStep())
            {
                if (_currentStep + 1 < _steps.Length)
                {
                    _currentStep += 1;
                    ShowStep();
                }
            }
        }

        private void btnPrevious_Click(object sender, System.EventArgs e)
        {
            if (_currentStep > 0)
            {
                _currentStep -= 1;
                ShowStep();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(LogFolder);
        }
    }
}