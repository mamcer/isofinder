using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IsoFinder.Application;
using IsoFinder.Core;
using IsoFinder.Data;
using VirtualDrive;

namespace IsoFinder.Handler
{
    public partial class Main : Form
    {
        private readonly VirtualCloneDriveWrapper _virtualCloneDrive;
        private readonly List<IsoRequest> _pendingRequests;
        private IsoRequest _currentIsoRequestInfo;
        private IIsoRequestService _isoRequestService;
        private IIsoVolumeService _isoVolumeService;

        private readonly string _isoFilesPath;
        private readonly string _unitLetter;
        private readonly string _copyFileFolder;
        private readonly object _lockObject = new object();
        private bool _requestInProgress;
        private bool _closeApplication;

        private delegate void InvokeWriteMessage(string message);

        public Main()
        {
            InitializeComponent();

            _unitLetter = ConfigurationManager.AppSettings["UnitLetter"];
            if (string.IsNullOrEmpty(_unitLetter))
            {
                throw new Exception("UnitLetter app setting not found.");
            }

            string vcdMountPath = ConfigurationManager.AppSettings["VCDMountPath"];
            if (string.IsNullOrEmpty(vcdMountPath))
            {
                throw new Exception("VCDMountPath app setting not found.");
            }

            _copyFileFolder = ConfigurationManager.AppSettings["CopyFilesFolder"];
            if (string.IsNullOrEmpty(_copyFileFolder))
            {
                throw new Exception("CopyFileFolder app setting not found.");
            }

            _isoFilesPath = ConfigurationManager.AppSettings["IsoFilesPath"];
            if (string.IsNullOrEmpty(_isoFilesPath))
            {
                throw new Exception("IsoFilesPath app setting not found.");
            }

            var timerInterval = ConfigurationManager.AppSettings["TimerInterval"];
            if (string.IsNullOrEmpty(_isoFilesPath))
            {
                throw new Exception("TimerInterval app setting not found.");
            }

            _virtualCloneDrive = new VirtualCloneDriveWrapper(_unitLetter, vcdMountPath);

            _pendingRequests = new List<IsoRequest>();

            _isoRequestService = new IsoRequestService(new UnitOfWork());
            _isoVolumeService = new IsoVolumeService(new UnitOfWork());
            timer.Interval = Convert.ToInt32(timerInterval);
            timer.Enabled = true;
        }

        private async Task<bool> UnMount()
        {
            var result = await _virtualCloneDrive.UnMountAsync();
            if (!result.HasError)
            {
                return true;
            }

            WriteLogMessage("Unmount Error: " + result.ErrorMessage);
            return false;
        }

