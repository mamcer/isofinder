using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IsoFinder.FileScanner.Tests
{
    [TestClass]
    public class FileScanResultTest
    {
        [TestMethod]
        public void FileScanResultConstructorShouldInitializeValues()
        {
            //Arrange
            FileScanResult fileScanResult;

            //Act
            fileScanResult = new FileScanResult();

            //Assert
            Assert.AreEqual(0, fileScanResult.ProcessedFileCount);
            Assert.AreEqual(0, fileScanResult.FilesWithErrorCount);
            Assert.AreEqual(0, fileScanResult.ProcessedFolderCount);
            Assert.AreEqual(0, fileScanResult.FoldersWithErrorCount);
            Assert.IsNotNull(fileScanResult.Log);
            Assert.AreEqual(0, fileScanResult.Log.Length);
        }
    }
}