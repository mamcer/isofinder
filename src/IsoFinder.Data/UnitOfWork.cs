using System;
using IsoFinder.Core;

namespace IsoFinder.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IsoFinderEntities _context = new IsoFinderEntities();
        private GenericRepository<IsoFile> _isoFileRepository;
        private GenericRepository<IsoFolder> _isoFolderRepository;
        private GenericRepository<IsoVolume> _isoVolumeRepository;
        private GenericRepository<IsoUser> _isoUserRepository;
        private GenericRepository<IsoRequest> _isoRequestRepository;
        private bool _disposed;

        public GenericRepository<IsoFile> IsoFileRepository
        {
            get
            {
                if (_isoFileRepository == null)
                {
                    _isoFileRepository = new GenericRepository<IsoFile>(_context);
                }

                return _isoFileRepository;
            }
        }

        public GenericRepository<IsoFolder> IsoFolderRepository
        {
            get
            {
                if (_isoFolderRepository == null)
                {
                    _isoFolderRepository = new GenericRepository<IsoFolder>(_context);
                }

                return _isoFolderRepository;
            }
        }

        public GenericRepository<IsoVolume> IsoVolumeRepository
        {
            get
            {
                if (_isoVolumeRepository == null)
                {
                    _isoVolumeRepository = new GenericRepository<IsoVolume>(_context);
                }

                return _isoVolumeRepository;
            }
        }

        public GenericRepository<IsoRequest> IsoRequestRepository
        {
            get
            {
                if (_isoRequestRepository == null)
                {
                    _isoRequestRepository = new GenericRepository<IsoRequest>(_context);
                }

                return _isoRequestRepository;
            }
        }

        public GenericRepository<IsoUser> IsoUserRepository
        {
            get
            {
                if (_isoUserRepository == null)
                {
                    _isoUserRepository = new GenericRepository<IsoUser>(_context);
                }

                return _isoUserRepository;
            }
        }
        
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }
    }
}