        private async void ProcessRequests()
        {
            _requestInProgress = true;
            for (int i = 0; i < _pendingRequests.Count; i++)
            {
                _currentIsoRequestInfo = _pendingRequests[i];
                WriteLogMessage(string.Format("Processing request: {0}. UserId: {1}",
                                              _currentIsoRequestInfo.Id,
                                              _currentIsoRequestInfo.User.Id));
                _currentIsoRequestInfo.Status = IsoRequestStatus.InProgress;
                _isoRequestService.Update(_currentIsoRequestInfo);

                var userName = RemoveInvalidCharacters(_currentIsoRequestInfo.User.Name);

                var tempPath = Path.GetTempPath();
                var sessionName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "-" + userName + "-" + RemoveInvalidCharacters(_currentIsoRequestInfo.Query);

                try
                {
                    var fileRequests = _currentIsoRequestInfo.Files.ToList();
                    if (fileRequests.Count > 0)
                    {
                        fileRequests.Sort((f, g) => f.IsoVolumeId.CompareTo(g.IsoVolumeId));
                        var lastIsoVolumeId = 0;
                        var isoVolumeFileName = string.Empty;
                        foreach (var fileRequest in fileRequests)
                        {
                            var unmountResult = false;

                            if (fileRequest.IsoVolumeId != lastIsoVolumeId)
                            {
                                isoVolumeFileName = _isoVolumeService.GetById(fileRequest.IsoVolumeId).FileName;
                                WriteLogMessage("Unmounting file...");
                                unmountResult = await UnMount();
                                WriteLogMessage("Unmount successfully completed");
                                WriteLogMessage(string.Format("Mounting file '{0}'...", isoVolumeFileName));

                                var mountResult = await Mount(Path.Combine(_isoFilesPath, isoVolumeFileName));
                                if (mountResult)
                                {
                                    WriteLogMessage("Mount successfully completed");
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            lastIsoVolumeId = fileRequest.IsoVolumeId;
                            var folderPath = fileRequest.Parent.Path.Replace("/", _unitLetter + ":\\");
                            CopyFileToTempFolder(sessionName, fileRequest.Name, folderPath, tempPath);
                            WriteLogMessage(string.Format("File successfully copied from: '{0}' to: '{1}'",
                                Path.Combine(folderPath, fileRequest.Name),
                                Path.Combine(_copyFileFolder, fileRequest.Name)));
                        }
                    }

                    var folderRequests = _currentIsoRequestInfo.Folders.ToList();
                    if (folderRequests.Count > 0)
                    {
                        folderRequests.Sort((f, g) => f.IsoVolumeId.CompareTo(g.IsoVolumeId));
                        var lastIsoVolumeId = 0;
                        var isoVolumeFileName = string.Empty;
                        foreach (var folderRequest in folderRequests)
                        {
                            var unmountResult = false;
                            if (folderRequest.IsoVolumeId != lastIsoVolumeId)
                            {
                                isoVolumeFileName = _isoVolumeService.GetById(folderRequest.IsoVolumeId).FileName;
                                WriteLogMessage("Unmounting file...");
                                unmountResult = await UnMount();
                                WriteLogMessage("Unmount successfully completed");
                                WriteLogMessage(string.Format("Mounting file '{0}'...", isoVolumeFileName));

                                var mountResult = await Mount(Path.Combine(_isoFilesPath, isoVolumeFileName));
                                if (mountResult)
                                {
                                    WriteLogMessage("Mount successfully completed");
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            lastIsoVolumeId = folderRequest.IsoVolumeId;
                            var folderPath = folderRequest.Path.Replace("/", _unitLetter + ":\\");
                            CopyFolderToTempFolder(sessionName, Path.GetFileName(folderPath), folderPath, tempPath);
                            WriteLogMessage(string.Format("Folder successfully copied from: '{0}' to: '{1}'",
                                folderPath,
                                Path.Combine(_copyFileFolder, Path.GetFileName(folderPath))));
                        }

                        ZipFolder(sessionName, _copyFileFolder, tempPath);
                        _currentIsoRequestInfo.Status = IsoRequestStatus.Done;
                        _currentIsoRequestInfo.FileName = sessionName + ".zip";
                        _isoRequestService.Update(_currentIsoRequestInfo);
                    }
                }
                catch (Exception ex)
                {
                    WriteLogMessage(ex.Message);
                    WriteLogMessage(ex.StackTrace);
                    _currentIsoRequestInfo.Status = IsoRequestStatus.Failed;
                    _isoRequestService.Update(_currentIsoRequestInfo);
                }
            }

            lock (_lockObject)
            {
                _pendingRequests.Clear();
                _requestInProgress = false;
            }

            WriteLogMessage("Process finished.");
            await UnMount();
        }

        private object RemoveInvalidCharacters(string potentialFileName)
        {
            if (string.IsNullOrEmpty(potentialFileName))
            {
                return string.Empty;
            }

            string fileName = potentialFileName;
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }

        private async Task<bool> Mount(string isoFilePath)
        {
            var mountResult = await _virtualCloneDrive.MountAsync(isoFilePath);
            if (mountResult.HasError)
            {
                WriteLogMessage(string.Format("There was an error mounting file: {0}.", isoFilePath));
                WriteLogMessage(string.Format("ERROR: {0}.", mountResult.ErrorMessage));

                return false;
            }

            return true;
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
                File.SetAttributes(temppath, FileAttributes.Normal);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void CopyFolderToTempFolder(string sessionName, string folderName, string fromPath, string tempFolder)
        {
            try
            {
                var tempFolderPath = Path.Combine(tempFolder, sessionName);
                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }

                var tempFolderDestination = Path.Combine(tempFolderPath, folderName);
                if (!Directory.Exists(tempFolderDestination))
                {
                    Directory.CreateDirectory(tempFolderDestination);
                }

                DirectoryCopy(fromPath, tempFolderDestination, true);
            }
            catch (Exception ex)
            {
                WriteLogMessage("Error copying files: " + ex.Message);
            }
        }

        private void CopyFileToTempFolder(string sessionName, string fileName, string fromPath, string tempFolder)
        {
            try
            {
                var tempFolderPath = Path.Combine(tempFolder, sessionName);
                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }

                var tempFilePath = Path.Combine(tempFolderPath, fileName);
                File.Copy(Path.Combine(fromPath, fileName), tempFilePath, true);
                File.SetAttributes(tempFilePath, FileAttributes.Normal);
            }
            catch(Exception ex)
            {
                WriteLogMessage("Error copying files: " + ex.Message);
            }
        }

        private void ZipFolder(string sessionName, string toPath, string tempFolder)
        {
            try
            {
                var tempFolderPath = Path.Combine(tempFolder, sessionName);
                var zipFilePath = Path.Combine(toPath, sessionName + ".zip");
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath);
                File.SetAttributes(zipFilePath, FileAttributes.Normal);
                Directory.Delete(tempFolderPath, true);
            }
            catch (Exception ex)
            {
                WriteLogMessage("Error zipping files: " + ex.Message);
            }
        }

        private void WriteMessage(string message)
        {
            txtConsole.Text += string.Format("{0} - {1}{2}", DateTime.Now.ToString("yyyy.MM.dd-HH:mm:ss"), message,
                                     Environment.NewLine);
        }

        private void WriteLogMessage(string message)
        {
            InvokeWriteMessage a = WriteMessage;
            txtConsole.Invoke(a, message);
        }

        private void lnkClearLogInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show(IsoFinder.Handler.Properties.Resources.ClearConsoleWindowQuestion, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                txtConsole.Text = string.Empty;
            }
        }

        private void lnkSaveLogInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileNames[0], txtConsole.Text);
                WriteLogMessage("Console log successfully saved to: " + saveFileDialog.FileNames[0]);
            }
        }

        private void lnkOpenDropFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(_copyFileFolder);
        }

        private void btnCheckRequests_Click(object sender, EventArgs e)
        {
            CheckRequests();
        }

        private void CheckRequests()
        {
            timer.Enabled = false;

            var isoRequests = _isoRequestService.GetByStatus(IsoRequestStatus.New).ToList();

            if (isoRequests.Count > 0)
            {
                foreach (var isoRequest in isoRequests)
                {
                    lock (_lockObject)
                    {
                        _pendingRequests.Add(isoRequest);
                    }

                    if (!_requestInProgress)
                    {
                        ProcessRequests();
                    }
                }
            }
            else
            {
                lblStatus.Text = string.Format("Last Check: {0} - {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
            }

            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            CheckRequests();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            BringToFront();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _closeApplication = true;
            Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_closeApplication)
            {
                WindowState = FormWindowState.Minimized;
                Visible = false;
                e.Cancel = true;

                ShowTrayMessage("Running in background");
            }
        }

        private void ShowTrayMessage(string message)
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipTitle = Text;
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(3000);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            BringToFront();
        }

        private void checkRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckRequests();
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
            }
        }
    }
}