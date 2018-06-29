using IsoFinder.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace IsoFinder.FileScanner.Tests
{
    [TestClass]
    public class FileScannerTest
    {
        [TestMethod]
        public void FileScannerConstructorShouldSetProperties()
        {
            //Arrange
            FileScanner fileScanner;

            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();

            //Act
            fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);

            //Assert
            Assert.IsNotNull(fileScanner.DirectoryProvider);
            Assert.IsNotNull(fileScanner.FileProvider);
            Assert.AreEqual(directoryProvider.Object, fileScanner.DirectoryProvider);
            Assert.AreEqual(fileProvider.Object, fileScanner.FileProvider);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileScannerConstructorWithNullDirectoryProviderShouldThrowException()
        {
            //Arrange
            FileScanner fileScanner;
            var fileProvider = new Mock<IIsoFileProvider>();

            //Act
            fileScanner = new FileScanner(null, fileProvider.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileScannerConstructorWithNullFileProviderShouldThrowException()
        {
            //Arrange
            FileScanner fileScanner;
            var directoryProvider = new Mock<IDirectoryProvider>();

            //Act
            fileScanner = new FileScanner(directoryProvider.Object, null);
        }

        [TestMethod]
        [Ignore]
        public void ScanFolderForFilesShouldReturnRootFolder()
        { 
            // Arrange
            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();
            directoryProvider.Setup(m => m.GetDirectories(It.IsAny<string>())).Returns(new string[0]);
            var fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);
            IsoFolder rootFolder;
            string driveLetter = "D";
            FileScanResult fileScanResult = new FileScanResult();

            // Act
            rootFolder = fileScanner.ScanFolderForFiles(driveLetter, fileScanResult, null);

            // Assert
            Assert.IsNotNull(rootFolder);
            Assert.AreEqual("/", rootFolder.Name);
            Assert.AreEqual(driveLetter + ":\\", rootFolder.Path);
            Assert.IsNull(rootFolder.Parent);
            Assert.AreEqual(0, rootFolder.ChildFiles.Count);
            Assert.AreEqual(0, rootFolder.ChildFolders.Count);
            Assert.AreEqual(0, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(0, fileScanResult.FoldersWithErrorCount);
            Assert.AreEqual(0, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(1, fileScanResult.ProcessedFolderCount);
        }

        [TestMethod]
        [Ignore]
        public void ScanFolderForFilesShouldAddFiles()
        {
            // Arrange
            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file01.txt")).Returns(new IsoFile { Name = "file01.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file02.txt")).Returns(new IsoFile { Name = "file02.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file03.txt")).Returns(new IsoFile { Name = "file03.txt" });
            directoryProvider.Setup(m => m.GetDirectories(It.IsAny<string>())).Returns(new string[0]);
            directoryProvider.Setup(m => m.GetFiles(@"D:\", It.IsAny<string>())).Returns(new string[] { @"D:\file01.txt", @"D:\file02.txt", @"D:\file03.txt"});
            var fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);
            IsoFolder rootFolder;
            string driveLetter = "D";
            FileScanResult fileScanResult = new FileScanResult();

            // Act
            rootFolder = fileScanner.ScanFolderForFiles(driveLetter, fileScanResult, null);

            // Assert
            Assert.AreEqual(3, rootFolder.ChildFiles.Count);
            Assert.AreEqual(0, rootFolder.ChildFolders.Count);
            Assert.AreEqual("file01.txt", rootFolder.ChildFiles.ToList()[0].Name);
            Assert.AreEqual(0, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(0, fileScanResult.FoldersWithErrorCount);
            Assert.AreEqual(3, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(1, fileScanResult.ProcessedFolderCount);
        }

        [TestMethod]
        [Ignore]
        public void ScanFolderForFilesWithFolderUnathorizeAccessShowLogError()
        {
            // Arrange
            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file01.txt")).Returns(new IsoFile { Name = "file01.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file02.txt")).Returns(new IsoFile { Name = "file02.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file03.txt")).Returns(new IsoFile { Name = "file03.txt" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\")).Returns(new string[] { @"D:\protected" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\protected")).Returns(new string[0]);
            directoryProvider.Setup(m => m.GetFiles(@"D:\", It.IsAny<string>())).Returns(new string[] { @"D:\file01.txt", @"D:\file02.txt", @"D:\file03.txt" });
            directoryProvider.Setup(m => m.GetFiles(@"D:\protected", It.IsAny<string>())).Throws(new UnauthorizedAccessException());
            var fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);
            IsoFolder rootFolder;
            string driveLetter = "D";
            FileScanResult fileScanResult = new FileScanResult();

            // Act
            rootFolder = fileScanner.ScanFolderForFiles(driveLetter, fileScanResult, null);

            // Assert
            Assert.AreEqual(3, rootFolder.ChildFiles.Count);
            Assert.AreEqual(1, rootFolder.ChildFolders.Count);
            Assert.AreEqual("file01.txt", rootFolder.ChildFiles.ToList()[0].Name);
            Assert.AreEqual(0, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(1, fileScanResult.FoldersWithErrorCount);
            Assert.AreEqual(3, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(1, fileScanResult.ProcessedFolderCount);
        }

        [TestMethod]
        [Ignore]
        public void ScanFolderForFilesWithFolderExceptionShowLogError()
        {
            // Arrange
            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file01.txt")).Returns(new IsoFile { Name = "file01.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file02.txt")).Returns(new IsoFile { Name = "file02.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file03.txt")).Returns(new IsoFile { Name = "file03.txt" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\")).Returns(new string[] { @"D:\protected" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\protected")).Returns(new string[0]);
            directoryProvider.Setup(m => m.GetFiles(@"D:\", It.IsAny<string>())).Returns(new string[] { @"D:\file01.txt", @"D:\file02.txt", @"D:\file03.txt" });
            directoryProvider.Setup(m => m.GetFiles(@"D:\protected", It.IsAny<string>())).Throws(new Exception());
            var fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);
            IsoFolder rootFolder;
            string driveLetter = "D";
            FileScanResult fileScanResult = new FileScanResult();

            // Act
            rootFolder = fileScanner.ScanFolderForFiles(driveLetter, fileScanResult, null);

            // Assert
            Assert.AreEqual(3, rootFolder.ChildFiles.Count);
            Assert.AreEqual(1, rootFolder.ChildFolders.Count);
            Assert.AreEqual("file01.txt", rootFolder.ChildFiles.ToList()[0].Name);
            Assert.AreEqual(0, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(1, fileScanResult.FoldersWithErrorCount);
            Assert.AreEqual(3, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(1, fileScanResult.ProcessedFolderCount);
        }

        [TestMethod]
        [Ignore]
        public void ScanFolderForFilesWithFileUnathourizedExceptionShowLogError()
        {
            // Arrange
            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file01.txt")).Returns(new IsoFile { Name = "file01.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file02.txt")).Throws(new UnauthorizedAccessException());
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file03.txt")).Returns(new IsoFile { Name = "file03.txt" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\")).Returns(new string[0]);
            directoryProvider.Setup(m => m.GetFiles(@"D:\", It.IsAny<string>())).Returns(new string[] { @"D:\file01.txt", @"D:\file02.txt", @"D:\file03.txt" });
            var fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);
            IsoFolder rootFolder;
            string driveLetter = "D";
            FileScanResult fileScanResult = new FileScanResult();

            // Act
            rootFolder = fileScanner.ScanFolderForFiles(driveLetter, fileScanResult, null);

            // Assert
            Assert.AreEqual(2, rootFolder.ChildFiles.Count);
            Assert.AreEqual(0, rootFolder.ChildFolders.Count);
            Assert.AreEqual("file01.txt", rootFolder.ChildFiles.ToList()[0].Name);
            Assert.AreEqual(1, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(0, fileScanResult.FoldersWithErrorCount);
            Assert.AreEqual(2, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(1, fileScanResult.ProcessedFolderCount);
        }

        [TestMethod]
        [Ignore]
        public void ScanFolderForFilesWithFileExceptionShowLogError()
        {
            // Arrange
            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file01.txt")).Returns(new IsoFile { Name = "file01.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file02.txt")).Throws(new Exception());
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file03.txt")).Returns(new IsoFile { Name = "file03.txt" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\")).Returns(new string[0]);
            directoryProvider.Setup(m => m.GetFiles(@"D:\", It.IsAny<string>())).Returns(new string[] { @"D:\file01.txt", @"D:\file02.txt", @"D:\file03.txt" });
            var fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);
            IsoFolder rootFolder;
            string driveLetter = "D";
            FileScanResult fileScanResult = new FileScanResult();

            // Act
            rootFolder = fileScanner.ScanFolderForFiles(driveLetter, fileScanResult, null);

            // Assert
            Assert.AreEqual(2, rootFolder.ChildFiles.Count);
            Assert.AreEqual(0, rootFolder.ChildFolders.Count);
            Assert.AreEqual("file01.txt", rootFolder.ChildFiles.ToList()[0].Name);
            Assert.AreEqual(1, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(0, fileScanResult.FoldersWithErrorCount);
            Assert.AreEqual(2, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(1, fileScanResult.ProcessedFolderCount);
        }

        [TestMethod]
        [Ignore]
        public void ScanFolderForFilesWithMultipleFolderShouldWork()
        {
            // Arrange
            var directoryProvider = new Mock<IDirectoryProvider>();
            var fileProvider = new Mock<IIsoFileProvider>();
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file01.txt")).Returns(new IsoFile { Name = "file01.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file02.txt")).Returns(new IsoFile { Name = "file02.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\file03.txt")).Returns(new IsoFile { Name = "file03.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\second\file04.txt")).Returns(new IsoFile { Name = "file04.txt" });
            fileProvider.Setup(m => m.GetIsoFile(@"D:\second\file05.txt")).Returns(new IsoFile { Name = "file05.txt" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\")).Returns(new string[] { @"D:\second" });
            directoryProvider.Setup(m => m.GetDirectories(@"D:\second")).Returns(new string[0]);
            directoryProvider.Setup(m => m.GetFiles(@"D:\", It.IsAny<string>())).Returns(new string[] { @"D:\file01.txt", @"D:\file02.txt", @"D:\file03.txt" });
            directoryProvider.Setup(m => m.GetFiles(@"D:\second", It.IsAny<string>())).Returns(new string[] { @"D:\second\file04.txt", @"D:\second\file05.txt" });
            var fileScanner = new FileScanner(directoryProvider.Object, fileProvider.Object);
            IsoFolder rootFolder;
            string driveLetter = "D";
            FileScanResult fileScanResult = new FileScanResult();

            // Act
            rootFolder = fileScanner.ScanFolderForFiles(driveLetter, fileScanResult, null);

            // Assert
            Assert.AreEqual(3, rootFolder.ChildFiles.Count);
            Assert.AreEqual(1, rootFolder.ChildFolders.Count);
            Assert.AreEqual("file01.txt", rootFolder.ChildFiles.ToList()[0].Name);
            Assert.AreEqual(0, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(0, fileScanResult.FoldersWithErrorCount);
            Assert.AreEqual(5, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(2, fileScanResult.ProcessedFolderCount);
        }
    }
}