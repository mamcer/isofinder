using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IsoFinder.Core.Test
{
    [TestClass]
    public class IsoRequestTest
    {
        [TestMethod]
        public void ConstructorShouldInitializeCollectionProperties()
        {
            // Arrange
            IsoRequest isoRequest;

            // Act
            isoRequest = new IsoRequest();

            // Assert
            Assert.IsNotNull(isoRequest.Folders);
            Assert.IsNotNull(isoRequest.Files);
        }
    }
}