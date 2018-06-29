using System;
using System.Collections.Generic;
using IsoFinder.Core;

namespace IsoFinder.Application
{
    public interface IIsoRequestService
    {
        void Insert(int userId, List<int> fileIds, List<int> folderIds);

        IEnumerable<IsoRequest> GetByStatus(IsoRequestStatus status);

        void Update(IsoRequest isoRequest);
    }
}