using System;
using IsoFinder.Core;

namespace IsoFinder.Data
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<IsoFile> IsoFileRepository { get; }

        GenericRepository<IsoFolder> IsoFolderRepository { get; }

        GenericRepository<IsoVolume> IsoVolumeRepository { get; }

        GenericRepository<IsoRequest> IsoRequestRepository { get; }
        
        GenericRepository<IsoUser> IsoUserRepository { get; }
        
        void Save();
    }
}