using System;
using System.Collections.Generic;
using IsoFinder.Core;
using IsoFinder.Data;
using System.Linq;

namespace IsoFinder.Application
{
    public class IsoRequestService : IIsoRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IsoRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Insert(int userId, List<int> fileIds, List<int> folderIds)
        {
            var user = _unitOfWork.IsoUserRepository.Get(userId);
            var files = fileIds.Select(m => _unitOfWork.IsoFileRepository.Get(m)).ToList();
            var folders = folderIds.Select(m => _unitOfWork.IsoFolderRepository.Get(m)).ToList();
            var isoRequest = new IsoRequest 
            {
                Created = DateTime.Now,
                Files = files,
                Folders = folders,
                User = user,
                Completed = DateTime.Now,
                Status = IsoRequestStatus.New
            };

            _unitOfWork.IsoRequestRepository.Insert(isoRequest);
            _unitOfWork.Save();
        }

        public IEnumerable<IsoRequest> GetByStatus(IsoRequestStatus status)
        {
            return _unitOfWork.IsoRequestRepository.Search(i => i.Status == status, null, "User,Files,Folders");
        }

        public void Update(IsoRequest isoRequest)
        {
            _unitOfWork.IsoRequestRepository.Update(isoRequest);
            _unitOfWork.Save();
        }
    }
}