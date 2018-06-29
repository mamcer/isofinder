using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;

namespace IsoFinder.Data.Test
{
    [TestClass]
    public class GenericRepositoryTest
    {
        [TestMethod]
        public void GetShouldReturnDbSetFind()
        {
            // Arrange
            var isoFinderEntitiesMock = new Mock<IsoFinderEntities>();
            var dbSetMock = new Mock<DbSet<IsoFile>>();
            var isoFile = new IsoFile { Id = 1 };
            dbSetMock.Setup(s => s.Find(It.IsAny<object[]>())).Returns(isoFile).Verifiable();
            isoFinderEntitiesMock.Setup(pb => pb.Set<IsoFile>()).Returns(dbSetMock.Object);
            var isoFileRepository = new GenericRepository<IsoFile>(isoFinderEntitiesMock.Object);
            IsoFile result;

            // Act
            result = isoFileRepository.Get(1);

            // Assert
            Assert.IsNotNull(result);
            dbSetMock.VerifyAll();
        }

        [TestMethod]
        public void InsertShouldCallDbSetAdd()
        {
            // Arrange
            var isoFinderEntitiesMock = new Mock<IsoFinderEntities>();
            var dbSetMock = new Mock<DbSet<IsoFile>>();
            var isoFile = new IsoFile { Id = 1 };
            dbSetMock.Setup(s => s.Add(It.IsAny<IsoFile>())).Returns(isoFile).Verifiable();
            isoFinderEntitiesMock.Setup(pb => pb.Set<IsoFile>()).Returns(dbSetMock.Object);
            var isoFileRepository = new GenericRepository<IsoFile>(isoFinderEntitiesMock.Object);

            // Act
            isoFileRepository.Insert(isoFile);

            // Assert
            dbSetMock.VerifyAll();
        }

        [TestMethod]
        public void DeleteWithEntityAlreadyAddedShouldCallDbSetRemove()
        {
            // Arrange
            var isoFinderEntitiesMock = new Mock<IIsoFinderEntities>();
            var dbSetMock = new Mock<DbSet<IsoFile>>();
            var isoFile = new IsoFile { Id = 1 };
            dbSetMock.Setup(s => s.Remove(It.IsAny<IsoFile>())).Returns(isoFile).Verifiable();
            isoFinderEntitiesMock.Setup(pb => pb.GetSet<IsoFile>()).Returns(dbSetMock.Object);
            isoFinderEntitiesMock.Setup(pb => pb.GetState(isoFile)).Returns(EntityState.Added);
            var isoFileRepository = new GenericRepository<IsoFile>(isoFinderEntitiesMock.Object);

            // Act
            isoFileRepository.Delete(isoFile);

            // Assert
            dbSetMock.VerifyAll();
        }

        [TestMethod]
        public void DeleteWithEntityDetachedShouldAttachItBeforeCallRemove()
        {
            // Arrange
            var isoFinderEntitiesMock = new Mock<IIsoFinderEntities>();
            var dbSetMock = new Mock<DbSet<IsoFile>>();
            var isoFile = new IsoFile { Id = 1 };
            dbSetMock.Setup(s => s.Remove(It.IsAny<IsoFile>())).Returns(isoFile).Verifiable();
            dbSetMock.Setup(s => s.Attach(It.IsAny<IsoFile>())).Verifiable();
            isoFinderEntitiesMock.Setup(pb => pb.GetSet<IsoFile>()).Returns(dbSetMock.Object);
            isoFinderEntitiesMock.Setup(pb => pb.GetState(isoFile)).Returns(EntityState.Detached);
            var isoFileRepository = new GenericRepository<IsoFile>(isoFinderEntitiesMock.Object);

            // Act
            isoFileRepository.Delete(isoFile);

            // Assert
            dbSetMock.VerifyAll();
        }

        [TestMethod]
        public void UpdateShouldAttachAndCallSetModified()
        {
            // Arrange
            var isoFinderEntitiesMock = new Mock<IIsoFinderEntities>();
            var dbSetMock = new Mock<DbSet<IsoFile>>();
            var isoFile = new IsoFile { Id = 1 };
            dbSetMock.Setup(s => s.Attach(It.IsAny<IsoFile>())).Verifiable();
            isoFinderEntitiesMock.Setup(pb => pb.GetSet<IsoFile>()).Returns(dbSetMock.Object);
            isoFinderEntitiesMock.Setup(pb => pb.SetModified(isoFile)).Verifiable();
            var isoFileRepository = new GenericRepository<IsoFile>(isoFinderEntitiesMock.Object);

            // Act
            isoFileRepository.Update(isoFile);

            // Assert
            dbSetMock.VerifyAll();
        }
    }
}