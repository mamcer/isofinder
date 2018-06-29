using System;
using System.Collections.Generic;
using System.IO;
using IsoFinder.Core;

namespace IsoFinder.FileScanner
{
    public class FileScanner
    {
        public IDirectoryProvider DirectoryProvider { get; set; }

        public IIsoFileProvider FileProvider { get; set; }

        public FileScanner() : this(new DirectoryProvider(), new IsoFileProvider())
        {
        }

        public FileScanner(IDirectoryProvider directoryProvider, IIsoFileProvider fileProvider)
        {
            if (directoryProvider == null)
            {
                throw new ArgumentNullException("directoryProvider");
            }

            if (fileProvider == null)
            {
                throw new ArgumentNullException("fileProvider");
            }

            DirectoryProvider = directoryProvider;
            FileProvider = fileProvider;
        }

        public IsoFolder ScanFolderForFiles(string driveUnitLetter, FileScanResult fileScanResult, IsoVolume iso)
        {
            string fullDrivePath = driveUnitLetter + ":\\";
            var currentTime = DateTime.Now;
            var rootFolder = new IsoFolder
                                {
                                    Name = "/",
                                    Path = fullDrivePath.Replace(driveUnitLetter + @":\", "/"),
                                    Parent = null,
                                    IsoVolumeId = iso.Id
                                };

            var folders = new List<IsoFolder> { rootFolder };

            for (int i = 0; i < folders.Count; i++)
            {
                var currentFolder = folders[i];

                AddFiles(driveUnitLetter, currentFolder, fileScanResult, iso);

                AddFolders(driveUnitLetter, currentFolder, folders, iso);
            }

            fileScanResult.TotalTime = DateTime.Now.Subtract(currentTime);
            fileScanResult.Log.AppendLine(
                string.Format(
                    "Scan finished:{0}hs:{1}min:{2}sec:{3}msec, files:{4}, fileErrors:{5}, folders:{6}, folderErrors:{7}",
                    fileScanResult.TotalTime.Hours, fileScanResult.TotalTime.Minutes,
                    fileScanResult.TotalTime.Seconds, fileScanResult.TotalTime.Milliseconds,
                    fileScanResult.ProcessedFileCount, fileScanResult.FilesWithErrorCount,
                    fileScanResult.ProcessedFolderCount, fileScanResult.FoldersWithErrorCount));

            return rootFolder;
        }

        private void AddFolders(string driveUnitLetter, IsoFolder currentFolder, List<IsoFolder> folders, IsoVolume iso)
        {
            var folderPaths = DirectoryProvider.GetDirectories(currentFolder.Path.Replace("/", driveUnitLetter + @":\"));
            foreach (var path in folderPaths)
            {
                var childFolder = new IsoFolder
                {
                    Name = Path.GetFileName(path),
                    Path = path,
                    Parent = currentFolder,
                    IsoVolumeId = iso.Id
                };
                childFolder.Path = childFolder.Path.Replace(driveUnitLetter + @":\", "/");
                currentFolder.ChildFolders.Add(childFolder);
                folders.Add(childFolder);
            }
        }

        private void AddFiles(string driveUnitLetter, IsoFolder currentFolder, FileScanResult fileScanResult, IsoVolume iso)
        {
            string[] filePaths;

            try
            {
                filePaths = DirectoryProvider.GetFiles(currentFolder.Path.Replace("/", driveUnitLetter + @":\"), "*.*");
                fileScanResult.ProcessedFolderCount += 1;
            }
            catch (UnauthorizedAccessException)
            {
                fileScanResult.Log.AppendLine(string.Format("ERROR folder-access rights-:'{0}'", currentFolder.Path));
                fileScanResult.FoldersWithErrorCount += 1;
                return;
            }
            catch (Exception)
            {
                fileScanResult.Log.AppendLine(string.Format("ERROR folder-unknown-:'{0}'", currentFolder.Path));
                fileScanResult.FoldersWithErrorCount += 1;
                return;
            }

            foreach (var filePath in filePaths)
            {
                IsoFile isoFile;
                try
                {
                    isoFile = FileProvider.GetIsoFile(filePath);
                    isoFile.IsoVolumeId = iso.Id;
                }
                catch (UnauthorizedAccessException)
                {
                    fileScanResult.Log.AppendLine(string.Format("ERROR file-access rights-:'{0}'", filePath));
                    fileScanResult.FilesWithErrorCount += 1;
                    continue;
                }
                catch (Exception)
                {
                    fileScanResult.Log.AppendLine(string.Format("ERROR file-unknown-:'{0}'", filePath));
                    fileScanResult.FilesWithErrorCount += 1;
                    continue;
                }

                isoFile.Parent = currentFolder;
                fileScanResult.ProcessedFileCount += 1;
                currentFolder.ChildFiles.Add(isoFile);

                fileScanResult.Log.AppendLine(
                    string.Format(
                        "FILE name:'{0}', extension:'{1}', path:'{2}', dateCreated:{3}, dateModified:{4}, size:{5}",
                        isoFile.Name,
                        isoFile.Extension,
                        filePath,
                        isoFile.Created,
                        isoFile.Modified,
                        GetSizeString(isoFile.Size)
                        ));
            }
        }

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