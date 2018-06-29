using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IsoFinder.Core.Test
{
    [TestClass]
    public class PagedResultTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContructorWithNullItemsShouldThrowException()
        {
            // Arrange
            PagedResult<string> pagedResult;
            string[] items = null;
            var totalCount = 100;

            // Act
            pagedResult = new PagedResult<string>(items, totalCount);
        }

        [TestMethod]
        public void ContructorShouldSetProperties()
        { 
            // Arrange
            PagedResult<string> pagedResult;
            var items = new string[] { "this", "is", "a", "test" };
            var totalCount = 100;

            // Act
            pagedResult = new PagedResult<string>(items, totalCount);

            // Assert
            Assert.IsNotNull(pagedResult.Items);
            Assert.AreEqual(totalCount, pagedResult.TotalCount);
        }
    }
}
