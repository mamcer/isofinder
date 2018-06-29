using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IsoFinder.Core.Test
{
    [TestClass]
    public class IsoFolderTest
    {
        [TestMethod]
        public void ConstructorShouldInitializeCollectionProperties()
        {
            // Arrange
            IsoFolder isoFolder;

            // Act
            isoFolder = new IsoFolder();

            // Assert
            Assert.IsNotNull(isoFolder.ChildFiles);
            Assert.IsNotNull(isoFolder.ChildFolders);
        }
    }
}