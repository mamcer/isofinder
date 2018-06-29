using System.Data.Entity;
using IsoFinder.FileScanner;
using IsoFinder.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VirtualDrive;

namespace ISOFinder.WPF.Spider
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public partial class MainWindow
    {
        private VirtualCloneDriveWrapper VirtualCloneDrive {get; set;}

        private List<string> IsoFilesPath { get; set; }

        private DateTime TotalScanStartTime { get; set; }

        private IsoFinderEntities _entities;

        private IsoFinderEntities Entities
        { 
            get
            {
                return _entities ?? (_entities = new IsoFinderEntities());
            }
        }

        private FileScanner _fileScanner;

        private FileScanner IsoFileScanner {
            get
            {
                if (_fileScanner == null)
                {
                    _fileScanner = new FileScanner(new DirectoryProvider(), new FileProvider());
                }

                return _fileScanner;
            }
        }

        public string LogFolder { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var unitLetter = ConfigurationManager.AppSettings["UnitLetter"];
            if (string.IsNullOrEmpty(unitLetter))
            {
                throw new InvalidOperationException("Cannot find a 'UnitLetter' configuration key.");
            }

            var vcdMountPath = ConfigurationManager.AppSettings["VCDMountPath"];
            if (string.IsNullOrEmpty(vcdMountPath))
            {
                throw new InvalidOperationException("Cannot find a 'VCDMountPath' configuration key.");
            }

            VirtualCloneDrive = new VirtualCloneDriveWrapper(unitLetter, vcdMountPath);

            var logFolderPath = ConfigurationManager.AppSettings["LogFolderPath"];
            if (!Directory.Exists(logFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(logFolderPath);
                }
                catch
                {
                    logFolderPath = Path.GetTempPath();
                }
            }

            LogFolder = logFolderPath;
        }

        private void SaveScanLog(string isoFileName, string log)
        {
            try
            {
                System.IO.File.WriteAllText(Path.Combine(LogFolder, isoFileName + ".txt"), log);
            }
            catch
            {
                ShowLogMessage(string.Format("Cannot save log file for '{0}'.", isoFileName));
            }
        }

        private async void btnStartScan_Click(object sender, RoutedEventArgs e)
        {
            lstItems.Visibility = Visibility.Hidden;
            chkSelectAll.Visibility = Visibility.Hidden;
            txtConsole.Text = string.Empty;
            txtConsole.Visibility = Visibility.Visible;
            txtISOFolderPath.IsEnabled = false;
            btnAnalize.IsEnabled = false;
            btnStartScan.IsEnabled = false;
            btnRestart.IsEnabled = false;
           
            ShowLogMessage("Scan started...");
            TotalScanStartTime = DateTime.Now;

            // ISO Volumes
            ScanIsoVolumes();

            //ISO Compilations 
            await ScanIsoCompilations();

            ShowLogMessage("Scan finished.");
            var totalTime = string.Format("Total time: {0}",
                                          DateTime.Now.Subtract(TotalScanStartTime).ToString("hh\\:mm\\:ss"));
            ShowLogMessage(totalTime);
            ShowInformationMessage(string.Format("Scan finished. {0}", totalTime));
            btnStartScan.IsEnabled = true;
            btnRestart.IsEnabled = true;
        }

        private async Task ScanIsoCompilations()
        {
            IsoFilesPath = GetIsoFilesReadyToScan();
            ShowLogMessage("Scanning ISO compilations...");
            var isoFilesFoundMessage = string.Format("{0} ISO files found", IsoFilesPath.Count);
            ShowInformationMessage(isoFilesFoundMessage);
            ShowLogMessage(isoFilesFoundMessage);
            var currentIsoFilePathIndex = 0;
            var unmountResult = await UnMount();
            if (!unmountResult)
            {
                ShowLogMessage(string.Format("Error trying to unmount file from unit. Process aborted."));
                return;
            }

            foreach (var isoFilePath in IsoFilesPath)
            {
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
                                                         IsoFilesPath.Count));

                    if (!Entities.IsoVolumes.Any(i => i.VolumeLabel == volumeName))
                    {
                        var iso = new IsoVolume
                            {
                                DateCreated = DateTime.Now,
                                FileName = Path.GetFileName(IsoFilesPath[currentIsoFilePathIndex]),
                                Size = Convert.ToDecimal(VirtualCloneDrive.TotalSize),
                                VolumeLabel = volumeName
                            };

                        Entities.IsoVolumes.Add(iso);
                        Entities.SaveChanges();

                        var fileScanResult = new FileScanResult();
                        await IsoFileScanner.ScanFolderForFilesAsync(Entities, iso,
                                                                            VirtualCloneDrive.UnitLetter,
                                                                            fileScanResult);
                        iso.FileCount = fileScanResult.ProcessedFileCount;
                        Entities.Entry(iso).State = EntityState.Modified;
                        Entities.SaveChanges();
                        ShowLogMessage(string.Format(
                            "Scan finished for {0} ({1}), scan time: {2}, files processed: {3}",
                            volumeName, iso.FileName,
                            fileScanResult.TotalTime.ToString("hh\\:mm\\:ss"),
                            fileScanResult.ProcessedFileCount));
                        SaveScanLog(iso.FileName, fileScanResult.Log.ToString());
                    }
                    else
                    {
                        ShowLogMessage(string.Format("ISO '{0}' already exists on the database.", volumeName));
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
        }

        private void ScanIsoVolumes()
        {
            List<IsoItem> isoVolumeItems = GetIsoVolumes();
            if (isoVolumeItems.Count <= 0)
            {
                return;
            }

            ShowLogMessage("Scanning ISO Volumes...");
            var itemCount = 1;
            foreach (IsoItem item in isoVolumeItems)
            {
                ShowLogMessage("Scanning: " + item.IsoFileName);
                ShowInformationMessage(string.Format("Scanning Volume: {0} [{1} of {2}]", item.Name, itemCount,
                                                     isoVolumeItems.Count));
                var iso = new IsoVolume
                    {
                        DateCreated = DateTime.Now,
                        FileName = Path.GetFileName(item.IsoFileName),
                        Size = Convert.ToDecimal(VirtualCloneDrive.TotalSize),
                        VolumeLabel = VirtualCloneDrive.VolumeLabel,
                        IsClosedBox = true,
                    };
                foreach (string tag in item.Tags.Split(';'))
                {
                    iso.IsoTags.Add(new IsoTag {Name = tag});
                }

                Entities.IsoVolumes.Add(iso);
                itemCount += 1;
            }

            Entities.SaveChanges();
        }

        private async Task<bool> UnMount()
        {
            ShowLogMessage("Unmounting file...");
            var result = await VirtualCloneDrive.UnMountAsync();
            if (!result.HasError)
            {
                ShowLogMessage("Unmount successfully completed");
                return true;
            }
            ShowLogMessage("Unmount Error: " + result.ErrorMessage);
            return false;
        }

        private List<IsoItem> GetIsoVolumes()
        {
            return lstItems.Items.Cast<IsoItem>().Where(item => item.IsSelected && item.IsVolume).ToList();
        }

        private List<string> GetIsoFilesReadyToScan()
        {
            var isoFilesPath = new List<string>();
            foreach (IsoItem item in lstItems.Items)
            {
                if (item.IsSelected && item.IsVolume == false)
                {
                    isoFilesPath.Add(Path.Combine(txtISOFolderPath.Text, item.IsoFileName));
                }
            }

            return isoFilesPath;
        }

        private void ShowLogMessage(string message)
        {
            txtConsole.Text += string.Format("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), message) + Environment.NewLine;
        }

        private void ShowInformationMessage(string message)
        {
            lblInformation.Content = message;
        }

        private string[] GetIsoFilesFromFolder(string folderPath)
        {
            var isoFiles = Directory.GetFiles(folderPath, "*.iso");
            var binFiles = Directory.GetFiles(folderPath, "*.bin");
            return isoFiles.Concat(binFiles).ToArray();
        }

        private async void btnAnalize_Click(object sender, RoutedEventArgs e)
        {
            var isoFolderPath = txtISOFolderPath.Text;
            if (string.IsNullOrEmpty(isoFolderPath))
            {
                throw new ArgumentNullException(isoFolderPath);
            }

            if (!Directory.Exists(isoFolderPath))
            {
                throw new IOException(string.Format("Folder '{0}' doesn't exists or doesn't have access.", isoFolderPath));
            }

            lstItems.Items.Clear();
            lstItems.Visibility = Visibility.Visible;
            txtConsole.Visibility = Visibility.Hidden;
            chkSelectAll.Visibility = Visibility.Hidden;
            btnAnalize.IsEnabled = false;
            txtISOFolderPath.IsEnabled = false;
            btnStartScan.IsEnabled = false;
            btnRestart.IsEnabled = false;

            string[] isoFilesPath = GetIsoFilesFromFolder(isoFolderPath);
            if (isoFilesPath.Length > 0)
            {
                ShowInformationMessage(string.Format("{0} iso files found", isoFilesPath.Length));
                Array.Sort(isoFilesPath, (s1, s2) => string.Compare(Path.GetFileNameWithoutExtension(s1), Path.GetFileNameWithoutExtension(s2), StringComparison.Ordinal));
                foreach (var isoFilePath in isoFilesPath)
                {
                    var fileName = Path.GetFileNameWithoutExtension(isoFilePath);
                    var isExistingItem = await Task.Run(() => Entities.IsoVolumes.Any(f => f.FileName == fileName));
                    AddItemList(Path.GetFileName(isoFilePath), isExistingItem);
                }

                btnStartScan.IsEnabled = true;
                chkSelectAll.Visibility = Visibility.Visible;
            }
            else
            {
                ShowInformationMessage("No iso files found.");
                ShowLogMessage("No iso files found.");
            }

            btnAnalize.IsEnabled = true;
            txtISOFolderPath.IsEnabled = true;
        }

        private void AddItemList(string isofileName, bool isExistingItem)
        {
            lstItems.Items.Add(new IsoItem 
            {
                IsoFileName = isofileName,
                Width = lstItems.ActualWidth - 35,
                IsExistingItem = isExistingItem
            });
        }

        private void Window_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (lstItems.Visibility == Visibility.Visible)
            {
                foreach (IsoItem item in lstItems.Items)
                {
                    item.Width = lstItems.ActualWidth - 35;
                }
            }
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckAllItems(false);
        }

        private void CheckAllItems(bool isChecked)
        {
            foreach (IsoItem item in lstItems.Items)
            {
                item.IsSelected = isChecked;
            }
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckAllItems(true);
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            lstItems.Items.Clear();
            lstItems.Visibility = Visibility.Visible;
            chkSelectAll.IsChecked = false;
            chkSelectAll.Visibility = Visibility.Hidden;
            txtConsole.Text = string.Empty;
            txtConsole.Visibility = Visibility.Hidden;
            txtISOFolderPath.IsEnabled = true;
            btnAnalize.IsEnabled = true;
            btnStartScan.IsEnabled = false;
            btnRestart.IsEnabled = false;
                        
            ShowInformationMessage("Process restarted");
        }

        private void lnkOpenLogFolder_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(LogFolder);
        }
    }
}