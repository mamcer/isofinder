using System.Collections.Generic;
using System.Linq;
using IsoFinder.Core;
using IsoFinder.Data;

namespace IsoFinder.Application
{
    public class IsoFolderService : IIsoFolderService
    {
        private IUnitOfWork _unitOfWork;

        public IsoFolderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<IsoFolder> GetAll()
        {
            return _unitOfWork.IsoFolderRepository.GetAll();
        }

        public IsoFolder GetById(int id)
        {
            return _unitOfWork.IsoFolderRepository.Search(m => m.Id == id, null, "ChildFolders,ChildFiles").FirstOrDefault();
        }
    }
}