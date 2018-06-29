using IsoFinder.Core;
using IsoFinder.Data;
using IsoFinder.FileScanner;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualDrive;

namespace IsoFinder.IsoScanner.Views
{
    public partial class Scan : UserControl, IScannerStep
    {
        private IsoScannerInfo _isoScannerInfo;
        private readonly FileScanner.FileScanner _fileScanner = new FileScanner.FileScanner();
        private IUnitOfWork _unitOfWork;
        private VirtualCloneDriveWrapper VirtualCloneDrive { get; set; }
        private DateTime TotalScanStartTime { get; set; }
        private string _driveLetter;
        private string _vcdmountPath;
        private string _logFolder;

        public Scan(string driveLetter, string vcdmountPath, string logFolder)
        {
            InitializeComponent();
            _driveLetter = driveLetter;
            _vcdmountPath = vcdmountPath;
            _logFolder = logFolder;
        }

        public void Initialize(IsoScannerInfo scannerInfo)
        {
            _isoScannerInfo = scannerInfo;
            VirtualCloneDrive = new VirtualCloneDriveWrapper(_driveLetter, _vcdmountPath);
            _unitOfWork = new UnitOfWork();
        }

        private void ShowLogMessage(string message)
        {
            txtConsole.Text += string.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), message) + Environment.NewLine;
        }

        private void ShowInformationMessage(string message)
        {
            lblInformation.Text = message;
            lblInformation.Visible = true;
        }

        private void SaveScanLog(string isoFileName, string log)
        {
            try
            {
                DateTime now = DateTime.Now;
                string fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}_{6}.txt", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, isoFileName);
                File.WriteAllText(Path.Combine(_logFolder, fileName), log);
            }
            catch
            {
                ShowLogMessage(string.Format("Cannot save log file for '{0}'.", isoFileName));
            }
        }

        private async Task<bool> UnMount()
        {
            ShowLogMessage("Unmounting file...");
            var result = await VirtualCloneDrive.UnMountAsync();
            if (!result.HasError)
            {
                ShowLogMessage("Unmount completed");
                return true;
            }

            ShowLogMessage("Unmount Error: " + result.ErrorMessage);
            return false;
        }

        public async void Execute()
        {
            txtConsole.Text = string.Empty;
            lblInformation.Visible = false;

            ShowLogMessage("Scan started...");
            TotalScanStartTime = DateTime.Now;

            var currentIsoFilePathIndex = 0;
            var isoFilesCount = _isoScannerInfo.SelectedIsoFileNames.Count();
            var isoFilesFoundMessage = string.Format("{0} iso files found", isoFilesCount);
            ShowInformationMessage(isoFilesFoundMessage);
            ShowLogMessage(isoFilesFoundMessage);
            var unmountResult = await UnMount();
            if (!unmountResult)
            {
                ShowLogMessage(string.Format("Error trying to unmount file from unit. Process aborted."));
                return;
            }

            foreach (var isoFileName in _isoScannerInfo.SelectedIsoFileNames)
            {
                var isoFilePath = Path.Combine(_isoScannerInfo.IsoFolderPath, isoFileName); 
                var mountResult = await VirtualCloneDrive.MountAsync(isoFilePath);
                if (mountResult.HasError)
                {
                    ShowLogMessage(string.Format("Error trying to mount: {0}", Path.GetFileName(isoFilePath)));
                }
                else
                {
                    var volumeName = VirtualCloneDrive.VolumeLabel;
                    ShowLogMessage(string.Format("Scanning: {0}...", volumeName));
                    ShowInformationMessage(string.Format("Scanning: {0} [{1} of {2}]", volumeName,
                                                         currentIsoFilePathIndex + 1,
                                                          isoFilesCount));

                    if (!_unitOfWork.IsoVolumeRepository.Search(i => i.VolumeLabel == volumeName).Any())
                    {
                        var initialFolder = new IsoFolder { Name = "$", Path = "/", IsoVolumeId = 0 };
                        var iso = new IsoVolume
                        {
                            Created = DateTime.Now,
                            FileName = Path.GetFileName(isoFileName),
                            Size = Convert.ToDecimal(VirtualCloneDrive.TotalSize),
                            VolumeLabel = volumeName,
                            FileCount = 0,
                            RootFolder = initialFolder
                        };

                        _unitOfWork.IsoVolumeRepository.Insert(iso);
                        _unitOfWork.Save();

                        var fileScanResult = new FileScanResult();
                        try
                        {
                            var rootFolder = _fileScanner.ScanFolderForFiles(VirtualCloneDrive.UnitLetter, fileScanResult, iso);
                            iso.FileCount = fileScanResult.ProcessedFileCount;
                            iso.RootFolder = rootFolder;
                        }
                        catch(Exception ex)
                        {
                            ShowLogMessage(string.Format("Error scanning iso file: {0}", Path.GetFileName(isoFilePath)));
                            ShowLogMessage(ex.Message);
                        }

                        ShowLogMessage(string.Format(
                            "Scan finished for {0} ({1}), scan duration: {2}, files processed: {3}",
                            volumeName, iso.FileName,
                            fileScanResult.TotalTime.ToString("hh\\:mm\\:ss"),
                            fileScanResult.ProcessedFileCount));
                        try
                        {
                            ShowLogMessage("Saving changes to the database...");
                            _unitOfWork.IsoVolumeRepository.Update(iso);
                            _unitOfWork.IsoFolderRepository.Delete(initialFolder);
                            _unitOfWork.Save();
                            ShowLogMessage("Changes successfully saved");
                        }
                        catch (Exception ex)
                        {
                            _unitOfWork.IsoVolumeRepository.Delete(iso);
                            _unitOfWork.IsoFolderRepository.Delete(initialFolder);
                            _unitOfWork.Save();
                            ShowLogMessage("An error has ocurred saving changes to database");
                            fileScanResult.Log.Append(ex.Message);
                            fileScanResult.Log.Append(ex.StackTrace);
                        }

                        try
                        {
                            ShowLogMessage("Saving log file...");
                            SaveScanLog(iso.FileName, fileScanResult.Log.ToString());
                            ShowLogMessage(string.Format("Log file successfully saved for {0} ", iso.FileName));
                        }
                        catch
                        {
                            ShowLogMessage("Failed to save log file");
                            ShowLogMessage(fileScanResult.Log.ToString());                        
                        }
                    }
                    else
                    {
                        ShowLogMessage(string.Format("Iso Volume '{0}' already exists in the database.", volumeName));
                    }

                    unmountResult = await UnMount();
                    if (!unmountResult)
                    {
                        ShowLogMessage(string.Format("Error trying to unmount '{0}'. Process aborted.", isoFilePath));
                        break;
                    }
                }

                currentIsoFilePathIndex += 1;
            }

            ShowLogMessage("Scan finished");
            var totalTime = string.Format("Total time: {0}",
                                          DateTime.Now.Subtract(TotalScanStartTime).ToString("hh\\:mm\\:ss"));
            ShowLogMessage(totalTime);
            ShowInformationMessage(string.Format("Scan finished. {0}", totalTime));

            if (WorkFinished != null)
            {
                WorkFinished();
            }
        }

        public bool ValidateStep()
        {
            return true;
        }

        public IsoScannerInfo Info
        {
            get { return _isoScannerInfo; }
        }

        public event Action WorkFinished;
    }
}